using System.Globalization;
using Microsoft.VisualBasic;

namespace TuringCompleteHelper;

public class FixedPointNumberConverter
{
    /// <summary>
    /// Converts a 32-bit Q16.16 fixed‑point value (given as an 8‑digit hex string)
    /// to a formatted string with 5 digits for the integer part and 11 digits for the fraction.
    /// For example, "FE7CA8C1" converts to "-00387.34080544522".
    /// </summary>
    public static string ConvertQ16_16ToString(string hexValue)
    {
        // Parse the hex string into a 32-bit unsigned value.
        uint ua = Convert.ToUInt32(hexValue, 16);
        // The Q16.16 fixed‑point number is stored in two’s complement.
        // The conversion splits the bits into an "upper" part and a "fraction" part.
        int upperPart = (int)((ua & 0x7FFF0000) >> 16);  // using only 15 bits for the magnitude
        long scaled = (long)upperPart * ScaleInteger;
        scaled += ((long)(ua & 0xFFFF)) * FractionMultiplier;
        // If the sign bit is set, adjust by subtracting the fixed offset.
        if ((ua & 0x80000000) != 0)
        {
            scaled -= NegativeOffset;
        }
        // Format the result: split into integer and fractional portions.
        bool isNegative = scaled < 0;
        long absScaled = Math.Abs(scaled);
        long intPart = absScaled / ScaleInteger;
        long fracPart = absScaled % ScaleInteger;
        // Format with 5 digits before the decimal point and 11 digits after.
        string formatted = (isNegative ? "-" : "") +
                           intPart.ToString("D5") + "." +
                           fracPart.ToString("D11");
        return formatted;
    }

    /// <summary>
    /// Inverse conversion: Given a formatted Q16.16 string (e.g. "-00387.34080544522"),
    /// reconstruct the 32-bit fixed‑point value as an 8‑digit hex string.
    /// </summary>
    public static string ConvertStringToQ16_16(string formatted)
    {
        if (string.IsNullOrEmpty(formatted) || formatted.Length < 17)
            throw new FormatException("Input string is not in the expected format.");

        bool isNegative = formatted.StartsWith("-");
        // Remove the sign (if present) and split at the decimal point.
        string s = isNegative ? formatted.Substring(1) : formatted;
        string[] parts = s.Split('.');
        if (parts.Length != 2)
            throw new FormatException("Input must have exactly one decimal point.");

        long intPart = long.Parse(parts[0]);    // e.g. "00387" becomes 387
        long fracPart = long.Parse(parts[1]);     // e.g. "34080544522"

        // Reconstruct the scaled value as in the forward conversion.
        long absScaled = intPart * ScaleInteger + fracPart;
        long S = isNegative ? -absScaled : absScaled;
        // In the forward conversion, if the number was negative, we subtracted NegativeOffset.
        // Here add it back to recover the adjusted scaled value.
        long S_adj = (S < 0) ? (S + NegativeOffset) : S;

        // Now, S_adj = A * ScaleInteger + F * FractionMultiplier.
        long A = S_adj / ScaleInteger;
        long remainder = S_adj - (A * ScaleInteger);
        long F = remainder / FractionMultiplier;

        // Check that A and F are within range.
        if (A < 0 || A > 0x7FFF)
            throw new OverflowException("Integer part out of range for Q16.16 fixed‑point.");
        if (F < 0 || F > 0xFFFF)
            throw new OverflowException("Fractional part out of range for Q16.16 fixed‑point.");

        // Reassemble the 32-bit fixed‑point value:
        uint ua = ((uint)A << 16) | ((uint)F & 0xFFFF);
        // For negative numbers, set the sign bit.
        if (isNegative)
        {
            ua |= 0x80000000;
        }
        return ua.ToString("X8");
    }

    /// <summary>
    /// Helper: Converts a formatted string (like "-00387.34080544522") into its internal 32-bit fixed‑point representation.
    /// </summary>
    public static int ConvertFormattedToFixed(string formatted)
    {
        string hex = ConvertStringToQ16_16(formatted);
        uint u = uint.Parse(hex, NumberStyles.HexNumber);
        int fixedVal = unchecked((int)u);
        return fixedVal;
    }

    /// <summary>
    /// Saturating addition: adds two Q16.16 fixed‑point values (32-bit) with saturation.
    /// </summary>
    public static int SaturatingAdd(int a, int b)
    {
        long sum = (long)a + (long)b;
        if (sum > int.MaxValue) return int.MaxValue;
        if (sum < int.MinValue) return int.MinValue;
        return (int)sum;
    }

    /// <summary>
    /// Saturating multiplication: multiplies two Q16.16 fixed‑point numbers.
    /// The product is computed as (a * b) / 65536.
    /// </summary>
    public static int SaturatingMultiply(int a, int b)
    {
        long product = ((long)a * (long)b) / 65536L;
        if (product > int.MaxValue) return int.MaxValue;
        if (product < int.MinValue) return int.MinValue;
        return (int)product;
    }

