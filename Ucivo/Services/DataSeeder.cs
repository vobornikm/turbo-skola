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
        // Pokud už existují data, pøeskoèit
        if (await context.SchoolLevels.AnyAsync())
        {
            return;
        }

        // 1. STUPNÌ ŠKOLY
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
}
