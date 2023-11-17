using System.Collections.Generic;

namespace X3Code.Utils.Convert;

/// <summary>
/// A converter for converting decimal numbers to romanian numerals and back
/// </summary>
public static class RomanianConverter
{
    #region Constant Values (Romanian numbers)

    private static readonly Dictionary<char, int> CharValues = new()
    {
        {'I', 1},
        {'V', 5},
        {'X', 10},
        {'L', 50},
        {'C', 100},
        {'D', 500},
        {'M', 1000}
    };
    private static readonly string[] ThouLetters = { "", "M", "MM", "MMM" };
    private static readonly string[] HundLetters = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
    private static readonly string[] TensLetters = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
    private static readonly string[] OnesLetters = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

    #endregion

    /// <summary>
    /// Converts the given romanian number into arabic system
    /// For numbers bigger than 1000 set the thousands in () like: (V)CVI
    /// </summary>
    /// <param name="romanInput">The romanian number</param>
    /// <returns>The converted number</returns>
    public static int RomanToArabic(string romanInput)
    {
        if (romanInput.Length == 0) return 0;
        romanInput = romanInput.ToUpper();

        if (romanInput[0] == '(')
        {
            var position = romanInput.LastIndexOf(')');

            var part1 = romanInput.Substring(1, position - 1);
            var part2 = romanInput.Substring(position + 1);
            return 1000 * RomanToArabic(part1) + RomanToArabic(part2);
        }

        var total = 0;
        var lastValue = 0;
        for (var i = romanInput.Length - 1; i >= 0; i--)
        {
            int newValue = CharValues[romanInput[i]];

            if (newValue < lastValue)
                total -= newValue;
            else
            {
                total += newValue;
                lastValue = newValue;
            }
        }

        return total;
    }

    /// <summary>
    /// Converts the given arabic number into romanian system
    /// For numbers bigger than 1000 the thousands will be written in () like: (V)CVI
    /// </summary>
    /// <param name="arabicInput">The arabic number</param>
    /// <returns>The converted romanian number</returns>
    public static string ArabicToRoman(int arabicInput)
    {
        if (arabicInput >= 4000)
        {
            var thou = arabicInput / 1000;
            arabicInput %= 1000;
            return "(" + ArabicToRoman(thou) + ")" + ArabicToRoman(arabicInput);
        }

        var result = string.Empty;

        var number = arabicInput / 1000;
        result += ThouLetters[number];
        arabicInput %= 1000;

        number = arabicInput / 100;
        result += HundLetters[number];
        arabicInput %= 100;

        number = arabicInput / 10;
        result += TensLetters[number];
        arabicInput %= 10;

        result += OnesLetters[arabicInput];

        return result;
    }
}