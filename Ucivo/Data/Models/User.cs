namespace TurboSkola.Data.Models;

public class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime? LastTrainingDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Relationships
    public ICollection<UserProfile> Profiles { get; set; } = new List<UserProfile>();
}
