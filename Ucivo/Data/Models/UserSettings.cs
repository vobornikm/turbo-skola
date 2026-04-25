namespace TurboSkola.Data.Models;

public class UserSettings
{
    public int SettingId { get; set; }
    public int ProfileId { get; set; }
    public string DurationType { get; set; } = "Time"; // "Time" nebo "Count"
    public int DurationValue { get; set; } = 10; // minuty nebo poèet pøíkladù
    public string IncludedMultipliers { get; set; } = "[0,1,2,3,4,5,6,7,8,9,10]"; // JSON pole
    public bool IncludeMultiplication { get; set; } = true;
    public bool IncludeDivision { get; set; } = false;
    public string? AddSubSettingsJson { get; set; }
    public string? PairedConsonantsSettingsJson { get; set; }
    public string? MixedMathSettingsJson { get; set; }
    public DateTime LastModified { get; set; }

    // Relationships
    public UserProfile Profile { get; set; } = null!;
}
