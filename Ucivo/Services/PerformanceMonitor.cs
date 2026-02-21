using System.Diagnostics;

namespace TurboSkola.Services;

public interface IPerformanceMonitor
{
    void LogMemory(string location);
    void StartTimer(string operationName);
    void StopTimer(string operationName);
    void LogComponentLifecycle(string componentName, string eventName, int instanceHash);
}

public class PerformanceMonitor : IPerformanceMonitor
{
    private readonly Dictionary<string, Stopwatch> _timers = new();
    private readonly Dictionary<string, List<long>> _operationTimes = new();
    private int _navigationCount = 0;

    public void LogMemory(string location)
    {
        var process = Process.GetCurrentProcess();
        var workingSet = process.WorkingSet64 / 1024 / 1024; // MB
        var privateMemory = process.PrivateMemorySize64 / 1024 / 1024; // MB
        
        Console.WriteLine($"[MEMORY] {location}: WorkingSet={workingSet}MB, Private={privateMemory}MB");
    }

    public void StartTimer(string operationName)
    {
        if (!_timers.ContainsKey(operationName))
        {
            _timers[operationName] = new Stopwatch();
        }
        _timers[operationName].Restart();
    }

    public void StopTimer(string operationName)
    {
        if (_timers.TryGetValue(operationName, out var timer))
        {
            timer.Stop();
            
            if (!_operationTimes.ContainsKey(operationName))
            {
                _operationTimes[operationName] = new List<long>();
            }
            _operationTimes[operationName].Add(timer.ElapsedMilliseconds);
            
            var times = _operationTimes[operationName];
            var avg = times.Count > 0 ? times.Average() : 0;
            
            Console.WriteLine($"[PERF] {operationName}: {timer.ElapsedMilliseconds}ms (avg: {avg:F1}ms, count: {times.Count})");
            
            // Warning pokud se èas zvyšuje
            if (times.Count > 5 && timer.ElapsedMilliseconds > avg * 2)
            {
                Console.WriteLine($"[PERF WARNING] {operationName} is getting SLOWER! Current: {timer.ElapsedMilliseconds}ms vs Avg: {avg:F1}ms");
            }
        }
    }

    public void LogComponentLifecycle(string componentName, string eventName, int instanceHash)
    {
        if (eventName == "OnInitialized")
        {
            _navigationCount++;
        }
        
        Console.WriteLine($"[LIFECYCLE] {componentName} #{instanceHash} - {eventName} (Navigation #{_navigationCount})");
    }
}
