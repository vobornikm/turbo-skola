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
    public bool EnableReadAloud { get; set; } = false;
}

public class AddSubTrainingSettings
{
    public string DurationType { get; set; } = "Time";
    public int DurationValue { get; set; } = 5;

    // Operace
    public bool IncludeAddition { get; set; } = true;
    public bool IncludeSubtraction { get; set; } = true;

    // Nastavení sčítání
    public bool AdditionSecondTwoDigit { get; set; } = false;
    public bool AdditionCarryOver { get; set; } = false;
    public bool AdditionTricks { get; set; } = false;

    // Nastavení odčítání
    public bool SubtractionSecondTwoDigit { get; set; } = false;
    public bool SubtractionCarryOver { get; set; } = false;
    public bool SubtractionTricks { get; set; } = false;

    // Kombinovaná nastavení (jen při obou operacích)
    public bool IncludeParentheses { get; set; } = false;
    public bool IncludeMultipleNumbers { get; set; } = false;

    // Kolik čísel ve výpočtu (výběr z 3–8, výchozí 3,4,5,6)
    public List<int> MultipleNumbersCounts { get; set; } = [3, 4, 5, 6];

    // Mezivýpočty (scratch work) – zobrazí se při závorách nebo více číslech
    public bool EnableScratchWork { get; set; } = false;

    // Čtení příkladu nahlas (Text-to-Speech)
    public bool EnableReadAloud { get; set; } = false;
}

public class MixedMathTrainingSettings
{
    public string DurationType { get; set; } = "Time";
    public int DurationValue { get; set; } = 5;

    // Operace
    public bool IncludeAddition { get; set; } = true;
    public bool IncludeSubtraction { get; set; } = true;
    public bool IncludeMultiplication { get; set; } = true;
    public bool IncludeDivision { get; set; } = true;

    // Pokročilá nastavení
    public bool IncludeTricks { get; set; } = false;
    public bool IncludeParentheses { get; set; } = false;
    public List<int> NumberCounts { get; set; } = [2];
    public bool EnableScratchWork { get; set; } = false;
    public bool EnableReadAloud { get; set; } = false;
}

public class PairedConsonantsTrainingSettings
{
    public string DurationType { get; set; } = "Time";
    public int DurationValue { get; set; } = 5;

    // Vybrané páry souhlásek
    public List<string> SelectedPairs { get; set; } = ["B_P", "D_T", "Z_S", "V_F"];

    // Pozice hlásky: "End" = konec slova, "Middle" = uprostřed, "Both" = obojí
    public string SoundPosition { get; set; } = "Both";

    // Zobrazit tlačítka odpovědí místo textového vstupu
    public bool ShowAnswerButtons { get; set; } = true;

    // Zobrazit tlačítka všech variant (všech vybraných párů)
    public bool ShowAllVariantsButtons { get; set; } = false;

    // Zobrazit celé slovo po správné odpovědi
    public bool ShowFullWordAfterAnswer { get; set; } = true;
}

public interface ISettingsService
{
    Task<TrainingSettings> GetProfileSettingsAsync(int? profileId);
    Task SaveProfileSettingsAsync(int profileId, TrainingSettings settings);
    Task<AddSubTrainingSettings> GetAddSubSettingsAsync(int? profileId);
    Task SaveAddSubSettingsAsync(int profileId, AddSubTrainingSettings settings);
    Task<PairedConsonantsTrainingSettings> GetPairedConsonantsSettingsAsync(int? profileId);
    Task SavePairedConsonantsSettingsAsync(int profileId, PairedConsonantsTrainingSettings settings);
    Task<MixedMathTrainingSettings> GetMixedMathSettingsAsync(int? profileId);
    Task SaveMixedMathSettingsAsync(int profileId, MixedMathTrainingSettings settings);
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

    public async Task<AddSubTrainingSettings> GetAddSubSettingsAsync(int? profileId)
    {
        if (!profileId.HasValue)
            return new AddSubTrainingSettings();

        var userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId);

        if (userSettings?.AddSubSettingsJson == null)
            return new AddSubTrainingSettings();

        return JsonSerializer.Deserialize<AddSubTrainingSettings>(userSettings.AddSubSettingsJson)
               ?? new AddSubTrainingSettings();
    }

    public async Task SaveAddSubSettingsAsync(int profileId, AddSubTrainingSettings settings)
    {
        var userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId);
        if (userSettings == null)
        {
            userSettings = new UserSettings { ProfileId = profileId };
            _dbContext.UserSettings.Add(userSettings);
        }

        userSettings.AddSubSettingsJson = JsonSerializer.Serialize(settings);
        userSettings.LastModified = DateTime.UtcNow;

        _dbContext.UserSettings.Update(userSettings);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PairedConsonantsTrainingSettings> GetPairedConsonantsSettingsAsync(int? profileId)
    {
        if (!profileId.HasValue)
            return new PairedConsonantsTrainingSettings();

        var userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId);

        if (userSettings?.PairedConsonantsSettingsJson == null)
            return new PairedConsonantsTrainingSettings();

        return JsonSerializer.Deserialize<PairedConsonantsTrainingSettings>(userSettings.PairedConsonantsSettingsJson)
               ?? new PairedConsonantsTrainingSettings();
    }

    public async Task SavePairedConsonantsSettingsAsync(int profileId, PairedConsonantsTrainingSettings settings)
    {
        var userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId);
        if (userSettings == null)
        {
            userSettings = new UserSettings { ProfileId = profileId };
            _dbContext.UserSettings.Add(userSettings);
        }

        userSettings.PairedConsonantsSettingsJson = JsonSerializer.Serialize(settings);
        userSettings.LastModified = DateTime.UtcNow;

        _dbContext.UserSettings.Update(userSettings);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<MixedMathTrainingSettings> GetMixedMathSettingsAsync(int? profileId)
    {
        if (!profileId.HasValue)
            return new MixedMathTrainingSettings();

        var userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId);

        if (userSettings?.MixedMathSettingsJson == null)
            return new MixedMathTrainingSettings();

        return JsonSerializer.Deserialize<MixedMathTrainingSettings>(userSettings.MixedMathSettingsJson)
               ?? new MixedMathTrainingSettings();
    }

    public async Task SaveMixedMathSettingsAsync(int profileId, MixedMathTrainingSettings settings)
    {
        var userSettings = await _dbContext.UserSettings.FirstOrDefaultAsync(s => s.ProfileId == profileId);
        if (userSettings == null)
        {
            userSettings = new UserSettings { ProfileId = profileId };
            _dbContext.UserSettings.Add(userSettings);
        }

        userSettings.MixedMathSettingsJson = JsonSerializer.Serialize(settings);
        userSettings.LastModified = DateTime.UtcNow;

        _dbContext.UserSettings.Update(userSettings);
        await _dbContext.SaveChangesAsync();
    }
}
