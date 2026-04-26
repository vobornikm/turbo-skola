using TurboSkola.Utilities;

namespace TurboSkola.TrainingGenerators;

/// <summary>
/// Generátor pro násobení a dělení dvojkou do stovky.
/// Násobení: 0–50 × 2, Dělení: sudá čísla 0–100 ÷ 2
/// </summary>
public class MultiplyDivideBy2Generator : ExerciseGeneratorBase
{
    private bool _includeMultiplication = true;
    private bool _includeDivision = true;

    public override string Name => "Násobení a dělení 2";
    public override string Description => "Trénink násobení (11–50 × 2) a dělení sudých čísel (22–100 ÷ 2)";

    protected override void OnInitialize()
    {
        _includeMultiplication = GetSetting("IncludeMultiplication", true);
        _includeDivision = GetSetting("IncludeDivision", true);

        if (!_includeMultiplication && !_includeDivision)
            throw new InvalidOperationException("Musí být vybrána alespoň jedna operace!");
    }

    public override GeneratedExercise GenerateExercise()
    {
        bool isMultiplication;
        if (_includeMultiplication && _includeDivision)
            isMultiplication = Random.Next(2) == 0;
        else
            isMultiplication = _includeMultiplication;

        Exercise exercise;
        string category;

        if (isMultiplication)
        {
            int multiplicand = Random.Next(11, 51); // 11–50
            exercise = new Exercise
            {
                FirstNumber = multiplicand,
                SecondNumber = 2,
                Operation = "×",
                CorrectAnswer = multiplicand * 2,
                DisplayText = $"{multiplicand} × 2"
            };
            category = "multiplication_2";
        }
        else
        {
            // Dělení: sudá čísla 22–100 (tj. 22, 24, 26, ... 100)
            // Random.Next(11, 51) dává 11–50, *2 = 22–100
            int dividend = Random.Next(11, 51) * 2;
            exercise = new Exercise
            {
                FirstNumber = dividend,
                SecondNumber = 2,
                Operation = "÷",
                CorrectAnswer = dividend / 2,
                DisplayText = $"{dividend} ÷ 2"
            };
            category = "division_2";
        }

        return new GeneratedExercise
        {
            Exercise = exercise,
            Category = category
        };
    }

    public override bool ValidateAnswer(Exercise exercise, int userAnswer)
        => exercise.CorrectAnswer == userAnswer;

    public override Dictionary<string, object> GetDefaultConfiguration()
    {
        return new Dictionary<string, object>
        {
            { "IncludeMultiplication", true },
            { "IncludeDivision", true }
        };
    }

    /// <summary>
    /// Vrátí nápovědu pro dané cvičení.
    /// U násobení: a + a
    /// U dělení: rozklad na desítkovou část a zbytek (pokud číslo > 20)
    /// </summary>
    public static MultiplyDivideHint GetHint(Exercise exercise)
    {
        if (exercise.Operation == "×")
        {
            int a = exercise.FirstNumber;
            return new MultiplyDivideHint
            {
                IsMultiplication = true,
                OriginalNumber = a,
                HintText = $"Sečti {a} + {a}",
                AdditionResult = a + a
            };
        }
        else
        {
            int n = exercise.FirstNumber;
            // Čísla násobná 20 (20, 40, 60, 80, 100) a čísla ≤ 20 se nerozkládají
            if (n % 20 == 0)
            {
                return new MultiplyDivideHint
                {
                    IsMultiplication = false,
                    OriginalNumber = n,
                    UseDecomposition = false,
                    HintText = $"{n} ÷ 2 = ?",
                    DirectResult = n / 2
                };
            }
            else
            {
                // Rozklad: zaokrouhlíme dolů na nejbližší násobek 20
                int bigPart = (n / 20) * 20;
                int smallPart = n - bigPart;

                return new MultiplyDivideHint
                {
                    IsMultiplication = false,
                    OriginalNumber = n,
                    UseDecomposition = true,
                    HintText = $"Rozlož {n} na {bigPart} + {smallPart}",
                    BigPart = bigPart,
                    SmallPart = smallPart,
                    BigPartResult = bigPart / 2,
                    SmallPartResult = smallPart / 2,
                    DirectResult = n / 2
                };
            }
        }
    }
}

public class MultiplyDivideHint
{
    public bool IsMultiplication { get; set; }
    public int OriginalNumber { get; set; }
    public bool UseDecomposition { get; set; }
    public string HintText { get; set; } = "";

    // Násobení
    public int AdditionResult { get; set; }

    // Dělení přímé
    public int DirectResult { get; set; }

    // Dělení rozkladem
    public int BigPart { get; set; }
    public int SmallPart { get; set; }
    public int BigPartResult { get; set; }
    public int SmallPartResult { get; set; }
}
