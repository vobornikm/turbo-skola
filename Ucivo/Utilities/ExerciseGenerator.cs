namespace TurboSkola.Utilities;

public class Exercise
{
    public int FirstNumber { get; set; }
    public int SecondNumber { get; set; }
    public string Operation { get; set; } = string.Empty; // "×" nebo "÷"
    public int CorrectAnswer { get; set; }
    public string DisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Strukturované segmenty výrazu pro zobrazení s mezivýpočty.
    /// Pokud je prázdné, použije se DisplayText jako prostý text.
    /// </summary>
    public List<ExerciseSegment> Segments { get; set; } = [];
}

public class ExerciseGenerator
{
    private readonly Random _random = new Random();

    public List<Exercise> GenerateExercises(
        int count,
        List<int> includedMultipliers,
        bool includeMultiplication,
        bool includeDivision)
    {
        if (!includeMultiplication && !includeDivision)
        {
            throw new ArgumentException("Musí být vybrána alespoň jedna operace.");
        }

        if (includedMultipliers.Count == 0)
        {
            throw new ArgumentException("Musí být vybrán alespoň jeden násobitel.");
        }

        var exercises = new List<Exercise>();

        for (int i = 0; i < count; i++)
        {
            var exercise = GenerateSingleExercise(includedMultipliers, includeMultiplication, includeDivision);
            exercises.Add(exercise);
        }

        return exercises;
    }

    private Exercise GenerateSingleExercise(
        List<int> includedMultipliers,
        bool includeMultiplication,
        bool includeDivision)
    {
        var operations = new List<string>();
        if (includeMultiplication) operations.Add("×");
        if (includeDivision) operations.Add("÷");

        var operation = operations[_random.Next(operations.Count)];

        // Pro dělení vyfiltrovat 0 z násobitelů (nulou nelze dělit!)
        List<int> availableMultipliers = includedMultipliers;
        if (operation == "÷")
        {
            availableMultipliers = includedMultipliers.Where(m => m != 0).ToList();

            // Pokud po filtraci nezůstal žádný násobitel, použij násobení místo toho
            if (availableMultipliers.Count == 0)
            {
                operation = "×";
                availableMultipliers = includedMultipliers;
            }
        }

        var multiplier = availableMultipliers[_random.Next(availableMultipliers.Count)];

        Exercise exercise;

        if (operation == "×")
        {
            // Násobení: number (1-10) × multiplier
            var number = _random.Next(1, 11);
            var correctAnswer = number * multiplier;

            exercise = new Exercise
            {
                FirstNumber = number,
                SecondNumber = multiplier,
                Operation = "×",
                CorrectAnswer = correctAnswer,
                DisplayText = $"{number} × {multiplier}"
            };
        }
        else
        {
            // Dělení: (multiplier × number) ÷ multiplier = number
            // multiplier je teď zaručeně != 0
            var number = _random.Next(1, 11);
            var dividend = multiplier * number;
            var divisor = multiplier;

            exercise = new Exercise
            {
                FirstNumber = dividend,
                SecondNumber = divisor,
                Operation = "÷",
                CorrectAnswer = number,
                DisplayText = $"{dividend} ÷ {divisor}"
            };
        }

        return exercise;
    }
}
