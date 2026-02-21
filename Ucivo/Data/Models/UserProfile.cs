using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TurboSkola.Data.Models;

public class UserProfile
{
    [Key]
    public int ProfileId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ProfileName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Nová pole pro rodièovskou kontrolu
    public bool IsParentProfile { get; set; } = false;

    [MaxLength(4)]
    public string? PinCode { get; set; } // 4 èíslice jako string, null = žádný PIN

    public int DisplayOrder { get; set; } = 0;

    [MaxLength(7)]
    public string AvatarColor { get; set; } = "#667eea"; // Hex barva

    [MaxLength(10)]
    public string AvatarIcon { get; set; } = "child"; // Ikona: child, parent, fox, bear, cat, dog, rocket, ball, music, game

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public ICollection<UserSettings> Settings { get; set; } = new List<UserSettings>();
    public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
}

