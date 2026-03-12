using Microsoft.EntityFrameworkCore;
using TurboSkola.Data;
using TurboSkola.Data.Models;

namespace TurboSkola.Services;

public interface IPreparedTrainingService
{
    Task<List<PreparedTraining>> CreatePreparedTrainingsAsync(
        int createdByProfileId,
        int assignedToProfileId,
        string trainingTypeCode,
        string settingsJson,
        int repeatCount,
        bool isRecurring,
        int recurringDays,
        string? note);

    Task<List<PreparedTraining>> GetPendingForProfileAsync(int profileId);
    Task<int> GetPendingCountForProfileAsync(int profileId);
    Task<List<PreparedTraining>> GetCreatedByProfileAsync(int profileId, bool includeArchived = false);
    Task<PreparedTraining?> GetByIdAsync(int preparedTrainingId);
    Task MarkAsStartedAsync(int preparedTrainingId);
    Task MarkAsCompletedAsync(int preparedTrainingId);
    Task CancelAsync(int preparedTrainingId);
    Task CancelBatchAsync(string batchId);
    Task DeleteBatchAsync(string batchId);
    Task ResetInProgressBatchAsync(string batchId);
    Task ArchiveBatchAsync(string batchId);
    Task AutoCompleteInProgressAsync(int profileId);
    Task<List<UserProfile>> GetChildProfilesForUserAsync(int userId);
    Task SaveBatchOrderAsync(int createdByProfileId, int assignedToProfileId, List<string> batchIdsInOrder);
}

