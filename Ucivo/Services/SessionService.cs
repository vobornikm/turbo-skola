using TurboSkola.Utilities;
using TurboSkola.TrainingGenerators;
using System.Collections.Concurrent;

namespace TurboSkola.Services;

public class CzechSessionExerciseData
{
    public int Index { get; set; }
    public CzechExercise Exercise { get; set; } = null!;
    public string? UserAnswer { get; set; }
    public int Attempts { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsSkipped { get; set; }
    public bool GaveUp { get; set; }
    public bool HintShown { get; set; }
    public List<string> WrongAnswers { get; set; } = new();
    public int TimeSpentSeconds { get; set; }
    public DateTime StartTime { get; set; }
}

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
    PairedConsonantsTrainingSettings? GetCurrentPairedConsonantsSettings();
    void SetCurrentPairedConsonantsSettings(PairedConsonantsTrainingSettings settings);
    MixedMathTrainingSettings? GetCurrentMixedMathSettings();
    void SetCurrentMixedMathSettings(MixedMathTrainingSettings settings);
    MultiplyDivideBy2Settings? GetCurrentMultiplyDivideBy2Settings();
    void SetCurrentMultiplyDivideBy2Settings(MultiplyDivideBy2Settings settings);
    int? ActivePreparedTrainingId { get; }
    void SetActivePreparedTraining(int id);
    void ClearActivePreparedTraining();
}

public class SessionService : ISessionService
{
    private SessionState? _lastSession;
    private TrainingSettings? _lastSettings;
    private AddSubTrainingSettings? _lastAddSubSettings;
    private PairedConsonantsTrainingSettings? _lastPairedConsonantsSettings;
    private MixedMathTrainingSettings? _lastMixedMathSettings;
    private MultiplyDivideBy2Settings? _lastMultiplyDivideBy2Settings;
    private int? _activePreparedTrainingId;
    private readonly object _lock = new();

    public int? ActivePreparedTrainingId
    {
        get { lock (_lock) { return _activePreparedTrainingId; } }
    }

    public void SetActivePreparedTraining(int id)
    {
        lock (_lock) { _activePreparedTrainingId = id; }
    }

    public void ClearActivePreparedTraining()
    {
        lock (_lock) { _activePreparedTrainingId = null; }
    }

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

    public PairedConsonantsTrainingSettings? GetCurrentPairedConsonantsSettings()
    {
        lock (_lock)
        {
            return _lastPairedConsonantsSettings;
        }
    }

    public void SetCurrentPairedConsonantsSettings(PairedConsonantsTrainingSettings settings)
    {
        lock (_lock)
        {
            _lastPairedConsonantsSettings = settings;
        }
    }

    public MixedMathTrainingSettings? GetCurrentMixedMathSettings()
    {
        lock (_lock)
        {
            return _lastMixedMathSettings;
        }
    }

    public void SetCurrentMixedMathSettings(MixedMathTrainingSettings settings)
    {
        lock (_lock)
        {
            _lastMixedMathSettings = settings;
        }
    }

    public MultiplyDivideBy2Settings? GetCurrentMultiplyDivideBy2Settings()
    {
        lock (_lock)
        {
            return _lastMultiplyDivideBy2Settings;
        }
    }

    public void SetCurrentMultiplyDivideBy2Settings(MultiplyDivideBy2Settings settings)
    {
        lock (_lock)
        {
            _lastMultiplyDivideBy2Settings = settings;
        }
    }
}
