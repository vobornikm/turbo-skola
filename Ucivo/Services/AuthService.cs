using TurboSkola.Data.Models;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;

namespace TurboSkola.Services;

public interface IAuthService
{
    event Action? OnAuthStateChanged;

    Task<bool> LoginAsync(string email, string password, bool rememberMe, IJSRuntime jsRuntime);
    Task<bool> RegisterAsync(string email, string password);
    Task LogoutAsync(IJSRuntime jsRuntime);
    Task<User?> GetCurrentUserAsync();
    Task<bool> TryAutoLoginAsync(IJSRuntime jsRuntime);
    bool IsAuthenticated { get; }
    int? CurrentUserId { get; }
    string? CurrentEmail { get; }
}

public class AuthService : IAuthService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private User? _currentUser;
    private bool _isInitialized = false;
    private readonly object _lock = new();

    public event Action? OnAuthStateChanged;

    public bool IsAuthenticated => _currentUser != null;
    public int? CurrentUserId => _currentUser?.UserId;
    public string? CurrentEmail => _currentUser?.Email;

    public AuthService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private void SetCurrentUser(User? user)
    {
        lock (_lock) { _currentUser = user; }
        OnAuthStateChanged?.Invoke();
    }

    public async Task<bool> LoginAsync(string email, string password, bool rememberMe, IJSRuntime jsRuntime)
    {
        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        var user = await userService.LoginAsync(email, password);
        if (user == null) return false;

        SetCurrentUser(user);

        try
        {
            // V�dy ulo�it do sessionStorage (p�e�ije refresh, ne zav�en� okna)
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "userId", user.UserId.ToString());
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "userEmail", user.Email);

            // Pokud rememberMe ? ulo�it i do localStorage (30 dn�)
            if (rememberMe)
            {
                var expiration = DateTime.UtcNow.AddDays(30).ToString("o");
                await jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", user.UserId.ToString());
                await jsRuntime.InvokeVoidAsync("localStorage.setItem", "userEmail", user.Email);
                await jsRuntime.InvokeVoidAsync("localStorage.setItem", "loginExpiration", expiration);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] LoginAsync storage error: {ex.Message}");
        }

        return true;
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return false;

        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        var user = await userService.RegisterAsync(email, password, "Rodi�", "0000", new List<string>());
        if (user != null)
        {
            SetCurrentUser(user);
            return true;
        }
        return false;
    }

    public async Task LogoutAsync(IJSRuntime jsRuntime)
    {
        SetCurrentUser(null);
        _isInitialized = false;

        try
        {
            await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "userId");
            await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "userEmail");
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userEmail");
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "loginExpiration");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] LogoutAsync storage error: {ex.Message}");
        }
    }

    public Task<User?> GetCurrentUserAsync() => Task.FromResult(_currentUser);

    public async Task<bool> TryAutoLoginAsync(IJSRuntime jsRuntime)
    {
        if (_isInitialized) return IsAuthenticated;

        try
        {
            // 1. Zkus sessionStorage (p�e�ije refresh)
            var userIdStr = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "userId");

            // 2. Fallback na localStorage (rememberMe)
            if (string.IsNullOrEmpty(userIdStr))
            {
                userIdStr = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");

                if (!string.IsNullOrEmpty(userIdStr))
                {
                    var expirationStr = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "loginExpiration");
                    if (string.IsNullOrEmpty(expirationStr) ||
                        !DateTime.TryParse(expirationStr, out var expiration) ||
                        expiration < DateTime.UtcNow)
                    {
                        await LogoutAsync(jsRuntime);
                        _isInitialized = true;
                        return false;
                    }
                }
            }

            _isInitialized = true;

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return false;

            using var scope = _scopeFactory.CreateScope();
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
            Console.WriteLine($"[AuthService] TryAutoLoginAsync error: {ex.Message}");
            _isInitialized = true;
        }

        return false;
    }
}