public class PreparedTrainingService : IPreparedTrainingService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PreparedTrainingService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<PreparedTraining>> CreatePreparedTrainingsAsync(
        int createdByProfileId,
        int assignedToProfileId,
        string trainingTypeCode,
        string settingsJson,
        int repeatCount,
        bool isRecurring,
        int recurringDays,
        string? note)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var batchId = Guid.NewGuid().ToString("N")[..12];
        var trainings = new List<PreparedTraining>();
        var today = DateTime.UtcNow.Date;

        if (isRecurring && recurringDays > 0)
        {
            for (int day = 0; day < recurringDays; day++)
            {
                var scheduledDate = today.AddDays(day);
                for (int i = 0; i < repeatCount; i++)
                {
                    trainings.Add(new PreparedTraining
                    {
                        CreatedByProfileId = createdByProfileId,
                        AssignedToProfileId = assignedToProfileId,
                        TrainingTypeCode = trainingTypeCode,
                        SettingsJson = settingsJson,
                        Status = "Pending",
                        Note = note,
                        ScheduledDate = scheduledDate,
                        CreatedDate = DateTime.UtcNow,
                        BatchId = batchId
                    });
                }
            }
        }
        else
        {
            for (int i = 0; i < repeatCount; i++)
            {
                trainings.Add(new PreparedTraining
                {
                    CreatedByProfileId = createdByProfileId,
                    AssignedToProfileId = assignedToProfileId,
                    TrainingTypeCode = trainingTypeCode,
                    SettingsJson = settingsJson,
                    Status = "Pending",
                    Note = note,
                    ScheduledDate = today,
                    CreatedDate = DateTime.UtcNow,
                    BatchId = batchId
                });
            }
        }

        context.PreparedTrainings.AddRange(trainings);
        await context.SaveChangesAsync();
        return trainings;
    }

    public async Task<List<PreparedTraining>> GetPendingForProfileAsync(int profileId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var today = DateTime.UtcNow.Date;

        return await context.PreparedTrainings
            .Include(pt => pt.CreatedByProfile)
            .Where(pt => pt.AssignedToProfileId == profileId
                && (pt.Status == "Pending" || pt.Status == "InProgress")
                && pt.ScheduledDate <= today)
            .OrderBy(pt => pt.ScheduledDate)
            .ThenBy(pt => pt.CreatedDate)
            .ToListAsync();
    }

    public async Task<int> GetPendingCountForProfileAsync(int profileId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var today = DateTime.UtcNow.Date;

        return await context.PreparedTrainings
            .CountAsync(pt => pt.AssignedToProfileId == profileId
                && (pt.Status == "Pending" || pt.Status == "InProgress")
                && pt.ScheduledDate <= today);
    }

    public async Task<List<PreparedTraining>> GetCreatedByProfileAsync(int profileId, bool includeArchived = false)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var query = context.PreparedTrainings
            .Include(pt => pt.AssignedToProfile)
            .Where(pt => pt.CreatedByProfileId == profileId);

        if (!includeArchived)
        {
            query = query.Where(pt => pt.Status != "Archived");
        }

        return await query
            .OrderBy(pt => pt.SortOrder)
            .ThenByDescending(pt => pt.CreatedDate)
            .ToListAsync();
    }

    public async Task<PreparedTraining?> GetByIdAsync(int preparedTrainingId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        return await context.PreparedTrainings
            .Include(pt => pt.CreatedByProfile)
            .FirstOrDefaultAsync(pt => pt.PreparedTrainingId == preparedTrainingId);
    }

    public async Task MarkAsStartedAsync(int preparedTrainingId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var training = await context.PreparedTrainings.FindAsync(preparedTrainingId);
        if (training != null && training.Status == "Pending")
        {
            training.Status = "InProgress";
            training.StartedDate = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task MarkAsCompletedAsync(int preparedTrainingId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var training = await context.PreparedTrainings.FindAsync(preparedTrainingId);
        if (training != null && (training.Status == "InProgress" || training.Status == "Pending"))
        {
            training.Status = "Completed";
            training.CompletedDate = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task CancelAsync(int preparedTrainingId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var training = await context.PreparedTrainings.FindAsync(preparedTrainingId);
        if (training != null && training.Status == "Pending")
        {
            training.Status = "Cancelled";
            await context.SaveChangesAsync();
        }
    }

    public async Task CancelBatchAsync(string batchId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var trainings = await context.PreparedTrainings
            .Where(pt => pt.BatchId == batchId && pt.Status == "Pending")
            .ToListAsync();

        foreach (var training in trainings)
        {
            training.Status = "Cancelled";
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteBatchAsync(string batchId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var trainings = await context.PreparedTrainings
            .Where(pt => pt.BatchId == batchId)
            .ToListAsync();

        context.PreparedTrainings.RemoveRange(trainings);
        await context.SaveChangesAsync();
    }

    public async Task ResetInProgressBatchAsync(string batchId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var trainings = await context.PreparedTrainings
            .Where(pt => pt.BatchId == batchId && pt.Status == "InProgress")
            .ToListAsync();

        foreach (var training in trainings)
        {
            training.Status = "Pending";
            training.StartedDate = null;
        }

        if (trainings.Count > 0)
        {
            await context.SaveChangesAsync();
        }
    }

    public async Task AutoCompleteInProgressAsync(int profileId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var inProgress = await context.PreparedTrainings
            .Where(pt => pt.AssignedToProfileId == profileId && pt.Status == "InProgress")
            .ToListAsync();

        foreach (var pt in inProgress)
        {
            var hasCompletedSession = await context.TrainingSessions
                .AnyAsync(ts => ts.ProfileId == profileId
                    && ts.TrainingTypeCode == pt.TrainingTypeCode
                    && ts.StartTime >= pt.StartedDate
                    && ts.DurationSeconds > 0
                    && ts.TotalExamples > 0);

            if (hasCompletedSession)
            {
                pt.Status = "Completed";
                pt.CompletedDate = DateTime.UtcNow;
            }
        }

        if (inProgress.Any())
        {
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<UserProfile>> GetChildProfilesForUserAsync(int userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        return await context.UserProfiles
            .Where(p => p.UserId == userId && p.IsActive && !p.IsParentProfile)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.ProfileName)
            .ToListAsync();
    }

    public async Task ArchiveBatchAsync(string batchId)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var trainings = await context.PreparedTrainings
            .Where(pt => pt.BatchId == batchId && pt.Status == "Completed")
            .ToListAsync();

        foreach (var training in trainings)
        {
            training.Status = "Archived";
        }

        if (trainings.Count > 0)
        {
            await context.SaveChangesAsync();
        }
    }

    public async Task SaveBatchOrderAsync(int createdByProfileId, int assignedToProfileId, List<string> batchIdsInOrder)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UcivoDbContext>();

        var trainings = await context.PreparedTrainings
            .Where(pt => pt.CreatedByProfileId == createdByProfileId
                && pt.AssignedToProfileId == assignedToProfileId)
            .ToListAsync();

        for (int i = 0; i < batchIdsInOrder.Count; i++)
        {
            var batchId = batchIdsInOrder[i];
            foreach (var t in trainings.Where(t => (t.BatchId ?? t.PreparedTrainingId.ToString()) == batchId))
            {
                t.SortOrder = i + 1;
            }
        }

        await context.SaveChangesAsync();
    }
}
