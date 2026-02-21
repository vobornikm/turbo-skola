using Microsoft.EntityFrameworkCore;
using TurboSkola.Data;
using TurboSkola.Data.Models;

namespace TurboSkola.Services;

public interface IProfileService
{
    Task<List<UserProfile>> GetUserProfilesAsync(int userId);
    Task<UserProfile?> GetProfileAsync(int profileId);
    Task<UserProfile> CreateProfileAsync(int userId, string profileName, bool isParent = false, string? pinCode = null, string? avatarColor = null, string? avatarIcon = null);
    Task<bool> DeleteProfileAsync(int profileId);
    Task<bool> UpdateProfileNameAsync(int profileId, string newName);
    Task<bool> UpdateProfileOrderAsync(int profileId, int newOrder);
    Task<bool> UpdateProfileAsync(int profileId, string name, string? pinCode, string avatarColor, string avatarIcon);
    Task<bool> VerifyPinAsync(int profileId, string pin);
    Task<bool> CanCreateParentProfileAsync(int userId);
    Task<bool> CanCreateChildProfileAsync(int userId);
    int? CurrentProfileId { get; }
    string? CurrentProfileName { get; }
    bool IsCurrentProfileParent { get; }
    string? CurrentAvatarIcon { get; }
    string? CurrentAvatarColor { get; }
    void SetActiveProfile(int profileId, string profileName, bool isParent, string avatarIcon, string avatarColor);
    void ClearActiveProfile();
    event Action? OnProfileChanged;
}


public class ProfileService : IProfileService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private int? _currentProfileId;
    private string? _currentProfileName;
    private bool _isCurrentProfileParent;
    private string? _currentAvatarIcon;
    private string? _currentAvatarColor;
    
    public int? CurrentProfileId => _currentProfileId;
    public string? CurrentProfileName => _currentProfileName;
    public bool IsCurrentProfileParent => _isCurrentProfileParent;
    public string? CurrentAvatarIcon => _currentAvatarIcon;
    public string? CurrentAvatarColor => _currentAvatarColor;
    
    public event Action? OnProfileChanged;

    // Konstanty pro limity
    private const int MAX_PARENT_PROFILES = 2;
    private const int MAX_CHILD_PROFILES = 4;

    public ProfileService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void SetActiveProfile(int profileId, string profileName, bool isParent, string avatarIcon, string avatarColor)
    {
        _currentProfileId = profileId;
        _currentProfileName = profileName;
        _isCurrentProfileParent = isParent;
        _currentAvatarIcon = avatarIcon;
        _currentAvatarColor = avatarColor;
        OnProfileChanged?.Invoke();
    }

    public void ClearActiveProfile()
    {
        _currentProfileId = null;
        _currentProfileName = null;
        _isCurrentProfileParent = false;
        _currentAvatarIcon = null;
        _currentAvatarColor = null;
        OnProfileChanged?.Invoke();
    }

    public async Task<List<UserProfile>> GetUserProfilesAsync(int userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        return await context.UserProfiles
            .Where(p => p.UserId == userId && p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<bool> CanCreateParentProfileAsync(int userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        var parentCount = await context.UserProfiles
            .CountAsync(p => p.UserId == userId && p.IsActive && p.IsParentProfile);
        
        return parentCount < MAX_PARENT_PROFILES;
    }

    public async Task<bool> CanCreateChildProfileAsync(int userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        var childCount = await context.UserProfiles
            .CountAsync(p => p.UserId == userId && p.IsActive && !p.IsParentProfile);
        
        return childCount < MAX_CHILD_PROFILES;
    }

    public async Task<bool> VerifyPinAsync(int profileId, string pin)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        var profile = await context.UserProfiles.FindAsync(profileId);
        if (profile == null || string.IsNullOrEmpty(profile.PinCode))
            return false;
        
        return profile.PinCode == pin;
    }

    public async Task<UserProfile?> GetProfileAsync(int profileId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        return await context.UserProfiles
            .FirstOrDefaultAsync(p => p.ProfileId == profileId && p.IsActive);
    }

    public async Task<UserProfile> CreateProfileAsync(int userId, string profileName, bool isParent = false, string? pinCode = null, string? avatarColor = null, string? avatarIcon = null)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        // Validace limitù
        if (isParent)
        {
            if (!await CanCreateParentProfileAsync(userId))
                throw new InvalidOperationException($"Nelze vytvoøit více než {MAX_PARENT_PROFILES} rodièovské profily.");
        }
        else
        {
            if (!await CanCreateChildProfileAsync(userId))
                throw new InvalidOperationException($"Nelze vytvoøit více než {MAX_CHILD_PROFILES} dìtské profily.");
        }
        
        // Zjistit nejvyšší DisplayOrder
        var maxOrder = await context.UserProfiles
            .Where(p => p.UserId == userId && p.IsActive)
            .MaxAsync(p => (int?)p.DisplayOrder) ?? -1;
        
        var profile = new UserProfile
        {
            UserId = userId,
            ProfileName = profileName,
            IsActive = true,
            IsParentProfile = isParent,
            PinCode = pinCode,
            AvatarColor = avatarColor ?? "#667eea",
            AvatarIcon = avatarIcon ?? (isParent ? "parent" : "child"),
            DisplayOrder = maxOrder + 1,
            CreatedDate = DateTime.UtcNow
        };

        context.UserProfiles.Add(profile);
        await context.SaveChangesAsync();

        // Vytvoøit výchozí nastavení pro profil
        var settings = new UserSettings
        {
            ProfileId = profile.ProfileId,
            DurationType = "Time",
            DurationValue = 10,
            IncludedMultipliers = "[0,1,2,3,4,5,6,7,8,9,10]",
            IncludeMultiplication = true,
            IncludeDivision = false,
            LastModified = DateTime.UtcNow
        };

        context.UserSettings.Add(settings);
        await context.SaveChangesAsync();

        return profile;
    }

    public async Task<bool> UpdateProfileOrderAsync(int profileId, int newOrder)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        var profile = await context.UserProfiles.FindAsync(profileId);
        if (profile == null) return false;

        profile.DisplayOrder = newOrder;
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteProfileAsync(int profileId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        var profile = await context.UserProfiles.FindAsync(profileId);
        if (profile == null) return false;

        profile.IsActive = false;
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateProfileNameAsync(int profileId, string newName)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        var profile = await context.UserProfiles.FindAsync(profileId);
        if (profile == null) return false;

        profile.ProfileName = newName;
        await context.SaveChangesAsync();

        if (_currentProfileId == profileId)
        {
            _currentProfileName = newName;
            OnProfileChanged?.Invoke();
        }

        return true;
    }

    public async Task<bool> UpdateProfileAsync(int profileId, string name, string? pinCode, string avatarColor, string avatarIcon)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
        
        var profile = await context.UserProfiles.FindAsync(profileId);
        if (profile == null) return false;

        profile.ProfileName = name;
        if (profile.IsParentProfile && pinCode != null)
        {
            profile.PinCode = pinCode;
        }
        profile.AvatarColor = avatarColor;
        profile.AvatarIcon = avatarIcon;
        
        await context.SaveChangesAsync();

        if (_currentProfileId == profileId)
        {
            _currentProfileName = name;
            OnProfileChanged?.Invoke();
        }

        return true;
    }
}

