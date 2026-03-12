namespace TurboSkola.TrainingGenerators;

/// <summary>
/// Banka slov pro trénink párových souhlásek (2. třída ZŠ).
/// Každé slovo obsahuje informaci o páru, správném písmenu, pozici a zobrazení.
/// </summary>
public static class PairedConsonantsWordBank
{
    public static readonly List<PcWord> AllWords = BuildWordList();

    /// <summary>
    /// Slovník nejednoznačných slov: (WordWithBlank, Pair) → info o všech platných odpovědích.
    /// Automaticky detekováno + ruční doplnění pro slova, kde protějšek není v bance
    /// ale existuje jako platné české slovo (zeď/zeť, plod/plot…).
    /// </summary>
    public static readonly Dictionary<(string WordWithBlank, string Pair), AmbiguousEntry> AmbiguousBlanks
        = BuildAmbiguousBlanks();

    private static List<PcWord> BuildWordList()
    {
        var words = new List<PcWord>();

        // ═══════════════════════════════════════════════════════════
        // B / P   (znělá / neznělá)
        // ═══════════════════════════════════════════════════════════
        AddPair(words, "B_P",
            endFirst: [
                "dub", "chléb", "zub", "hrob", "krab", "bob", "klub", "hřib",
                "holub", "slib", "žlab", "srub", "rub", "snob", "šroub",
                "hrb", "vrub", "sob", "záhyb"
            ],
            endSecond: [
                "vstup", "výstup", "chlap", "šíp", "strop", "kop", "typ",
                "snop", "klip", "sklep", "čáp", "step", "cep", "zástup",
                "příkop", "přístup", "nástup", "sup", "nákup", "výkop",
                "poklop", "chlup", "střep"
            ],
            midFirst: [
                "kabát", "rybník", "oblak", "oblek", "kobyla", "sobota",
                "kobliha", "oběd", "obchod", "žebřík", "hubený",
                "jabloň", "zobák", "dubový", "chlebík",
                "bublanina", "rybář", "rubrika", "bubák",
                "obálka", "obrázek", "robot", "obora", "hříbek",
                "obličej", "zabírat", "obyvatel"
            ],
            midSecond: [
                "kapsa", "lopata", "kopec", "kapka", "šipka", "čepice",
                "kapuce", "úplněk", "opona", "kopřiva", "topinka", "lopatka",
                "napsat", "doprava", "skupina", "tupý",
                "kupovat", "slepice", "lopuch", "popel",
                "teplo", "opička", "polévka", "kapesník", "kopačky", "lupínek"
            ]);

        // ═══════════════════════════════════════════════════════════
        // V / F   (znělá / neznělá)
        // ═══════════════════════════════════════════════════════════
        AddPair(words, "V_F",
            endFirst: [
                "dav", "zpěv", "krev", "stav", "mrav", "ostrov", "hřbitov",
                "motiv", "detektiv", "přeliv", "odliv", "záliv",
                "Václav", "výlov", "úlov",
                "kov", "hněv", "úsměv", "splav", "nápěv", "průliv"
            ],
            endSecond: [
                "šéf", "tarif", "kartograf", "paragraf",
                "fotograf", "autograf", "telegraf",
                "golf", "triumf", "blaf"
            ],
            midFirst: [
                "lavička", "ovce", "ovoce", "svíčka", "květ", "dvůr",
                "hlava", "kovář", "levný", "sova", "dívka", "odvaha",
                "povídka", "havran", "návštěva", "živý", "slavný",
                "chvála", "tvrdý", "tvář", "provaz", "červený",
                "kravata", "divadlo", "lavina", "závora", "stavba", "pavučina"
            ],
            midSecond: [
                "kufr", "ofina", "bufet", "kafr", "infekce", "reforma",
                "profesor", "defekt", "konfekt", "referát", "nafta",
                "šafrán", "telefon", "delfín", "konflikt",
                "trafika", "žirafa", "semafor", "kufřík"
            ]);

        // ═══════════════════════════════════════════════════════════
        // D / T   (znělá / neznělá)
        // ═══════════════════════════════════════════════════════════
        AddPair(words, "D_T",
            endFirst: [
                "had", "med", "led", "sad", "hrad", "rod", "plod", "lid",
                "hlad", "západ", "soud", "záchod", "pohled", "dopad",
                "výpad", "chod", "závod", "východ", "úvod",
                "svod", "důvod", "porod", "národ",
                "brod", "smrad", "obklad", "nápad", "odchod", "příchod",
                "přívod", "rozvod", "výhled"
            ],
            endSecond: [
                "let", "svět", "list", "most", "host", "prst", "mast",
                "kost", "past", "mat", "kat", "květ", "záchvat", "výlet",
                "čert", "dort", "smrt", "vlast", "radost", "kout",
                "slet", "plast", "hrot", "ret", "plot",
                "pot", "šrot", "nehet", "plat", "balet", "přílet"
            ],
            midFirst: [
                "ledový", "vodopád", "hodiny", "záhada", "vadný", "hadice",
                "hladký", "medvěd", "sadový", "hradní", "lidový",
                "chodník", "odpad", "soudce", "lednice", "pádlo",
                "podlaha", "nádoba", "radnice", "studený", "podzim", "vedro",
                "budík", "hádanka", "vidlička", "budova", "odměna"
            ],
            midSecond: [
                "letadlo", "motorka", "ostrov", "hotový",
                "listový", "kostka", "městečko", "květina", "botička",
                "matka", "pletivo", "potrava", "světlo",
                "katedra", "kotník", "potok", "čtvrtka",
                "motýl", "batoh", "pytlík", "kulatý", "kytka", "metla", "kotě"
            ]);

        // ═══════════════════════════════════════════════════════════
        // Ď / Ť   (znělá / neznělá)
        // ═══════════════════════════════════════════════════════════
        AddPair(words, "Ď_Ť",
            endFirst: [
                "buď", "teď", "hoď", "poď", "seď", "viď", "záď",
                "hleď", "veď", "jeď", "klaď", "hlaď", "odpověď", "zeď", "choď",
                "hruď", "zpověď", "výpověď"
            ],
            endSecond: [
                "pouť", "paměť", "huť", "choť", "pusť",
                "nechť", "zeť", "suť",
                "chuť", "labuť"
            ],
            midFirst: [
                "ďábel", "loďka", "meďák", "loďstvo"
            ],
            midSecond: [
                "ťukat", "ťapkat", "koťátko", "ťukání",
                "šťastný", "ťapka",
                "šťáva", "šťavnatý"
            ]);

        // ═══════════════════════════════════════════════════════════
        // Z / S   (znělá / neznělá)
        // ═══════════════════════════════════════════════════════════
        AddPair(words, "Z_S",
            endFirst: [
                "jez", "mráz", "rez", "kaz", "vaz", "ráz", "obraz",
                "výraz", "úkaz", "provaz", "plaz", "řez", "vítěz",
                "kněz", "odkaz", "rozkaz", "hráz", "průkaz",
                "řetěz", "svaz", "podraz", "přívoz", "příkaz", "poukaz", "výkaz"
            ],
            endSecond: [
                "les", "nos", "pes", "kos", "klas", "pás", "vlas", "ples",
                "kus", "čas", "hlas", "lis", "mys", "oves",
                "nápis", "zápis", "výpis", "převis", "útes",
                "kompas", "ananas", "autobus", "cirkus", "globus", "kaktus", "losos",
                "věhlas"
            ],
            midFirst: [
                "jezero", "rezavý", "jazyk", "vazba", "obrazárna",
                "mrazák", "kazit", "brzda", "mazat",
                "poznat", "kozačky",
                "razítko", "lezení", "bazén", "gazela",
                "rozum", "pozor", "muzeum", "kozlík", "různý", "nazývat", "železo"
            ],
            midSecond: [
                "hasič", "máslo", "jasný", "osoba", "kostel", "nosit",
                "pasáček", "vesnice", "cesta", "místnost",
                "nástup", "prostor", "hospoda", "masový", "osud",
                "ústav", "pustina", "luskat", "bystřina",
                "pastelka", "kastrol", "prsten", "veslo", "poslat", "kreslit"
            ]);

        // ═══════════════════════════════════════════════════════════
        // Ž / Š   (znělá / neznělá)
        // ═══════════════════════════════════════════════════════════
        AddPair(words, "Ž_Š",
            endFirst: [
                "muž", "nůž", "kříž", "lež", "blíž", "páž",
                "věž", "mládež", "garáž", "pláž",
                "výtěž", "stráž", "louž", "loupež"
            ],
            endSecond: [
                "koš", "myš", "tuš", "guláš", "marš", "pleš",
                "Matyáš", "Lukáš", "Tomáš", "náš", "váš",
                "Mikuláš"
            ],
            midFirst: [
                "nožík", "kožich", "služba", "nůžky", "kůže", "louže",
                "možný", "snížit", "složit", "každý", "důležitý",
                "výživa", "dožínky", "manžel", "kožešina",
                "úžasný", "stěžovat", "množství", "ležet",
                "ježek", "kožíšek", "podložka", "složka"
            ],
            midSecond: [
                "košík", "myška", "pošta", "prší", "košile", "výška",
                "hruška", "šiška", "tušit", "pěšky", "sušenka", "třešně",
                "strašidlo", "plášť", "rušný", "škola",
                "polštář", "koštovat", "mušle",
                "lišák", "ušák"
            ]);

        // ═══════════════════════════════════════════════════════════
        // G / K   (znělá / neznělá)
        // ═══════════════════════════════════════════════════════════
        AddPair(words, "G_K",
            endFirst: [
                "dialog", "katalog", "analog", "blog", "smog",
                "epilog", "prolog", "monolog"
            ],
            endSecond: [
                "rak", "vlak", "zrak", "oblak", "mrak", "jazyk", "hrnek",
                "hák", "lék", "rok", "zvuk", "tlak", "blok",
                "kluk", "vrak", "pluk", "krok", "hřebík",
                "dárek", "pořádek", "soudek", "výrok", "balík"
            ],
            midFirst: [
                "program", "garáž", "agentura", "elegant",
                "legrace", "magnet", "signál", "organizace",
                "telegram", "diagnóza", "migrace", "agregát",
                "golfový", "dogma", "agresivní",
                "regál"
            ],
            midSecond: [
                "okno", "kukla", "kaktus", "skákat", "leknout",
                "briketa", "raketa", "sekera",
                "hokej", "cvakat", "ukázat",
                "dekret", "mikina", "rekord", "bukový",
                "rukavice", "poklad", "pokuta", "sukně"
            ]);

        // ═══════════════════════════════════════════════════════════
        // H / CH   (znělá / neznělá)
        // ═══════════════════════════════════════════════════════════
        AddPair(words, "H_CH",
            endFirst: [
                "sněh", "Bůh", "roh", "pluh", "břeh", "vrh", "luh",
                "tah", "pruh", "sloh", "výběh",
                "návrh", "výtah",
                "potah", "dosah", "přesah", "odtah", "zátah"
            ],
            endSecond: [
                "vrch", "mech", "prach", "smích", "duch", "strach",
                "hrách", "ořech", "Čech", "Oldřich", "Vojtěch",
                "plech", "hoch", "pach", "ruch",
                "vzduch", "dech", "vzdech",
                "výslech", "spěch", "rozruch"
            ],
            midFirst: [
                "zahrada", "nehty", "lahev", "pohádka", "souhlas",
                "rohož", "nahý", "pohyb",
                "ohrada", "nahrávka", "pohoda",
                "drahý", "ohrožený", "bahno",
                "výhoda", "nehoda", "pohár", "zahradník"
            ],
            midSecond: [
                "záchod", "zachránit", "záchvat", "rachot", "ucho",
                "pochopit", "technika", "uchovat", "vychovat",
                "pochod", "vrchol", "kuchyně",
                "obchod", "průchod", "nachový", "schovat",
                "výchova", "nachlazení", "pochoutka", "úchyt",
                "procházka", "mechový"
            ]);

        return words;
    }

