using TurboSkola.Utilities;
using System.Collections.Concurrent;

namespace TurboSkola.Services;

public class SessionExerciseData
{
    public int Index { get; set; }
    public Exercise Exercise { get; set; } = null!;
    public int? UserAnswer { get; set; }
    public int Attempts { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsSkipped { get; set; }
    public bool GaveUp { get; set; }
    public bool HintShown { get; set; }
    public List<int> WrongAnswers { get; set; } = new();
    public int TimeSpentSeconds { get; set; }
    public DateTime StartTime { get; set; }
}

public class SessionState
{
    public int SessionId { get; set; }
    public int? UserId { get; set; }
    public List<SessionExerciseData> Exercises { get; set; } = new();
    public int CurrentExerciseIndex { get; set; }
    public DateTime SessionStartTime { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsTimeBasedDuration { get; set; }
}

public interface ISessionService
{
    SessionState? GetCurrentSession();
    void SetCurrentSession(SessionState session);
    void ClearCurrentSession();
    TrainingSettings? GetCurrentSettings();
    void SetCurrentSettings(TrainingSettings settings);
    AddSubTrainingSettings? GetCurrentAddSubSettings();
    void SetCurrentAddSubSettings(AddSubTrainingSettings settings);
}

public class SessionService : ISessionService
{
    private SessionState? _lastSession;
    private TrainingSettings? _lastSettings;
    private AddSubTrainingSettings? _lastAddSubSettings;
    private readonly object _lock = new();

    public SessionState? GetCurrentSession()
    {
        lock (_lock)
        {
            return _lastSession;
        }
    }

    public void SetCurrentSession(SessionState session)
    {
        lock (_lock)
        {
            _lastSession = session;
        }
    }

    public void ClearCurrentSession()
    {
        lock (_lock)
        {
            _lastSession = null;
        }
    }

    public TrainingSettings? GetCurrentSettings()
    {
        lock (_lock)
        {
            return _lastSettings;
        }
    }

    public void SetCurrentSettings(TrainingSettings settings)
    {
        lock (_lock)
        {
            _lastSettings = settings;
        }
    }

    public AddSubTrainingSettings? GetCurrentAddSubSettings()
    {
        lock (_lock)
        {
            return _lastAddSubSettings;
        }
    }

    public void SetCurrentAddSubSettings(AddSubTrainingSettings settings)
    {
        lock (_lock)
        {
            _lastAddSubSettings = settings;
        }
    }
}
