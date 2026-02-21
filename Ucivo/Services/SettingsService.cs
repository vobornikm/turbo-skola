using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TurboSkola.Data;
using TurboSkola.Data.Models;

namespace TurboSkola.Services;

public class TrainingSettings
{
    public string DurationType { get; set; } = "Time";
    public int DurationValue { get; set; } = 10;
    public List<int> IncludedMultipliers { get; set; } = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public bool IncludeMultiplication { get; set; } = true;
    public bool IncludeDivision { get; set; } = false;
}

public interface ISettingsService
{
    Task<TrainingSettings> GetProfileSettingsAsync(int? profileId);
    Task SaveProfileSettingsAsync(int profileId, TrainingSettings settings);
}

public class SettingsService : ISettingsService
{
    private readonly UcivoDbContext _dbContext;

    public SettingsService(UcivoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TrainingSettings> GetProfileSettingsAsync(int? profileId)
    {
        UserSettings? userSettings = null;

        if (profileId.HasValue)
        {
            userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId);
        }

        if (userSettings == null)
        {
            return new TrainingSettings();
        }

        var multipliers = JsonSerializer.Deserialize<List<int>>(userSettings.IncludedMultipliers) 
            ?? new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        return new TrainingSettings
        {
            DurationType = userSettings.DurationType,
            DurationValue = userSettings.DurationValue,
            IncludedMultipliers = multipliers,
            IncludeMultiplication = userSettings.IncludeMultiplication,
            IncludeDivision = userSettings.IncludeDivision
        };
    }

    public async Task SaveProfileSettingsAsync(int profileId, TrainingSettings settings)
    {
        var userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId);
        if (userSettings == null)
        {
            userSettings = new UserSettings { ProfileId = profileId };
            _dbContext.UserSettings.Add(userSettings);
        }

        userSettings.DurationType = settings.DurationType;
        userSettings.DurationValue = settings.DurationValue;
        userSettings.IncludedMultipliers = JsonSerializer.Serialize(settings.IncludedMultipliers);
        userSettings.IncludeMultiplication = settings.IncludeMultiplication;
        userSettings.IncludeDivision = settings.IncludeDivision;
        userSettings.LastModified = DateTime.UtcNow;

        _dbContext.UserSettings.Update(userSettings);
        await _dbContext.SaveChangesAsync();
    }
}
