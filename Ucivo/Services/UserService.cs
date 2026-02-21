using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TurboSkola.Data;
using TurboSkola.Data.Models;

namespace TurboSkola.Services;

public interface IUserService
{
    Task<User?> RegisterAsync(string email, string password, string parentName, string parentPin, List<string> childNames);
    Task<User?> LoginAsync(string email, string password);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> EmailExistsAsync(string email);
}

public class UserService : IUserService
{
    private readonly UcivoDbContext _dbContext;

    public UserService(UcivoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> RegisterAsync(string email, string password, string parentName, string parentPin, List<string> childNames)
    {
        if (await EmailExistsAsync(email))
            return null;

        var user = new User
        {
            Email = email,
            PasswordHash = HashPassword(password),
            RegistrationDate = DateTime.UtcNow,
            IsActive = true
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        // Vytvoøení rodièovského profilu
        var parentProfile = new UserProfile
        {
            UserId = user.UserId,
            ProfileName = parentName,
            IsParentProfile = true,
            PinCode = parentPin,
            AvatarColor = "#667eea",
            AvatarIcon = "parent",
            DisplayOrder = 0,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _dbContext.UserProfiles.Add(parentProfile);
        await _dbContext.SaveChangesAsync();

        // Vytvoøení defaultních nastavení pro rodièovský profil
        var parentSettings = new UserSettings
        {
            ProfileId = parentProfile.ProfileId,
            DurationType = "Time",
            DurationValue = 10,
            IncludedMultipliers = "[0,1,2,3,4,5,6,7,8,9,10]",
            IncludeMultiplication = true,
            IncludeDivision = false,
            LastModified = DateTime.UtcNow
        };

        _dbContext.UserSettings.Add(parentSettings);

        // Vytvoøení dìtských profilù
        if (childNames != null && childNames.Any())
        {
            int order = 1;
            var colors = new[] { "#4299e1", "#48bb78", "#ecc94b", "#f6ad55" };
            var icons = new[] { "child", "fox", "bear", "cat" };

            foreach (var childName in childNames.Where(n => !string.IsNullOrWhiteSpace(n)))
            {
                var childProfile = new UserProfile
                {
                    UserId = user.UserId,
                    ProfileName = childName.Trim(),
                    IsParentProfile = false,
                    PinCode = null,
                    AvatarColor = colors[(order - 1) % colors.Length],
                    AvatarIcon = icons[(order - 1) % icons.Length],
                    DisplayOrder = order,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.UserProfiles.Add(childProfile);
                await _dbContext.SaveChangesAsync();

                // Vytvoøení defaultních nastavení pro dìtský profil
                var childSettings = new UserSettings
                {
                    ProfileId = childProfile.ProfileId,
                    DurationType = "Time",
                    DurationValue = 10,
                    IncludedMultipliers = "[0,1,2,3,4,5,6,7,8,9,10]",
                    IncludeMultiplication = true,
                    IncludeDivision = false,
                    LastModified = DateTime.UtcNow
                };

                _dbContext.UserSettings.Add(childSettings);
                order++;
            }
        }

        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        if (user == null)
            return null;

        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == email);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
