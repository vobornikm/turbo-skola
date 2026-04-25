namespace TurboSkola.Utilities;

/// <summary>
/// Jeden segment strukturovaného výrazu.
/// Segment může mít volitelný "scratch slot" – malý input nad textem,
/// kam si dítě může napsat mezivýpočet (jako na papíře).
/// </summary>
public class ExerciseSegment
{
    /// <summary>Text segmentu, např. "(5 + 7)" nebo " + 3"</summary>
    public string Text { get; set; } = "";

    /// <summary>
    /// Očekávaný mezivýsledek. Pokud není null, nad segmentem se zobrazí scratch input.
    /// Slouží k vizuální validaci (zelená/oranžová) při špatné odpovědi.
    /// </summary>
    public int? ScratchValue { get; set; }
}
