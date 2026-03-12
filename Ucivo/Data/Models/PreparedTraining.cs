using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TurboSkola.Data.Models;

public class PreparedTraining
{
    [Key]
    public int PreparedTrainingId { get; set; }

    [Required]
    public int CreatedByProfileId { get; set; }

    [Required]
    public int AssignedToProfileId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TrainingTypeCode { get; set; } = string.Empty;

    public string SettingsJson { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled, Archived

    [MaxLength(500)]
    public string? Note { get; set; }

    public DateTime ScheduledDate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? StartedDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    [MaxLength(36)]
    public string? BatchId { get; set; }

    public int SortOrder { get; set; } = 0;

    // Navigation properties
    [ForeignKey(nameof(CreatedByProfileId))]
    public UserProfile CreatedByProfile { get; set; } = null!;

    [ForeignKey(nameof(AssignedToProfileId))]
    public UserProfile AssignedToProfile { get; set; } = null!;
}
