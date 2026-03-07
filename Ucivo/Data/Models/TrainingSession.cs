namespace TurboSkola.Data.Models;

public class TrainingSession
{
    public int SessionId { get; set; }
    public int? ProfileId { get; set; }
    public string? AnonymousId { get; set; } // Pro neregistrované uživatele
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationSeconds { get; set; }
    public int TotalExamples { get; set; }
    public int CorrectFirstAttempt { get; set; }
    public int TotalAttempts { get; set; }
    public int SkippedExamples { get; set; }
    public string? TrainingTypeCode { get; set; } // "SMALL_MULTIPLICATION", "ADD_SUB_100", ...

    // Relationships
    public UserProfile? Profile { get; set; }
    public ICollection<SessionExercise> Exercises { get; set; } = new List<SessionExercise>();
}
