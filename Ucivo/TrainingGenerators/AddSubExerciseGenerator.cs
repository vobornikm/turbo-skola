using TurboSkola.Services;
using TurboSkola.Utilities;

namespace TurboSkola.TrainingGenerators;

/// <summary>
/// Generátor pro sčítání a odčítání do 100 (2. třída ZŠ)
/// </summary>
public class AddSubExerciseGenerator
{
    private readonly Random _random = new();
    private readonly AddSubTrainingSettings _settings;

    public AddSubExerciseGenerator(AddSubTrainingSettings settings)
    {
        _settings = settings;
    }

    public Exercise GenerateExercise()
    {
        var pool = BuildTypePool();
        if (pool.Count == 0)
            return FallbackExercise();

        for (int attempt = 0; attempt < 150; attempt++)
        {
            var type = pool[_random.Next(pool.Count)];
            var ex = type switch
            {
                ExType.Addition => TryGenerateAddition(),
                ExType.Subtraction => TryGenerateSubtraction(),
                ExType.Parentheses => TryGenerateParentheses(),
                ExType.MultipleNumbers => TryGenerateMultipleNumbers(),
                _ => null
            };
            if (ex != null) return ex;
        }

        return FallbackExercise();
    }

    private enum ExType { Addition, Subtraction, Parentheses, MultipleNumbers }

    private List<ExType> BuildTypePool()
    {
        var pool = new List<ExType>();

        // Jednoduchá sčítání/odčítání (2 čísla) – přidáme pouze pokud:
        //   a) režim "více čísel" není zapnutý, NEBO
        //   b) je zapnutý a uživatel má v počtech vybranou dvojku
        bool multipleOn = _settings.IncludeMultipleNumbers && _settings.MultipleNumbersCounts.Count > 0;
        bool twoNumbersAllowed = !multipleOn || _settings.MultipleNumbersCounts.Contains(2);

        if (_settings.IncludeAddition && twoNumbersAllowed) pool.Add(ExType.Addition);
        if (_settings.IncludeSubtraction && twoNumbersAllowed) pool.Add(ExType.Subtraction);

        bool both = _settings.IncludeAddition && _settings.IncludeSubtraction;
        if (both && _settings.IncludeParentheses) pool.Add(ExType.Parentheses);
        if (both && _settings.IncludeMultipleNumbers) pool.Add(ExType.MultipleNumbers);
        return pool;
    }

    // ============================================================
    // SČÍTÁNÍ
    // ============================================================

    private Exercise? TryGenerateAddition()
    {
        for (int i = 0; i < 80; i++)
        {
            int b = _settings.AdditionSecondTwoDigit
                ? _random.Next(10, 90)
                : _random.Next(1, 10);

            int maxA = 100 - b;
            if (maxA < 1) continue;
            int a = _random.Next(1, maxA + 1);

            bool hasCarry = (a % 10) + (b % 10) >= 10;
            if (!_settings.AdditionCarryOver && hasCarry) continue;          // vypnuto → nikdy carry
            if (_settings.AdditionCarryOver && !hasCarry && _random.Next(5) != 0) continue; // zapnuto → ~20 % bez carry

            return BuildAdditionExercise(a, b);
        }
        return null;
    }

    private Exercise BuildAdditionExercise(int a, int b)
    {
        int result = a + b;

        if (!_settings.AdditionTricks)
        {
            return new Exercise
            {
                FirstNumber = a, SecondNumber = b, Operation = "+",
                CorrectAnswer = result, DisplayText = $"{a} + {b}"
            };
        }

        return _random.Next(5) switch
        {
            1 => new Exercise { FirstNumber = b, SecondNumber = a, Operation = "+", CorrectAnswer = result, DisplayText = $"{b} + {a}" },
            2 => new Exercise { FirstNumber = a, SecondNumber = b, Operation = "+", CorrectAnswer = b, DisplayText = $"{a} + _ = {result}" },
            3 => new Exercise { FirstNumber = a, SecondNumber = b, Operation = "+", CorrectAnswer = a, DisplayText = $"_ + {b} = {result}" },
            4 => new Exercise { FirstNumber = a, SecondNumber = b, Operation = "+", CorrectAnswer = a, DisplayText = $"{result} = _ + {b}" },
            _ => new Exercise { FirstNumber = a, SecondNumber = b, Operation = "+", CorrectAnswer = result, DisplayText = $"{a} + {b}" }
        };
    }

