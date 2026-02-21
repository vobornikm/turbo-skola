namespace TurboSkola.Data.Models;

/// <summary>
/// Pøedmìt (Matematika, Èeský jazyk, atd.)
/// </summary>
public class Subject
{
    public int SubjectId { get; set; }
    public string Name { get; set; } = string.Empty; // "Matematika", "Èeský jazyk"
    public string Code { get; set; } = string.Empty; // "MATH", "CZECH"
    public string Icon { get; set; } = string.Empty; // "??", "??"
    public string Color { get; set; } = "#667eea"; // Barva pøedmìtu
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Relationships
    public ICollection<SubjectGrade> SubjectGrades { get; set; } = new List<SubjectGrade>();
    public ICollection<TrainingType> TrainingTypes { get; set; } = new List<TrainingType>();
}
