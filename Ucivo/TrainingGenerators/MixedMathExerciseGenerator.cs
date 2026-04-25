using TurboSkola.Services;
using TurboSkola.Utilities;

namespace TurboSkola.TrainingGenerators;

/// <summary>
/// Generátor smíšených příkladů: +, -, ×, ÷ do 100
/// Respektuje prioritu operací (× a ÷ před + a -).
/// Násobení/dělení vždy jednociferné (0–10 × 0–10).
/// Žádný mezivýsledek ani výsledek nepřesáhne 100.
/// Dělení je vždy beze zbytku.
/// Dva po sobě jdoucí × nebo ÷ nejsou povoleny ve flat režimu
/// (vyžadovaly by násobení/dělení dvoucifernými čísly).
/// </summary>
public class MixedMathExerciseGenerator
{
    private readonly Random _random = new();
    private readonly MixedMathTrainingSettings _settings;

    public MixedMathExerciseGenerator(MixedMathTrainingSettings settings)
    {
        _settings = settings;
    }

    public Exercise GenerateExercise()
    {
        for (int attempt = 0; attempt < 200; attempt++)
        {
            var ex = TryGenerate();
            if (ex != null) return ex;
        }
        return FallbackExercise();
    }

    private Exercise? TryGenerate()
    {
        int numCount = _settings.NumberCounts.Count > 0
            ? _settings.NumberCounts[_random.Next(_settings.NumberCounts.Count)]
            : 2;

        bool useParentheses = _settings.IncludeParentheses && numCount >= 3 && _random.Next(3) == 0;

        if (useParentheses)
            return TryGenerateWithParentheses(numCount);

        return TryGenerateFlat(numCount);
    }

    // ============================================================
    // FLAT (bez závorek) — respektuje prioritu operací
    // Nikdy dva po sobě jdoucí × nebo ÷ (dítě by muselo
    // násobit/dělit dvouciferným číslem).
    // ============================================================

