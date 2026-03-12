using TurboSkola.Services;

namespace TurboSkola.TrainingGenerators;

/// <summary>
/// Cvičení pro trénink párových souhlásek – slovo s vynechaným písmenem.
/// </summary>
public class CzechExercise
{
    public string Pair { get; set; } = "";           // "B_P"
    public string FullWord { get; set; } = "";       // "chléb"
    public string WordWithBlank { get; set; } = "";  // "chle_"
    public string CorrectLetter { get; set; } = "";  // "b"
    public string PairOption1 { get; set; } = "";    // "b" (první z páru – znělá)
    public string PairOption2 { get; set; } = "";    // "p" (druhá z páru – neznělá)
    public string Position { get; set; } = "";       // "End" / "Middle"

    /// <summary>
    /// Všechny přijatelné odpovědi. Obvykle jen CorrectLetter, ale u nejednoznačných
    /// slov (zeď/zeť, led/let, choď/choť, plod/plot) obě písmena z páru.
    /// </summary>
    public HashSet<string> AcceptedAnswers { get; set; } = [];

    /// <summary>
    /// Mapování odpovědi → celé slovo (pro zobrazení po správné odpovědi).
    /// Např. "d" → "led", "t" → "let"
    /// </summary>
    public Dictionary<string, string> AnswerToFullWord { get; set; } = [];

    /// <summary>
    /// Je toto slovo nejednoznačné? (obě písmena tvoří platné slovo)
    /// </summary>
    public bool IsAmbiguous => AcceptedAnswers.Count > 1;
}

/// <summary>
/// Generátor cvičení pro párové souhlásky (2. třída ZŠ).
/// Vybírá náhodná slova z banky podle nastavení.
/// </summary>
public class PairedConsonantsGenerator
{
    private readonly Random _random = new();
    private readonly PairedConsonantsTrainingSettings _settings;
    private readonly List<PcWord> _filteredWords;
    private readonly HashSet<int> _usedIndices = [];

    public PairedConsonantsGenerator(PairedConsonantsTrainingSettings settings, HashSet<string>? blockedWords = null)
    {
        _settings = settings;

        // Filtrovat slova podle nastavení
        _filteredWords = PairedConsonantsWordBank.AllWords
            .Where(w => _settings.SelectedPairs.Contains(w.Pair))
            .Where(w => _settings.SoundPosition switch
            {
                "End" => w.Position == "End",
                "Middle" => w.Position == "Middle",
                _ => true // "Both"
            })
            .Where(w => blockedWords == null || !blockedWords.Contains(w.FullWord.ToLower()))
            .ToList();
    }

    /// <summary>
    /// Vygeneruje jedno cvičení – slovo s vynechaným písmenem.
    /// </summary>
    public CzechExercise GenerateExercise()
    {
        if (_filteredWords.Count == 0)
            return FallbackExercise();

        // Pokud jsme vyčerpali všechna slova, resetujeme
        if (_usedIndices.Count >= _filteredWords.Count)
            _usedIndices.Clear();

        // Vybíráme dosud nepoužité slovo
        int idx;
        int attempts = 0;
        do
        {
            idx = _random.Next(_filteredWords.Count);
            attempts++;
        }
        while (_usedIndices.Contains(idx) && attempts < 100);

        _usedIndices.Add(idx);
        var word = _filteredWords[idx];

        var parts = word.Pair.Split('_');
        var exercise = new CzechExercise
        {
            Pair = word.Pair,
            FullWord = word.FullWord,
            WordWithBlank = word.WordWithBlank,
            CorrectLetter = word.CorrectLetter,
            PairOption1 = parts[0].ToLower(),
            PairOption2 = parts[1].ToLower(),
            Position = word.Position
        };

        // Zkontroluj nejednoznačnost (obě písmena tvoří platné slovo)
        var key = (word.WordWithBlank, word.Pair);
        if (PairedConsonantsWordBank.AmbiguousBlanks.TryGetValue(key, out var ambiguous))
        {
            exercise.AcceptedAnswers = ambiguous.AllCorrectLetters;
            exercise.AnswerToFullWord = new Dictionary<string, string>(ambiguous.LetterToFullWord);
        }
        else
        {
            exercise.AcceptedAnswers = [word.CorrectLetter];
            exercise.AnswerToFullWord = new Dictionary<string, string> { { word.CorrectLetter, word.FullWord } };
        }

        return exercise;
    }

    /// <summary>
    /// Vrátí všechna možná tlačítka odpovědí na základě nastavení.
    /// Pokud ShowAllVariantsButtons=true, vrátí písmena ze VŠECH vybraných párů.
    /// Jinak vrátí pouze dvě písmena aktuálního páru.
    /// </summary>
    public List<string> GetAnswerOptions(CzechExercise exercise)
    {
        if (_settings.ShowAllVariantsButtons && _settings.SelectedPairs.Count > 1)
        {
            // Všechna písmena ze všech vybraných párů
            var options = new HashSet<string>();
            foreach (var pair in _settings.SelectedPairs)
            {
                var parts = pair.Split('_');
                options.Add(parts[0].ToLower());
                options.Add(parts[1].ToLower());
            }
            return options.OrderBy(_ => _random.Next()).ToList();
        }
        else
        {
            // Jen dvě varianty aktuálního páru (náhodné pořadí)
            var options = new List<string> { exercise.PairOption1, exercise.PairOption2 };
            if (_random.Next(2) == 0)
                options.Reverse();
            return options;
        }
    }

    /// <summary>
    /// Počet dostupných slov pro aktuální nastavení.
    /// </summary>
    public int AvailableWordCount => _filteredWords.Count;

    private CzechExercise FallbackExercise()
    {
        return new CzechExercise
        {
            Pair = "B_P",
            FullWord = "dub",
            WordWithBlank = "du_",
            CorrectLetter = "b",
            PairOption1 = "b",
            PairOption2 = "p",
            Position = "End",
            AcceptedAnswers = ["b"],
            AnswerToFullWord = new Dictionary<string, string> { { "b", "dub" } }
        };
    }
}
