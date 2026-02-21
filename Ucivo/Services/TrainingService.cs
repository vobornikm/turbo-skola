using TurboSkola.Data;
using TurboSkola.Data.Models;
using TurboSkola.Utilities;

namespace TurboSkola.Services;

public interface ITrainingService
{
    Task<TrainingSession> CreateSessionAsync(int? profileId, List<int> multipliers, bool includeMultiplication, bool includeDivision, int durationSeconds, int? exerciseCount);
    Task CompleteSessionAsync(int sessionId, int correctFirstAttempt, int totalAttempts, List<SessionExercise> exercises);
    Task<TrainingSession?> GetSessionAsync(int sessionId);
}

public class TrainingService : ITrainingService
{
    private readonly UcivoDbContext _dbContext;
    private readonly ExerciseGenerator _exerciseGenerator;

    public TrainingService(UcivoDbContext dbContext, ExerciseGenerator exerciseGenerator)
    {
        _dbContext = dbContext;
        _exerciseGenerator = exerciseGenerator;
    }

    public async Task<TrainingSession> CreateSessionAsync(
        int? profileId,
        List<int> multipliers,
        bool includeMultiplication,
        bool includeDivision,
        int durationSeconds,
        int? exerciseCount)
    {
        var session = new TrainingSession
        {
            ProfileId = profileId,
            AnonymousId = profileId == null ? Guid.NewGuid().ToString() : null,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddSeconds(durationSeconds),
            TotalExamples = exerciseCount ?? (durationSeconds / 30),
            CorrectFirstAttempt = 0,
            TotalAttempts = 0,
            SkippedExamples = 0
        };

        _dbContext.TrainingSessions.Add(session);
        await _dbContext.SaveChangesAsync();

        return session;
    }

    public async Task CompleteSessionAsync(int sessionId, int correctFirstAttempt, int totalAttempts, List<SessionExercise> exercises)
    {
        var session = await _dbContext.TrainingSessions.FindAsync(sessionId);
        if (session == null) return;

        session.EndTime = DateTime.UtcNow;
        session.DurationSeconds = (int)(session.EndTime - session.StartTime).TotalSeconds;
        session.CorrectFirstAttempt = correctFirstAttempt;
        session.TotalAttempts = totalAttempts;
        session.SkippedExamples = exercises.Count(e => e.SkippedByUser);
        session.TotalExamples = exercises.Count(e => !e.SkippedByUser);

        _dbContext.TrainingSessions.Update(session);
        
        // Batch save všech exercises najednou
        _dbContext.SessionExercises.AddRange(exercises);
        
        // Aktualizace LastTrainingDate u uživatele pøes profil
        if (session.ProfileId.HasValue)
        {
            var profile = await _dbContext.UserProfiles.FindAsync(session.ProfileId.Value);
            if (profile != null)
            {
                var user = await _dbContext.Users.FindAsync(profile.UserId);
                if (user != null)
                {
                    user.LastTrainingDate = DateTime.UtcNow;
                    _dbContext.Users.Update(user);
                }
            }
        }

        // Jeden SaveChanges pro všechno
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TrainingSession?> GetSessionAsync(int sessionId)
    {
        return await _dbContext.TrainingSessions.FindAsync(sessionId);
    }
}