    private Exercise? TryGenerateFlat(int numCount)
    {
        var ops = PickOperatorsNoConsecutiveMulDiv(numCount - 1);
        if (ops == null) return null;

        // Generujeme čísla — nejprve obsadíme pozice × a ÷ (jednociferné),
        // pak doplníme zbylé pozice pro + a -.
        // Díky zákazu po sobě jdoucích × / ÷ se pozice nepřekrývají.
        var numbers = new int[numCount];
        var usedPositions = new HashSet<int>();

        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] == '×')
            {
                numbers[i] = _random.Next(1, 11);
                numbers[i + 1] = _random.Next(0, 11);
                usedPositions.Add(i);
                usedPositions.Add(i + 1);
            }
            else if (ops[i] == '÷')
            {
                int quotient = _random.Next(1, 11);
                int divisor = _random.Next(1, 11);
                int dividend = quotient * divisor;
                if (dividend > 100) return null;
                numbers[i] = dividend;
                numbers[i + 1] = divisor;
                usedPositions.Add(i);
                usedPositions.Add(i + 1);
            }
        }

        // Zbylé pozice: čísla pro + a - (1–50)
        for (int i = 0; i < numCount; i++)
        {
            if (!usedPositions.Contains(i))
                numbers[i] = _random.Next(1, 50);
        }

        var numberList = numbers.ToList();

        int? result = EvaluateWithPriority(numberList, ops);
        if (result == null || result < 0 || result > 100) return null;
        if (!ValidateIntermediates(numberList, ops)) return null;

        string display = BuildDisplayText(numberList, ops);

        if (_settings.IncludeTricks && _random.Next(4) == 0)
            return TryMakeTrick(numberList, ops, result.Value, display);

        var segments = _settings.EnableScratchWork && numCount > 2
            ? BuildFlatSegments(numberList, ops)
            : [];

        return new Exercise
        {
            FirstNumber = 0,
            SecondNumber = 0,
            Operation = "mixed",
            CorrectAnswer = result.Value,
            DisplayText = display,
            Segments = segments
        };
    }

    // ============================================================
    // SE ZÁVORKAMI — jednoduchý vzor: A op (B op C) nebo (A op B) op C
    // Závorka vždy obaluje právě 2 čísla s +/- uvnitř.
    // Operátor vedle závorky je buď × nebo ÷ (aby závorka měla smysl).
    // ============================================================

    private Exercise? TryGenerateWithParentheses(int numCount)
    {
        // Vnitřní operátor závorky: +/-
        char innerOp = _random.Next(2) == 0 ? '+' : '-';

        // Vnější operátor vedle závorky: × nebo ÷ (aby závorka měla smysl)
        char outerOp;
        if (_settings.IncludeMultiplication && _settings.IncludeDivision)
            outerOp = _random.Next(2) == 0 ? '×' : '÷';
        else if (_settings.IncludeMultiplication)
            outerOp = '×';
        else if (_settings.IncludeDivision)
            outerOp = '÷';
        else
            outerOp = '+';

        // Rozhodneme pozici závorky: parenFirst = true → (a op b) outerOp num
        bool parenFirst = _random.Next(2) == 0;

        // Klíčové pravidlo: u × musí být OBA operandy jednociferné (1–10).
        // U ÷ musí být DĚLITEL jednociferný.
        // → parenResult musí být ≤ 10 pokud:
        //   - outerOp je × (vždy)
        //   - outerOp je ÷ a závorka je dělitel (parenFirst == false)
        bool needSmallParenResult = outerOp == '×'
            || (outerOp == '÷' && !parenFirst);

        // Závorku: (A innerOp B)
        int a, b, parenResult;
        if (needSmallParenResult)
        {
            // Výsledek závorky musí být 1–10
            parenResult = _random.Next(1, 11);
            if (innerOp == '+')
            {
                // a + b = parenResult, a ≥ 1, b ≥ 1
                if (parenResult < 2) return null;
                a = _random.Next(1, parenResult);
                b = parenResult - a;
            }
            else
            {
                // a - b = parenResult, b ≥ 1
                b = _random.Next(1, 20);
                a = parenResult + b;
            }
        }
        else
        {
            // Závorka je dělenec u ÷ nebo operand u + → může být větší
            if (innerOp == '+')
            {
                a = _random.Next(1, 30);
                b = _random.Next(1, 30);
                parenResult = a + b;
            }
            else
            {
                a = _random.Next(5, 40);
                b = _random.Next(1, a);
                parenResult = a - b;
            }
        }
        if (parenResult <= 0 || parenResult > 100) return null;

        // Číslo vedle závorky (jednociferné pro × a ÷)
        int outerNum;
        if (outerOp == '×')
        {
            int maxMul = Math.Min(10, 100 / Math.Max(parenResult, 1));
            if (maxMul < 1) return null;
            outerNum = _random.Next(2, maxMul + 1); // min 2 aby násobení nebylo triviální
        }
        else if (outerOp == '÷')
        {
            if (parenFirst)
            {
                // parenResult ÷ outerNum — outerNum je dělitel (1–10)
                var divisors = Enumerable.Range(1, 10).Where(d => parenResult % d == 0 && d != parenResult).ToList();
                if (divisors.Count == 0) return null;
                outerNum = divisors[_random.Next(divisors.Count)];
            }
            else
            {
                // outerNum ÷ parenResult — parenResult je dělitel (už je ≤ 10)
                if (parenResult == 0) return null;
                int quotient = _random.Next(2, Math.Min(11, 100 / parenResult + 1));
                outerNum = quotient * parenResult;
                if (outerNum > 100) return null;
            }
        }
        else
        {
            outerNum = _random.Next(1, 40);
        }

        // Sestavíme výraz
        var numbers = new List<int>();
        var ops = new List<char>();
        int parenPos;

        if (parenFirst)
        {
            // (a innerOp b) outerOp outerNum [+ extra ...]
            numbers.Add(a);
            numbers.Add(b);
            ops.Add(innerOp);
            ops.Add(outerOp);
            numbers.Add(outerNum);
            parenPos = 0;
        }
        else
        {
            // outerNum outerOp (a innerOp b) [+ extra ...]
            numbers.Add(outerNum);
            ops.Add(outerOp);
            numbers.Add(a);
            numbers.Add(b);
            ops.Add(innerOp);
            parenPos = 1;
        }

        // Extra flat čísla (jen + a -)
        var availableOps = new List<char>();
        if (_settings.IncludeAddition) availableOps.Add('+');
        if (_settings.IncludeSubtraction) availableOps.Add('-');
        if (availableOps.Count == 0) availableOps.Add('+');

        for (int i = 3; i < numCount; i++)
        {
            ops.Add(availableOps[_random.Next(availableOps.Count)]);
            numbers.Add(_random.Next(1, 30));
        }

        // Vyhodnotit: závorku → nahradit → flat s prioritou
        var evalNumbers = new List<int>(numbers);
        evalNumbers[parenPos] = parenResult;
        evalNumbers.RemoveAt(parenPos + 1);
        var evalOps = new List<char>(ops);
        evalOps.RemoveAt(parenPos);

        int? result = EvaluateWithPriority(evalNumbers, evalOps);
        if (result == null || result < 0 || result > 100) return null;
        if (!ValidateIntermediates(evalNumbers, evalOps)) return null;

        string display = BuildDisplayTextWithParens(numbers, ops, parenPos);

        var segments = _settings.EnableScratchWork
            ? BuildParenSegments(numbers, ops, parenPos, parenResult)
            : [];

        return new Exercise
        {
            FirstNumber = 0,
            SecondNumber = 0,
            Operation = "mixed-parens",
            CorrectAnswer = result.Value,
            DisplayText = display,
            Segments = segments
        };
    }

    // ============================================================
    // HELPERS
    // ============================================================

    /// <summary>
    /// Vybere operátory s pravidlem: dva po sobě jdoucí nesmí být oba × nebo ÷.
    /// Bez tohoto pravidla by např. 10 × 2 × 5 vyžadovalo násobení 20 × 5.
    /// </summary>
    private List<char>? PickOperatorsNoConsecutiveMulDiv(int count)
    {
        var available = new List<char>();
        if (_settings.IncludeAddition) available.Add('+');
        if (_settings.IncludeSubtraction) available.Add('-');
        if (_settings.IncludeMultiplication) available.Add('×');
        if (_settings.IncludeDivision) available.Add('÷');
        if (available.Count == 0) return null;

        var addSub = available.Where(c => c is '+' or '-').ToList();
        var mulDiv = available.Where(c => c is '×' or '÷').ToList();

        var ops = new List<char>();
        for (int i = 0; i < count; i++)
        {
            bool prevWasMulDiv = i > 0 && ops[i - 1] is '×' or '÷';

            if (prevWasMulDiv)
            {
                // Po × nebo ÷ musí následovat + nebo -
                if (addSub.Count > 0)
                    ops.Add(addSub[_random.Next(addSub.Count)]);
                else
                    ops.Add('+'); // fallback
            }
            else
            {
                ops.Add(available[_random.Next(available.Count)]);
            }
        }
        return ops;
    }

    /// <summary>Vyhodnotí výraz s prioritou operací (× a ÷ před + a -).</summary>
    private static int? EvaluateWithPriority(List<int> numbers, List<char> ops)
    {
        if (numbers.Count == 0) return null;

        var nums = new List<int>(numbers);
        var opers = new List<char>(ops);

        // 1. fáze: vyhodnotit × a ÷
        for (int i = 0; i < opers.Count;)
        {
            if (opers[i] is '×' or '÷')
            {
                int left = nums[i];
                int right = nums[i + 1];
                int val;
                if (opers[i] == '×')
                {
                    val = left * right;
                }
                else
                {
                    if (right == 0) return null;
                    if (left % right != 0) return null;
                    val = left / right;
                }
                if (val < 0 || val > 100) return null;
                nums[i] = val;
                nums.RemoveAt(i + 1);
                opers.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        // 2. fáze: vyhodnotit + a - (zleva doprava)
        int result = nums[0];
        for (int i = 0; i < opers.Count; i++)
        {
            result = opers[i] == '+' ? result + nums[i + 1] : result - nums[i + 1];
            if (result < 0 || result > 100) return null;
        }

        return result;
    }

    private static bool ValidateIntermediates(List<int> numbers, List<char> ops)
    {
        var nums = new List<int>(numbers);
        var opers = new List<char>(ops);

        for (int i = 0; i < opers.Count;)
        {
            if (opers[i] is '×' or '÷')
            {
                int val;
                if (opers[i] == '×')
                    val = nums[i] * nums[i + 1];
                else
                {
                    if (nums[i + 1] == 0 || nums[i] % nums[i + 1] != 0) return false;
                    val = nums[i] / nums[i + 1];
                }
                if (val < 0 || val > 100) return false;
                nums[i] = val;
                nums.RemoveAt(i + 1);
                opers.RemoveAt(i);
            }
            else i++;
        }

        int result = nums[0];
        for (int i = 0; i < opers.Count; i++)
        {
            result = opers[i] == '+' ? result + nums[i + 1] : result - nums[i + 1];
            if (result < 0 || result > 100) return false;
        }

        return true;
    }

    private static string BuildDisplayText(List<int> numbers, List<char> ops)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append(numbers[0]);
        for (int i = 0; i < ops.Count; i++)
            sb.Append($" {ops[i]} {numbers[i + 1]}");
        return sb.ToString();
    }

    private static string BuildDisplayTextWithParens(List<int> numbers, List<char> ops, int parenPos)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < numbers.Count; i++)
        {
            if (i == parenPos + 1) continue;

            if (i > 0 && i != parenPos + 1)
            {
                int opIdx = i - 1;
                sb.Append($" {ops[opIdx]} ");
            }

            if (i == parenPos)
                sb.Append($"({numbers[i]} {ops[i]} {numbers[i + 1]})");
            else
                sb.Append(numbers[i]);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Scratch work pro flat výrazy.
    /// Mezivýpočet se zobrazí:
    ///   1) nad výsledkem každé × a ÷ operace (po vyhodnocení priority)
    ///   2) nad průběžným součtem/rozdílem + a − (po vyřešení × a ÷)
    /// Poslední krok (= konečný výsledek) scratch nedostane — to je odpověď.
    /// </summary>
    private List<ExerciseSegment> BuildFlatSegments(List<int> numbers, List<char> ops)
    {
        // --- 1) Zjistíme scratch pro × a ÷ ---
        // Mapujeme původní index operátoru → mezivýsledek
        var mulDivScratch = new Dictionary<int, int>();
        {
            var nums = new List<int>(numbers);
            var opers = new List<char>(ops);
            // Mapování: originální index → aktuální pozice (posunuje se odstraňováním)
            var origIndices = Enumerable.Range(0, ops.Count).ToList();

            for (int i = 0; i < opers.Count;)
            {
                if (opers[i] is '×' or '÷')
                {
                    int val = opers[i] == '×' ? nums[i] * nums[i + 1] : nums[i] / nums[i + 1];
                    mulDivScratch[origIndices[i]] = val;
                    nums[i] = val;
                    nums.RemoveAt(i + 1);
                    opers.RemoveAt(i);
                    origIndices.RemoveAt(i);
                }
                else i++;
            }
        }

        // --- 2) Zjistíme scratch pro + a − (po vyřešení × a ÷) ---
        // Po fázi 1 zbývají jen + a − na „zredukovaném" výrazu.
        // Spočteme průběžné mezisoučty. Scratch dáme na pozici
        // původního operátoru pro + / − (kromě posledního, to je odpověď).
        var addSubScratch = new Dictionary<int, int>(); // orig op index → mezivýsledek
        {
            var nums2 = new List<int>(numbers);
            var opers2 = new List<char>(ops);
            var origIndices2 = Enumerable.Range(0, ops.Count).ToList();

            // Vyřešit × a ÷
            for (int i = 0; i < opers2.Count;)
            {
                if (opers2[i] is '×' or '÷')
                {
                    int val = opers2[i] == '×' ? nums2[i] * nums2[i + 1] : nums2[i] / nums2[i + 1];
                    nums2[i] = val;
                    nums2.RemoveAt(i + 1);
                    opers2.RemoveAt(i);
                    origIndices2.RemoveAt(i);
                }
                else i++;
            }

            // Průběžné součty pro + a −
            if (opers2.Count >= 2) // scratch jen pokud zbývají aspoň 2 kroky
            {
                int running = nums2[0];
                for (int i = 0; i < opers2.Count; i++)
                {
                    running = opers2[i] == '+' ? running + nums2[i + 1] : running - nums2[i + 1];
                    // Přeskočit scratch posledního kroku — to je konečná odpověď
                    if (i < opers2.Count - 1)
                        addSubScratch[origIndices2[i]] = running;
                }
            }
        }

        // --- 3) Sestavíme segmenty ---
        var segments = new List<ExerciseSegment>();
        segments.Add(new ExerciseSegment { Text = numbers[0].ToString() });

        for (int i = 0; i < ops.Count; i++)
        {
            segments.Add(new ExerciseSegment { Text = $" {ops[i]} " });

            int? scratch = null;
            if (mulDivScratch.TryGetValue(i, out int mdVal))
                scratch = mdVal;
            else if (addSubScratch.TryGetValue(i, out int asVal))
                scratch = asVal;

            segments.Add(new ExerciseSegment { Text = numbers[i + 1].ToString(), ScratchValue = scratch });
        }

        return segments;
    }

    private static List<ExerciseSegment> BuildParenSegments(List<int> numbers, List<char> ops, int parenPos, int parenResult)
    {
        var segments = new List<ExerciseSegment>();

        for (int i = 0; i < numbers.Count; i++)
        {
            if (i > 0 && i != parenPos + 1)
                segments.Add(new ExerciseSegment { Text = $" {ops[i - 1]} " });

            if (i == parenPos)
            {
                segments.Add(new ExerciseSegment
                {
                    Text = $"({numbers[i]} {ops[i]} {numbers[i + 1]})",
                    ScratchValue = parenResult
                });
            }
            else if (i == parenPos + 1)
            {
                continue;
            }
            else
            {
                segments.Add(new ExerciseSegment { Text = numbers[i].ToString() });
            }
        }

        return segments;
    }

    private Exercise? TryMakeTrick(List<int> numbers, List<char> ops, int result, string display)
    {
        int trickPos = _random.Next(numbers.Count);
        int missingValue = numbers[trickPos];

        var trickNumbers = new List<string>(numbers.Select(n => n.ToString()));
        trickNumbers[trickPos] = "_";

        var sb = new System.Text.StringBuilder();
        sb.Append(trickNumbers[0]);
        for (int i = 0; i < ops.Count; i++)
            sb.Append($" {ops[i]} {trickNumbers[i + 1]}");
        sb.Append($" = {result}");

        return new Exercise
        {
            FirstNumber = 0,
            SecondNumber = 0,
            Operation = "mixed-trick",
            CorrectAnswer = missingValue,
            DisplayText = sb.ToString(),
            Segments = []
        };
    }

    private Exercise FallbackExercise()
    {
        int a = _random.Next(10, 50);
        int b = _random.Next(1, 10);
        return new Exercise
        {
            FirstNumber = a,
            SecondNumber = b,
            Operation = "+",
            CorrectAnswer = a + b,
            DisplayText = $"{a} + {b}",
            Segments = []
        };
    }
}
