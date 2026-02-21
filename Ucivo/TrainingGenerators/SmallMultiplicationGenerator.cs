using TurboSkola.Utilities;

namespace TurboSkola.TrainingGenerators;

/// <summary>
/// Generátor pro malou násobilku (0-10)
/// </summary>
public class SmallMultiplicationGenerator : ExerciseGeneratorBase
{
    private List<int> _multipliers = new();
    private bool _includeMultiplication = true;
    private bool _includeDivision = true;

    public override string Name => "Malá násobilka";
    public override string Description => "Generátor pro trénink násobilky 0-10 vèetnì dìlení";

    protected override void OnInitialize()
    {
        // Naèíst nastavení z konfigurace
        _multipliers = GetSetting("Multipliers", new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        _includeMultiplication = GetSetting("IncludeMultiplication", true);
        _includeDivision = GetSetting("IncludeDivision", true);

        if (_multipliers.Count == 0)
        {
            throw new InvalidOperationException("Musí být vybrán alespoò jeden násobitel!");
        }

        if (!_includeMultiplication && !_includeDivision)
        {
            throw new InvalidOperationException("Musí být vybrána alespoò jedna operace!");
        }
    }

    public override GeneratedExercise GenerateExercise()
    {
        // Rozhodnout operaci
        bool isMultiplication;
        if (_includeMultiplication && _includeDivision)
        {
            isMultiplication = Random.Next(2) == 0;
        }
        else
        {
            isMultiplication = _includeMultiplication;
        }

        // Vybrat násobitel
        int multiplier = _multipliers[Random.Next(_multipliers.Count)];
        int multiplicand = Random.Next(0, 11); // 0-10

        Exercise exercise;
        string category;

        if (isMultiplication)
        {
            exercise = new Exercise
            {
                FirstNumber = multiplier,
                SecondNumber = multiplicand,
                Operation = "×",
                CorrectAnswer = multiplier * multiplicand,
                DisplayText = $"{multiplier} × {multiplicand}"
            };
            category = $"multiplication_{multiplier}";
        }
        else
        {
            // Dìlení - vygenerovat jako opaènou operaci násobení
            int result = Random.Next(0, 11);
            int divisor = multiplier;
            int dividend = divisor * result;

            exercise = new Exercise
            {
                FirstNumber = dividend,
                SecondNumber = divisor,
                Operation = "÷",
                CorrectAnswer = result,
                DisplayText = $"{dividend} ÷ {divisor}"
            };
            category = $"division_{divisor}";
        }

        return new GeneratedExercise
        {
            Exercise = exercise,
            Category = category
        };
    }

    public override bool ValidateAnswer(Exercise exercise, int userAnswer)
    {
        return exercise.CorrectAnswer == userAnswer;
    }

    public override Dictionary<string, object> GetDefaultConfiguration()
    {
        return new Dictionary<string, object>
        {
            { "Multipliers", new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } },
            { "IncludeMultiplication", true },
            { "IncludeDivision", true }
        };
    }
}
