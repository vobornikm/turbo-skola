using Microsoft.EntityFrameworkCore;
using TurboSkola.Components;
using TurboSkola.Data;
using TurboSkola.Services;
using TurboSkola.Utilities;
using TurboSkola.TrainingGenerators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure DbContext
builder.Services.AddDbContext<UcivoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IProfileService, ProfileService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ITrainingService, TrainingService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IPerformanceMonitor, PerformanceMonitor>();
builder.Services.AddScoped<ExerciseGenerator>();

// Register new training architecture services
builder.Services.AddSingleton<IExerciseGeneratorFactory, ExerciseGeneratorFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Apply pending migrations and seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();
    
    // Automaticky aplikovat migrace
    context.Database.Migrate();
    
    // Seed data
    await DataSeeder.SeedAsync(context);
}

app.Run();
