using Microsoft.EntityFrameworkCore;
using TurboSkola.Data;
using TurboSkola.Data.Models;

namespace TurboSkola.Services;

/// <summary>
/// Služba pro naplnìní databáze základními daty
/// </summary>
public class DataSeeder
{
    public static async Task SeedAsync(UcivoDbContext context)
    {
        // Hlavní seed (jednorázový pøi prvním spuštìní)
        if (!await context.SchoolLevels.AnyAsync())
        {
            await SeedInitialDataAsync(context);
        }

        // Vždy opravit ikony (øeší encoding problémy z pùvodního seedu)
        await SeedFixIconsAsync(context);

        // Vždy zkontrolovat nové tréninky (pøidává chybìjící typy)
        await SeedAddSubTrainingAsync(context);

        // Párové souhlásky
        await SeedPairedConsonantsTrainingAsync(context);
    }

    private static async Task SeedInitialDataAsync(UcivoDbContext context)
    {
        var primary1 = new SchoolLevel
        {
            Name = "1. stupeò ZŠ",
            Code = "PRIMARY_1",
            DisplayOrder = 1,
            IsActive = true
        };

        var primary2 = new SchoolLevel
        {
            Name = "2. stupeò ZŠ",
            Code = "PRIMARY_2",
            DisplayOrder = 2,
            IsActive = true
        };

        context.SchoolLevels.AddRange(primary1, primary2);
        await context.SaveChangesAsync();

        // 2. ROÈNÍKY
        var grades = new List<Grade>();
        for (int i = 1; i <= 9; i++)
        {
            grades.Add(new Grade
            {
                SchoolLevelId = i <= 5 ? primary1.SchoolLevelId : primary2.SchoolLevelId,
                Name = $"{i}. tøída",
                Code = $"GRADE_{i}",
                GradeNumber = i,
                DisplayOrder = i,
                IsActive = true
            });
        }
        context.Grades.AddRange(grades);
        await context.SaveChangesAsync();

        // 3. PØEDMÌTY
        var mathematics = new Subject
        {
            Name = "Matematika",
            Code = "MATH",
            Icon = "??",
            Color = "#667eea",
            DisplayOrder = 1,
            IsActive = true
        };

        var czech = new Subject
        {
            Name = "Èeský jazyk",
            Code = "CZECH",
            Icon = "??",
            Color = "#48bb78",
            DisplayOrder = 2,
            IsActive = true
        };

        context.Subjects.AddRange(mathematics, czech);
        await context.SaveChangesAsync();

        // 4. PØIØAZENÍ PØEDMÌTÙ K ROÈNÍKÙM
        var subjectGrades = new List<SubjectGrade>();
        
        // Matematika pro 1.-9. tøídu
        foreach (var grade in grades)
        {
            subjectGrades.Add(new SubjectGrade
            {
                SubjectId = mathematics.SubjectId,
                GradeId = grade.GradeId,
                IsActive = true
            });
        }

        // Èeský jazyk pro 1.-9. tøídu
        foreach (var grade in grades)
        {
            subjectGrades.Add(new SubjectGrade
            {
                SubjectId = czech.SubjectId,
                GradeId = grade.GradeId,
                IsActive = true
            });
        }

        context.SubjectGrades.AddRange(subjectGrades);
        await context.SaveChangesAsync();

        // 5. TYPY TRÉNINKÙ
        var multiplicationTraining = new TrainingType
        {
            SubjectId = mathematics.SubjectId,
            Name = "Malá násobilka",
            Code = "SMALL_MULTIPLICATION",
            Description = "Trénink násobilky 0-10 vèetnì dìlení",
            Icon = "??",
            GeneratorClassName = "TurboSkola.TrainingGenerators.SmallMultiplicationGenerator",
            ConfigurationJson = @"{
                ""Multipliers"": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
                ""IncludeMultiplication"": true,
                ""IncludeDivision"": true
            }",
            MinGradeNumber = 2,
            MaxGradeNumber = 5,
            DisplayOrder = 1,
            IsActive = true
        };

        context.TrainingTypes.Add(multiplicationTraining);
        await context.SaveChangesAsync();
    }

    private static async Task SeedFixIconsAsync(UcivoDbContext context)
    {
        // Oprava ikon pøedmìtù — Unicode escape = bezpeèné pøed encoding problémy
        var subjectIcons = new Dictionary<string, string>
        {
            { "MATH",  "\U0001F9EE" }, // ??
            { "CZECH", "\U0001F4DA" }  // ??
        };

        var trainingIcons = new Dictionary<string, string>
        {
            { "SMALL_MULTIPLICATION", "\u2716\uFE0F" }, // ??
            { "ADD_SUB_100",          "\u2795"  },       // ?
            { "PAIRED_CONSONANTS",    "\U0001F524" }     // ??
        };

        bool changed = false;

        var subjects = await context.Subjects.Where(s => subjectIcons.Keys.Contains(s.Code)).ToListAsync();
        foreach (var s in subjects)
        {
            if (subjectIcons.TryGetValue(s.Code, out var icon) && s.Icon != icon)
            {
                s.Icon = icon;
                changed = true;
            }
        }

        var trainings = await context.TrainingTypes.Where(t => trainingIcons.Keys.Contains(t.Code)).ToListAsync();
        foreach (var t in trainings)
        {
            if (trainingIcons.TryGetValue(t.Code, out var icon) && t.Icon != icon)
            {
                t.Icon = icon;
                changed = true;
            }
        }

        if (changed)
            await context.SaveChangesAsync();
    }

    private static async Task SeedAddSubTrainingAsync(UcivoDbContext context)
    {
        if (await context.TrainingTypes.AnyAsync(t => t.Code == "ADD_SUB_100"))
            return;

        var mathematics = await context.Subjects.FirstOrDefaultAsync(s => s.Code == "MATH");
        if (mathematics == null) return;

        var addSubTraining = new TrainingType
        {
            SubjectId = mathematics.SubjectId,
            Name = "Sèítání a odèítání do 100",
            Code = "ADD_SUB_100",
            Description = "Trénink sèítání a odèítání èísel do 100 pro 2. tøídu ZŠ",
            Icon = "?",
            GeneratorClassName = "TurboSkola.TrainingGenerators.AddSubExerciseGenerator",
            MinGradeNumber = 1,
            MaxGradeNumber = 4,
            DisplayOrder = 2,
            IsActive = true
        };

        context.TrainingTypes.Add(addSubTraining);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPairedConsonantsTrainingAsync(UcivoDbContext context)
    {
        if (await context.TrainingTypes.AnyAsync(t => t.Code == "PAIRED_CONSONANTS"))
            return;

        var czech = await context.Subjects.FirstOrDefaultAsync(s => s.Code == "CZECH");
        if (czech == null) return;

        var pairedConsonants = new TrainingType
        {
            SubjectId = czech.SubjectId,
            Name = "P\u00e1rov\u00e9 souhl\u00e1sky",
            Code = "PAIRED_CONSONANTS",
            Description = "Tr\u00e9nink p\u00e1rov\u00fdch souhl\u00e1sek B/P, V/F, D/T, \u010e/\u0164, Z/S, \u017d/\u0160, G/K, H/CH",
            Icon = "\U0001F524",
            GeneratorClassName = "TurboSkola.TrainingGenerators.PairedConsonantsGenerator",
            MinGradeNumber = 2,
            MaxGradeNumber = 4,
            DisplayOrder = 1,
            IsActive = true
        };

        context.TrainingTypes.Add(pairedConsonants);
        await context.SaveChangesAsync();
    }
}
