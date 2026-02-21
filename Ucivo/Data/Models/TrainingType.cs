namespace TurboSkola.Data.Models;

/// <summary>
/// Typ tréninku (Malá násobilka, Sèítání do 20, Slovní druhy, atd.)
/// </summary>
public class TrainingType
{
    public int TrainingTypeId { get; set; }
    public int SubjectId { get; set; }
    public string Name { get; set; } = string.Empty; // "Malá násobilka", "Sèítání do 20"
    public string Code { get; set; } = string.Empty; // "SMALL_MULTIPLICATION", "ADDITION_20"
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty; // "??", "?"
    
    /// <summary>
    /// Plnì kvalifikovaný název tøídy generátoru (napø. "TurboSkola.TrainingGenerators.MultiplicationGenerator")
    /// </summary>
    public string GeneratorClassName { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON konfigurace generátoru (specifické nastavení pro každý typ)
    /// </summary>
    public string? ConfigurationJson { get; set; }
    
    public int MinGradeNumber { get; set; } // Od kterého roèníku
    public int MaxGradeNumber { get; set; } // Do kterého roèníku
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Relationships
    public Subject Subject { get; set; } = null!;
}