    /// <summary>
    /// Adds two formatted Q16.16 numbers.
    /// The inputs and result are given in the formatted string form (e.g. "-00387.34080544522").
    /// </summary>
    public static string AddFixed(string formattedA, string formattedB)
    {
        int a = ConvertFormattedToFixed(formattedA);
        int b = ConvertFormattedToFixed(formattedB);
        int sum = SaturatingAdd(a, b);
        // Convert back: get hex string and then format.
        string hexSum = ((uint)sum).ToString("X8");
        return ConvertQ16_16ToString(hexSum);
    }

    /// <summary>
    /// Multiplies two formatted Q16.16 numbers.
    /// The inputs and result are given in the formatted string form.
    /// </summary>
    public static string MultiplyFixed(string formattedA, string formattedB)
    {
        int a = ConvertFormattedToFixed(formattedA);
        int b = ConvertFormattedToFixed(formattedB);
        int prod = SaturatingMultiply(a, b);
        string hexProd = ((uint)prod).ToString("X8");
        return ConvertQ16_16ToString(hexProd);
    }
    
    public static string ConvertDoubleStringToQ16_16(string formatted)
    {
        if (string.IsNullOrEmpty(formatted) || formatted.Length < 17)
            throw new FormatException("Input string is not in the expected format.");

        // Determine if the number is negative.
        bool isNegative = formatted.StartsWith("-");
        
        // Remove the sign (if any) and then split the string at the decimal point.
        string s = isNegative ? formatted.Substring(1) : formatted;
        string[] parts = s.Split('.');
        if(parts.Length != 2)
            throw new FormatException("Input must have exactly one decimal point.");
        
        // Parse the integer and fractional parts.
        // The integer part is expected to have 5 digits and the fractional part 11 digits.
        long intPart = long.Parse(parts[0]);    // e.g. 00387 -> 387
        long fracPart = long.Parse(parts[1]);     // e.g. 34080544522
        
        // Reconstruct the scaled absolute value.
        // This value is what was obtained (in absolute value) in the forward conversion.
        long absScaled = intPart * ScaleInteger + fracPart;
        // Reapply the sign.
        long S = isNegative ? -absScaled : absScaled;
        
        // In the forward conversion, if the original value was negative, we subtracted NegativeOffset.
        // Here, we add it back to recover the "adjusted" scaled value.
        long S_adj = (S < 0) ? (S + NegativeOffset) : S;
        
        // Now S_adj = A * ScaleInteger + F * FractionMultiplier, where:
        //   A = (upper 15 bits) and F = (lower 16 bits).
        long A = S_adj / ScaleInteger;
        long remainder = S_adj - (A * ScaleInteger);
        // It should divide exactly, but in case of rounding we use integer division.
        long F = remainder / FractionMultiplier;
        
        // A must fit in 15 bits and F in 16 bits.
        if (A < 0 || A > 0x7FFF)
            throw new OverflowException("Integer part out of range for Q16.16 fixed‑point.");
        if (F < 0 || F > 0xFFFF)
            throw new OverflowException("Fractional part out of range for Q16.16 fixed‑point.");
        
        // Reassemble the 32‑bit fixed‑point number.
        uint ua = ((uint)A << 16) | ((uint)F & 0xFFFF);
        // For negative numbers, set the sign bit.
        if (isNegative)
        {
            ua |= 0x80000000;
        }
        // Return the 8‑digit hex representation.
        return ua.ToString("X8");
    }
    
    const long ScaleInteger = 100000000000L;   // 1e11
    const long FractionMultiplier = 1525878L;
    const long NegativeOffset = 3276800000000000L;
    
    public static string ConvertQ16_16ToDoubleString(string hexValue)
    {
        // Interpret the hex string as an unsigned 32-bit number.
        uint ua = Convert.ToUInt32(hexValue, 16);
        // Extract the upper part (mask with 0x7fff0000) then shift right 16 bits.
        // This gives the positive integer part (ignoring the sign bit).
        int upperPart = (int)((ua & 0x7FFF0000) >> 16);
        
        // Start with the integer part scaled by 100000000000.
        long scaled = (long)upperPart * 100000000000L;
        // Add the fractional part: low 16 bits multiplied by 1525878.
        scaled += ((long)(ua & 0xFFFF)) * 1525878L;
        // If the number is negative (i.e. the sign bit is set), subtract 3276800000000000.
        if ((ua & 0x80000000) != 0)
        {
            scaled -= 3276800000000000L;
        }
        
        // The scaled value represents the fixed-point number multiplied by 10^11.
        // We now want to format it as: sign, 5-digit integer part, a dot, and 11-digit fractional part.
        bool isNegative = scaled < 0;
        long absScaled = Math.Abs(scaled);
        long intPart = absScaled / 100000000000L;   // 10^11
        long fracPart = absScaled % 100000000000L;
        
        // Format the output string:
        // The integer part is padded to 5 digits and the fractional part to 11 digits.
        string formatted = (isNegative ? "-" : "") +
                           intPart.ToString("D5") + "." +
                           fracPart.ToString("D11");
        return formatted;
    }
}