using Microsoft.EntityFrameworkCore;
using TurboSkola.Data.Models;

namespace TurboSkola.Data;

public class UcivoDbContext : DbContext
{
    public UcivoDbContext(DbContextOptions<UcivoDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<TrainingSession> TrainingSessions { get; set; }
    public DbSet<SessionExercise> SessionExercises { get; set; }
    
    // New training catalog entities
    public DbSet<SchoolLevel> SchoolLevels { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<SubjectGrade> SubjectGrades { get; set; }
    public DbSet<TrainingType> TrainingTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasMany(e => e.Profiles)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserProfile configuration
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId);
            entity.Property(e => e.ProfileName).HasMaxLength(100).IsRequired();
            
            entity.HasMany(e => e.Settings)
                .WithOne(s => s.Profile)
                .HasForeignKey(s => s.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.TrainingSessions)
                .WithOne(t => t.Profile)
                .HasForeignKey(t => t.ProfileId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // UserSettings configuration
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e.SettingId);
            entity.Property(e => e.DurationType).HasMaxLength(10);
            entity.Property(e => e.IncludedMultipliers).IsRequired();
        });

        // TrainingSession configuration
        modelBuilder.Entity<TrainingSession>(entity =>
        {
            entity.HasKey(e => e.SessionId);
            entity.Property(e => e.AnonymousId).HasMaxLength(36);

            entity.HasMany(e => e.Exercises)
                .WithOne(ex => ex.Session)
                .HasForeignKey(ex => ex.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // SessionExercise configuration
        modelBuilder.Entity<SessionExercise>(entity =>
        {
            entity.HasKey(e => e.ExerciseId);
            entity.Property(e => e.ExerciseText).HasMaxLength(50);
        });

        // SchoolLevel configuration
        modelBuilder.Entity<SchoolLevel>(entity =>
        {
            entity.HasKey(e => e.SchoolLevelId);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Grade configuration
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
            
            entity.HasOne(e => e.SchoolLevel)
                .WithMany(s => s.Grades)
                .HasForeignKey(e => e.SchoolLevelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Subject configuration
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Icon).HasMaxLength(10);
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // SubjectGrade configuration
        modelBuilder.Entity<SubjectGrade>(entity =>
        {
            entity.HasKey(e => e.SubjectGradeId);
            
            entity.HasOne(e => e.Subject)
                .WithMany(s => s.SubjectGrades)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Grade)
                .WithMany()
                .HasForeignKey(e => e.GradeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => new { e.SubjectId, e.GradeId }).IsUnique();
        });

        // TrainingType configuration
        modelBuilder.Entity<TrainingType>(entity =>
        {
            entity.HasKey(e => e.TrainingTypeId);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(10);
            entity.Property(e => e.GeneratorClassName).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
            
            entity.HasOne(e => e.Subject)
                .WithMany(s => s.TrainingTypes)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
