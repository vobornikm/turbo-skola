namespace TurboSkola.Data.Models;

/// <summary>
/// Roèník (1. tøída, 2. tøída, atd.)
/// </summary>
public class Grade
{
    public int GradeId { get; set; }
    public int SchoolLevelId { get; set; }
    public string Name { get; set; } = string.Empty; // "1. tøída", "2. tøída"
    public string Code { get; set; } = string.Empty; // "GRADE_1", "GRADE_2"
    public int GradeNumber { get; set; } // 1, 2, 3...
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Relationships
    public SchoolLevel SchoolLevel { get; set; } = null!;
}
