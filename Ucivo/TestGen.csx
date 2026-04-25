using TurboSkola.Services;
using TurboSkola.TrainingGenerators;
var s = new MixedMathTrainingSettings { IncludeAddition=true, IncludeSubtraction=true, IncludeMultiplication=true, IncludeDivision=true, NumberCounts=[2,3,4], EnableScratchWork=true, IncludeParentheses=false, IncludeTricks=false };
var g = new MixedMathExerciseGenerator(s);
for(int i=0;i<30;i++){var e=g.GenerateExercise(); var segs=string.Join("",e.Segments.Select(seg=>seg.ScratchValue.HasValue?$"[{seg.Text}={seg.ScratchValue}]":seg.Text)); Console.WriteLine($"{e.DisplayText} = {e.CorrectAnswer}  scratch: {segs}");}