    /// <summary>
    /// Automaticky detekuje kolize (stejný WordWithBlank v rámci páru → obě písmena platná)
    /// + ruční doplnění pro slova kde protějšek není v bance, ale existuje v češtině.
    /// </summary>
    private static Dictionary<(string WordWithBlank, string Pair), AmbiguousEntry> BuildAmbiguousBlanks()
    {
        var result = new Dictionary<(string, string), AmbiguousEntry>();

        // 1) Automatická detekce: skupiny se stejným WordWithBlank v rámci páru
        //    kde existuje víc než jedno unikátní CorrectLetter
        var groups = AllWords
            .GroupBy(w => (w.WordWithBlank, w.Pair))
            .Where(g => g.Select(w => w.CorrectLetter).Distinct().Count() > 1);

        foreach (var group in groups)
        {
            var letters = group.Select(w => w.CorrectLetter).Distinct().ToHashSet();
            var letterToWord = new Dictionary<string, string>();
            foreach (var w in group)
                letterToWord.TryAdd(w.CorrectLetter, w.FullWord);

            result[group.Key] = new AmbiguousEntry(letters, letterToWord);
        }

        // 2) Ruční doplnění: slova kde protějšek není v bance, ale je to platné české slovo
        //    Formát: (wordWithBlank, pair, missingLetter, missingFullWord)
        (string blank, string pair, string letter, string fullWord)[] manualAmbiguous =
        [
            // "ko_" → Z/S: koz (genitiv "koza") / kos – obojí může být
            // "le_" → Z/S: lez (leze!) / les – ale "lez" je neformální imperativ, akceptujeme
        ];

        foreach (var (blank, pair, letter, fullWord) in manualAmbiguous)
        {
            var key = (blank, pair);
            if (result.TryGetValue(key, out var existing))
            {
                existing.AllCorrectLetters.Add(letter);
                existing.LetterToFullWord.TryAdd(letter, fullWord);
            }
            else
            {
                // Najdeme původní slovo v bance
                var original = AllWords.FirstOrDefault(w => w.WordWithBlank == blank && w.Pair == pair);
                if (original != null)
                {
                    result[key] = new AmbiguousEntry(
                        [original.CorrectLetter, letter],
                        new Dictionary<string, string>
                        {
                            { original.CorrectLetter, original.FullWord },
                            { letter, fullWord }
                        });
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Přidá slova pro jeden pár do seznamu. Automaticky generuje zobrazení s vynechaným písmenem.
    /// </summary>
    private static void AddPair(
        List<PcWord> target,
        string pair,
        string[] endFirst,
        string[] endSecond,
        string[] midFirst,
        string[] midSecond)
    {
        var parts = pair.Split('_');
        string firstLetter = parts[0].ToLower();
        string secondLetter = parts[1].ToLower();

        foreach (var word in endFirst)
            TryAdd(target, pair, word, firstLetter, "End");

        foreach (var word in endSecond)
            TryAdd(target, pair, word, secondLetter, "End");

        foreach (var word in midFirst)
            TryAdd(target, pair, word, firstLetter, "Middle");

        foreach (var word in midSecond)
            TryAdd(target, pair, word, secondLetter, "Middle");
    }

    private static void TryAdd(List<PcWord> target, string pair, string fullWord, string correctLetter, string position)
    {
        string lower = fullWord.ToLower();

        int idx;
        if (position == "End")
        {
            if (lower.EndsWith(correctLetter))
            {
                idx = lower.Length - correctLetter.Length;
            }
            else
            {
                idx = lower.LastIndexOf(correctLetter);
            }
        }
        else
        {
            idx = lower.IndexOf(correctLetter);
            if (idx >= 0 && idx == lower.Length - correctLetter.Length && lower.Length > correctLetter.Length)
            {
                var earlier = lower[..idx].LastIndexOf(correctLetter);
                if (earlier >= 0) idx = earlier;
            }
        }

        if (idx < 0) return;

        string wordWithBlank = fullWord[..idx] + "_" + fullWord[(idx + correctLetter.Length)..];
        string actualLetter = fullWord.Substring(idx, correctLetter.Length).ToLower();

        target.Add(new PcWord(pair, fullWord, wordWithBlank, actualLetter, position));
    }
}

/// <summary>
/// Info o nejednoznačném blanku – obě písmena z páru tvoří platné slovo.
/// </summary>
public class AmbiguousEntry
{
    public HashSet<string> AllCorrectLetters { get; }
    public Dictionary<string, string> LetterToFullWord { get; }

    public AmbiguousEntry(HashSet<string> allCorrectLetters, Dictionary<string, string> letterToFullWord)
    {
        AllCorrectLetters = allCorrectLetters;
        LetterToFullWord = letterToFullWord;
    }
}

/// <summary>
/// Jedno slovo pro trénink párových souhlásek.
/// </summary>
public record PcWord(
    string Pair,            // "B_P"
    string FullWord,        // "chléb"
    string WordWithBlank,   // "chle_"
    string CorrectLetter,   // "b"
    string Position         // "End" / "Middle"
);
