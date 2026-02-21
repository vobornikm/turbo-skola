namespace TurboSkola.Data.Models;

public class SessionExercise
{
    public int ExerciseId { get; set; }
    public int SessionId { get; set; }
    public int ExerciseIndex { get; set; } // Poøadí v tréninku
    public string ExerciseText { get; set; } = string.Empty; // "5 × 7" nebo "35 ÷ 5"
    public int CorrectAnswer { get; set; }
    public int UserAnswer { get; set; }
    public int TimeSpentSeconds { get; set; }
    public int NumberOfAttempts { get; set; }
    public bool IsCorrect { get; set; }
    public bool SkippedByUser { get; set; }

    // Relationships
    public TrainingSession Session { get; set; } = null!;
}
