namespace TurboSkola.Utilities;

/// <summary>
/// Převádí matematické výrazy na mluvený český text pro Text-to-Speech.
/// </summary>
public static class MathSpeechHelper
{
    /// <summary>
    /// Převede DisplayText matematického příkladu na mluvený text v češtině.
    /// Např. "25 + 14" → "dvacet pět plus čtrnáct"
    /// Např. "(5 + 7) - 3" → "závorka pět plus sedm konec závorky minus tři"
    /// Např. "4 × 6" → "čtyři krát šest"
    /// Např. "_ + 5 = 12" → "kolik plus pět rovná se dvanáct"
    /// </summary>
    public static string ToSpokenCzech(string displayText)
    {
        if (string.IsNullOrWhiteSpace(displayText))
            return "";

        var result = new System.Text.StringBuilder();
        var tokens = Tokenize(displayText);

        foreach (var token in tokens)
        {
            if (result.Length > 0)
                result.Append(' ');

            result.Append(token switch
            {
                "+" => "plus",
                "-" => "minus",
                "×" => "krát",
                "÷" => "děleno",
                "=" => "rovná se",
                "(" => "závorka",
                ")" => "konec závorky",
                "_" => "kolik",
                _ when int.TryParse(token, out int num) => NumberToWords(num),
                _ => token
            });
        }

        return result.ToString();
    }

    private static List<string> Tokenize(string text)
    {
        var tokens = new List<string>();
        var current = new System.Text.StringBuilder();

        foreach (char c in text)
        {
            if (c == ' ')
            {
                if (current.Length > 0)
                {
                    tokens.Add(current.ToString());
                    current.Clear();
                }
            }
            else if (c is '(' or ')')
            {
                if (current.Length > 0)
                {
                    tokens.Add(current.ToString());
                    current.Clear();
                }
                tokens.Add(c.ToString());
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
            tokens.Add(current.ToString());

        return tokens;
    }

    private static string NumberToWords(int number)
    {
        if (number < 0) return "minus " + NumberToWords(-number);
        if (number == 0) return "nula";

        return number switch
        {
            1 => "jedna",
            2 => "dva",
            3 => "tři",
            4 => "čtyři",
            5 => "pět",
            6 => "šest",
            7 => "sedm",
            8 => "osm",
            9 => "devět",
            10 => "deset",
            11 => "jedenáct",
            12 => "dvanáct",
            13 => "třináct",
            14 => "čtrnáct",
            15 => "patnáct",
            16 => "šestnáct",
            17 => "sedmnáct",
            18 => "osmnáct",
            19 => "devatenáct",
            20 => "dvacet",
            >= 21 and <= 29 => "dvacet " + NumberToWords(number - 20),
            30 => "třicet",
            >= 31 and <= 39 => "třicet " + NumberToWords(number - 30),
            40 => "čtyřicet",
            >= 41 and <= 49 => "čtyřicet " + NumberToWords(number - 40),
            50 => "padesát",
            >= 51 and <= 59 => "padesát " + NumberToWords(number - 50),
            60 => "šedesát",
            >= 61 and <= 69 => "šedesát " + NumberToWords(number - 60),
            70 => "sedmdesát",
            >= 71 and <= 79 => "sedmdesát " + NumberToWords(number - 70),
            80 => "osmdesát",
            >= 81 and <= 89 => "osmdesát " + NumberToWords(number - 80),
            90 => "devadesát",
            >= 91 and <= 99 => "devadesát " + NumberToWords(number - 90),
            100 => "sto",
            _ => number.ToString()
        };
    }
}
