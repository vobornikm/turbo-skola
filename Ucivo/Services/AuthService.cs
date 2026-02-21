using TurboSkola.Data.Models;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;

namespace TurboSkola.Services;

public interface IAuthService
{
    event Action? OnAuthStateChanged;
    
    Task<bool> LoginAsync(string email, string password, bool rememberMe = false);
    Task<bool> RegisterAsync(string email, string password);
    Task LogoutAsync();
    Task<User?> GetCurrentUserAsync();
    Task<bool> TryAutoLoginAsync();
    bool IsAuthenticated { get; }
    int? CurrentUserId { get; }
    string? CurrentEmail { get; }
}

public class AuthService : IAuthService
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    // Pro lokální aplikaci staèí jeden uživatel
    private User? _currentUser;
    private bool _isInitialized = false;
    private readonly object _lock = new();
    
    // Event pro notifikaci zmìny auth stavu
    public event Action? OnAuthStateChanged;

    public bool IsAuthenticated => _currentUser != null;
    public int? CurrentUserId => _currentUser?.UserId;
    public string? CurrentEmail => _currentUser?.Email;

    public AuthService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private User? GetCurrentUser()
    {
        lock (_lock)
        {
            return _currentUser;
        }
    }

    private void SetCurrentUser(User? user)
    {
        lock (_lock)
        {
            _currentUser = user;
        }
        // Notifikovat zmìnu auth stavu
        OnAuthStateChanged?.Invoke();
    }

    public async Task<bool> LoginAsync(string email, string password, bool rememberMe = false)
    {
        Console.WriteLine($"[AuthService] LoginAsync started for: {email}");
        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        
        var user = await userService.LoginAsync(email, password);
        Console.WriteLine($"[AuthService] User from DB: {(user != null ? user.Email : "NULL")}");
        
        if (user != null)
        {
            SetCurrentUser(user);
            Console.WriteLine($"[AuthService] User set, IsAuthenticated: {IsAuthenticated}");
            
            if (rememberMe)
            {
                try
                {
                    var jsRuntime = scope.ServiceProvider.GetRequiredService<IJSRuntime>();
                    var expirationDate = DateTime.UtcNow.AddDays(30);
                    await jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", user.UserId.ToString());
                    await jsRuntime.InvokeVoidAsync("localStorage.setItem", "userEmail", user.Email);
                    await jsRuntime.InvokeVoidAsync("localStorage.setItem", "loginExpiration", expirationDate.ToString("o"));
                    Console.WriteLine($"[AuthService] Saved to localStorage");
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("statically rendered"))
                {
                    // JSInterop není dostupný bìhem prerendering - ignorovat, zkusíme pozdìji
                    Console.WriteLine($"[AuthService] localStorage not available during prerendering, will retry later");
                }
            }
            
            Console.WriteLine($"[AuthService] Returning TRUE");
            return true;
        }
        Console.WriteLine($"[AuthService] Returning FALSE");
        return false;
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < 6)
            return false;

        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        
        // Výchozí rodièovský profil s PINEM "0000"
        var user = await userService.RegisterAsync(email, password, "Rodiè", "0000", new List<string>());
        if (user != null)
        {
            SetCurrentUser(user);
            return true;
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        SetCurrentUser(null);
        
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var jsRuntime = scope.ServiceProvider.GetRequiredService<IJSRuntime>();
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userEmail");
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "loginExpiration");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("statically rendered"))
        {
            // localStorage není dostupný bìhem prerendering - ignorovat
            Console.WriteLine("[AuthService] localStorage not available during prerendering");
        }
        catch
        {
            // localStorage mùže být nedostupný z jiných dùvodù
        }
    }

    public Task<User?> GetCurrentUserAsync()
    {
        return Task.FromResult(GetCurrentUser());
    }

    public async Task<bool> TryAutoLoginAsync()
    {
        if (_isInitialized)
            return IsAuthenticated;
            
        _isInitialized = true;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var jsRuntime = scope.ServiceProvider.GetRequiredService<IJSRuntime>();
            
            var userIdStr = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");
            var expirationStr = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "loginExpiration");

            if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(expirationStr))
                return false;

            if (!DateTime.TryParse(expirationStr, out var expiration) || expiration < DateTime.UtcNow)
            {
                await LogoutAsync();
                return false;
            }

            if (!int.TryParse(userIdStr, out var userId))
                return false;

            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            
            var user = await userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                SetCurrentUser(user);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TryAutoLoginAsync error: {ex.Message}");
        }

        return false;
    }
}
