using TurboSkola.Utilities;

namespace TurboSkola.TrainingGenerators;

/// <summary>
/// Konfigurace pro generátor cvièení
/// </summary>
public class ExerciseGeneratorConfig
{
    public int DurationSeconds { get; set; }
    public int? ExerciseCount { get; set; }
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// Výsledek generování cvièení
/// </summary>
public class GeneratedExercise
{
    public Exercise Exercise { get; set; } = null!;
    public string Category { get; set; } = string.Empty; // Pro statistiky (napø. "multiplication_5", "addition_10")
}

/// <summary>
/// Interface pro všechny generátory cvièení
/// </summary>
public interface IExerciseGenerator
{
    /// <summary>
    /// Název generátoru (pro logování a debugging)
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Popis co tento generátor dìlá
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Inicializuje generátor s konfigurací
    /// </summary>
    void Initialize(ExerciseGeneratorConfig config);
    
    /// <summary>
    /// Vygeneruje jedno cvièení
    /// </summary>
    GeneratedExercise GenerateExercise();
    
    /// <summary>
    /// Vygeneruje zadaný poèet cvièení
    /// </summary>
    List<GeneratedExercise> GenerateExercises(int count);
    
    /// <summary>
    /// Validuje uživatelovu odpovìï
    /// </summary>
    bool ValidateAnswer(Exercise exercise, int userAnswer);
    
    /// <summary>
    /// Vrátí výchozí konfiguraci pro tento generátor
    /// </summary>
    Dictionary<string, object> GetDefaultConfiguration();
}
