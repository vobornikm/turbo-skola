namespace TurboSkola.Data.Models;

/// <summary>
/// Slovo zablokované rodičem – nebude se nadále zobrazovat v trénincích.
/// </summary>
public class BlockedWord
{
    public int BlockedWordId { get; set; }
    public string FullWord { get; set; } = string.Empty;
    public string TrainingTypeCode { get; set; } = string.Empty; // "PAIRED_CONSONANTS"
    public DateTime BlockedAt { get; set; }
    public string? BlockedBy { get; set; } // Jméno profilu rodiče
}
