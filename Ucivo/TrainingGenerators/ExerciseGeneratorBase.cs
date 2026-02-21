using TurboSkola.Utilities;

namespace TurboSkola.TrainingGenerators;

/// <summary>
/// Abstraktní tøída pro generátory - implementuje spoleènou logiku
/// </summary>
public abstract class ExerciseGeneratorBase : IExerciseGenerator
{
    protected ExerciseGeneratorConfig Config { get; private set; } = null!;
    protected Random Random { get; } = new Random();

    public abstract string Name { get; }
    public abstract string Description { get; }

    public virtual void Initialize(ExerciseGeneratorConfig config)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
        OnInitialize();
    }

    /// <summary>
    /// Override pro specifickou inicializaci
    /// </summary>
    protected virtual void OnInitialize() { }

    public abstract GeneratedExercise GenerateExercise();

    public virtual List<GeneratedExercise> GenerateExercises(int count)
    {
        var exercises = new List<GeneratedExercise>();
        for (int i = 0; i < count; i++)
        {
            exercises.Add(GenerateExercise());
        }
        return exercises;
    }

    public abstract bool ValidateAnswer(Exercise exercise, int userAnswer);

    public abstract Dictionary<string, object> GetDefaultConfiguration();

    /// <summary>
    /// Helper metoda pro získání custom nastavení
    /// </summary>
    protected T GetSetting<T>(string key, T defaultValue)
    {
        if (Config.CustomSettings.TryGetValue(key, out var value))
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
        return defaultValue;
    }
}
