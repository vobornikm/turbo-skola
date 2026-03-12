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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ITrainingService, TrainingService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IPreparedTrainingService, PreparedTrainingService>();
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
    
    // ?? DEBUG: Zobraz co vidí aplikace
    Console.WriteLine($"?? ENVIRONMENT: {app.Environment.EnvironmentName}");
    Console.WriteLine($"?? CONNECTION STRING: {app.Configuration.GetConnectionString("DefaultConnection")}");
    
    // Automaticky aplikovat migrace
    context.Database.Migrate();
    
    // Seed data
    await DataSeeder.SeedAsync(context);
}

app.Run();