    // ============================================================
    // ODČÍTÁNÍ
    // ============================================================

    private Exercise? TryGenerateSubtraction()
    {
        for (int i = 0; i < 80; i++)
        {
            int b = _settings.SubtractionSecondTwoDigit
                ? _random.Next(10, 90)
                : _random.Next(1, 10);

            if (b >= 100) continue;
            int a = _random.Next(b + 1, Math.Min(101, b + 91));

            bool hasBorrow = (a % 10) < (b % 10);
            if (!_settings.SubtractionCarryOver && hasBorrow) continue;           // vypnuto → nikdy borrow
            if (_settings.SubtractionCarryOver && !hasBorrow && _random.Next(5) != 0) continue; // zapnuto → ~20 % bez borrow

            return BuildSubtractionExercise(a, b);
        }
        return null;
    }

    private Exercise BuildSubtractionExercise(int a, int b)
    {
        int result = a - b;

        if (!_settings.SubtractionTricks)
        {
            return new Exercise
            {
                FirstNumber = a, SecondNumber = b, Operation = "-",
                CorrectAnswer = result, DisplayText = $"{a} - {b}"
            };
        }

        return _random.Next(3) switch
        {
            1 => new Exercise { FirstNumber = a, SecondNumber = b, Operation = "-", CorrectAnswer = b, DisplayText = $"{a} - _ = {result}" },
            2 => new Exercise { FirstNumber = a, SecondNumber = b, Operation = "-", CorrectAnswer = a, DisplayText = $"_ - {b} = {result}" },
            _ => new Exercise { FirstNumber = a, SecondNumber = b, Operation = "-", CorrectAnswer = result, DisplayText = $"{a} - {b}" }
        };
    }

    // ============================================================
    // ZÁVORKY (bez lumpáren, i kdyby byly povoleny)
    // ============================================================

    private Exercise? TryGenerateParentheses()
    {
        for (int attempt = 0; attempt < 80; attempt++)
        {
            int pattern = _random.Next(6);
            int a = _random.Next(5, 80);
            int b = _random.Next(2, 40);
            int c = _random.Next(2, 40);

            var ex = BuildParentheses(a, b, c, pattern);
            if (ex != null) return ex;
        }
        return null;
    }

    private Exercise? BuildParentheses(int a, int b, int c, int pattern)
    {
        int result;
        string display;
        List<ExerciseSegment>? segments = null;

        switch (pattern)
        {
            case 0: // a + (b + c)
                result = a + b + c;
                if (result > 100) return null;
                display = $"{a} + ({b} + {c})";
                if (_settings.EnableScratchWork)
                    segments = [
                        new() { Text = $"{a}" },
                        new() { Text = " + " },
                        new() { Text = $"({b} + {c})", ScratchValue = b + c }
                    ];
                break;
            case 1: // a - (b + c)
                result = a - b - c;
                if (result < 0) return null;
                display = $"{a} - ({b} + {c})";
                if (_settings.EnableScratchWork)
                    segments = [
                        new() { Text = $"{a}" },
                        new() { Text = " - " },
                        new() { Text = $"({b} + {c})", ScratchValue = b + c }
                    ];
                break;
            case 2: // (a + b) - c
                if (a + b > 100) return null;
                result = a + b - c;
                if (result < 0) return null;
                display = $"({a} + {b}) - {c}";
                if (_settings.EnableScratchWork)
                    segments = [
                        new() { Text = $"({a} + {b})", ScratchValue = a + b },
                        new() { Text = " - " },
                        new() { Text = $"{c}" }
                    ];
                break;
            case 3: // (a - b) + c
                if (a <= b) return null;
                result = a - b + c;
                if (result > 100) return null;
                display = $"({a} - {b}) + {c}";
                if (_settings.EnableScratchWork)
                    segments = [
                        new() { Text = $"({a} - {b})", ScratchValue = a - b },
                        new() { Text = " + " },
                        new() { Text = $"{c}" }
                    ];
                break;
            case 4: // a + (b - c)
                if (b <= c) return null;
                result = a + (b - c);
                if (result > 100) return null;
                display = $"{a} + ({b} - {c})";
                if (_settings.EnableScratchWork)
                    segments = [
                        new() { Text = $"{a}" },
                        new() { Text = " + " },
                        new() { Text = $"({b} - {c})", ScratchValue = b - c }
                    ];
                break;
            default: // (a + b) + c
                result = a + b + c;
                if (result > 100) return null;
                display = $"({a} + {b}) + {c}";
                if (_settings.EnableScratchWork)
                    segments = [
                        new() { Text = $"({a} + {b})", ScratchValue = a + b },
                        new() { Text = " + " },
                        new() { Text = $"{c}" }
                    ];
                break;
        }

        return new Exercise
        {
            FirstNumber = 0, SecondNumber = 0,
            Operation = "parentheses",
            CorrectAnswer = result,
            DisplayText = display,
            Segments = segments ?? []
        };
    }

