namespace TurboSkola.Data.Models;

/// <summary>
/// Stupeò školy (základní, støední, atd.)
/// </summary>
public class SchoolLevel
{
    public int SchoolLevelId { get; set; }
    public string Name { get; set; } = string.Empty; // "1. stupeò ZŠ", "2. stupeò ZŠ", "Støední škola"
    public string Code { get; set; } = string.Empty; // "PRIMARY_1", "PRIMARY_2", "SECONDARY"
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Relationships
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
