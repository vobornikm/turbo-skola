using System.Reflection;
using System.Text.Json;

namespace TurboSkola.TrainingGenerators;

/// <summary>
/// Factory pro vytváøení instancí generátorù podle typu
/// </summary>
public interface IExerciseGeneratorFactory
{
    IExerciseGenerator CreateGenerator(string generatorClassName, string? configurationJson = null);
    IExerciseGenerator CreateGenerator<T>() where T : IExerciseGenerator, new();
    List<string> GetAvailableGenerators();
}

public class ExerciseGeneratorFactory : IExerciseGeneratorFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _registeredGenerators = new();

    public ExerciseGeneratorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        RegisterDefaultGenerators();
    }

    private void RegisterDefaultGenerators()
    {
        // Automaticky najít všechny generátory v assembly
        var generatorType = typeof(IExerciseGenerator);
        var generators = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && generatorType.IsAssignableFrom(t));

        foreach (var generator in generators)
        {
            _registeredGenerators[generator.FullName!] = generator;
        }
    }

    public IExerciseGenerator CreateGenerator(string generatorClassName, string? configurationJson = null)
    {
        if (!_registeredGenerators.TryGetValue(generatorClassName, out var generatorType))
        {
            throw new InvalidOperationException($"Generátor '{generatorClassName}' není registrován!");
        }

        // Vytvoøit instanci
        var generator = (IExerciseGenerator)Activator.CreateInstance(generatorType)!;

        // Inicializovat s konfigurací
        if (!string.IsNullOrEmpty(configurationJson))
        {
            var customSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(configurationJson) 
                               ?? new Dictionary<string, object>();
            
            var config = new ExerciseGeneratorConfig
            {
                CustomSettings = customSettings
            };
            
            generator.Initialize(config);
        }

        return generator;
    }

    public IExerciseGenerator CreateGenerator<T>() where T : IExerciseGenerator, new()
    {
        return new T();
    }

    public List<string> GetAvailableGenerators()
    {
        return _registeredGenerators.Keys.ToList();
    }
}