    // ============================================================
    // VÍCE ČÍSEL (3–6)
    // ============================================================

    private Exercise? TryGenerateMultipleNumbers()
    {
        var counts = _settings.MultipleNumbersCounts.Where(n => n >= 3).ToList();
        if (counts.Count == 0) counts = [3, 4, 5, 6];

        for (int attempt = 0; attempt < 80; attempt++)
        {
            int count = counts[_random.Next(counts.Count)];
            var numbers = new List<int>();
            var ops = new List<char>();

            numbers.Add(_random.Next(10, 80));

            for (int i = 1; i < count; i++)
            {
                char op = _random.Next(2) == 0 ? '+' : '-';
                ops.Add(op);
                bool twoDigit = (op == '+' && _settings.AdditionSecondTwoDigit)
                             || (op == '-' && _settings.SubtractionSecondTwoDigit);
                int num = twoDigit ? _random.Next(10, 25) : _random.Next(1, 10);
                numbers.Add(num);
            }

            int result = numbers[0];
            bool valid = true;
            for (int i = 0; i < ops.Count; i++)
            {
                result = ops[i] == '+' ? result + numbers[i + 1] : result - numbers[i + 1];
                if (result < 0 || result > 100) { valid = false; break; }
            }
            if (!valid) continue;

            // DisplayText
            var sb = new System.Text.StringBuilder(numbers[0].ToString());
            for (int i = 0; i < ops.Count; i++)
                sb.Append($" {ops[i]} {numbers[i + 1]}");

            // Segmenty s mezivýpočty (pokud je povoleno)
            // Každé číslo a znaménko je vlastní segment. Scratch nad číslem = mezivýsledek po tomto kroku.
            var segments = new List<ExerciseSegment>();
            if (_settings.EnableScratchWork)
            {
                // První číslo (bez scratch)
                segments.Add(new ExerciseSegment { Text = numbers[0].ToString() });

                int running = numbers[0];
                for (int i = 0; i < ops.Count; i++)
                {
                    running = ops[i] == '+' ? running + numbers[i + 1] : running - numbers[i + 1];

                    // Znaménko
                    segments.Add(new ExerciseSegment { Text = $" {ops[i]} " });

                    // Číslo – scratch slot KROMĚ posledního kroku (ten je finální odpověď)
                    int? scratch = (i < ops.Count - 1) ? running : null;
                    segments.Add(new ExerciseSegment { Text = numbers[i + 1].ToString(), ScratchValue = scratch });
                }
            }

            return new Exercise
            {
                FirstNumber = 0, SecondNumber = 0,
                Operation = "multi",
                CorrectAnswer = result,
                DisplayText = sb.ToString(),
                Segments = segments
            };
        }
        return null;
    }

    private Exercise FallbackExercise() =>
        new() { FirstNumber = 25, SecondNumber = 14, Operation = "+", CorrectAnswer = 39, DisplayText = "25 + 14" };
}
