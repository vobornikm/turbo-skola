namespace TurboSkola.Data.Models;

/// <summary>
/// Spojovací tabulka - jaké pøedmìty se uèí v jakých roènících
/// </summary>
public class SubjectGrade
{
    public int SubjectGradeId { get; set; }
    public int SubjectId { get; set; }
    public int GradeId { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Relationships
    public Subject Subject { get; set; } = null!;
    public Grade Grade { get; set; } = null!;
}
