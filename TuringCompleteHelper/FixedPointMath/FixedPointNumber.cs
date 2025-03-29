using System.Globalization;
using System.Numerics;

namespace TuringCompleteHelper.FixedPointMath;

public readonly record struct FixedPointNumber
{
    public static readonly FixedPointNumber MinValue = new("80000000");
    public static readonly FixedPointNumber MaxValue = new("7FFFFFFF");
    public static readonly FixedPointNumber Zero = new(0);

    public FixedPointNumber(string hexValue)
        : this(double.Parse(FixedPointNumberConverter.ConvertQ16_16ToDoubleString(hexValue)))
    {
    }

    public FixedPointNumber(double value)
    {
        Hex = FixedPointNumberConverter.ConvertDoubleStringToQ16_16(value.ToString("00000.00000000000"));
        Value = double.Parse(FixedPointNumberConverter.ConvertQ16_16ToDoubleString(Hex));
        SignedValue = int.Parse($"{Hex}", NumberStyles.HexNumber);
    }

    public double Value { get; }

    public string Hex { get; }

    private int SignedValue { get; }

    public FixedPointNumber Multiply(FixedPointNumber other)
    {
        var multiply = ((long)int.Parse(Hex, NumberStyles.HexNumber) *
                        (long)int.Parse(other.Hex, NumberStyles.HexNumber));
        short roundingByte = (short)(multiply & (short)-1);
        var multiplyShifted = multiply >> 16;
        int increment = multiply < 0 ? 1 : -1;
        if ((roundingByte < -1 && multiply < 0) || (roundingByte != 0 && roundingByte > -1 && multiply > 0)) 
            multiplyShifted += increment;
        string hex = Convert.ToString(Clamp(multiplyShifted), 16).PadLeft(8, '0');
        return new(hex[^8..]);
    }

    public FixedPointNumber Divide(FixedPointNumber divisor)
    {
        var divide = ((long)int.Parse(Hex, NumberStyles.HexNumber) << 16) /
                        int.Parse(divisor.Hex, NumberStyles.HexNumber);
        string hex = Convert.ToString(Clamp(divide), 16).PadLeft(8, '0');
        return new(hex[^8..]);
    }

    public FixedPointNumber Add(FixedPointNumber other) => new(Clamp(Value + other.Value));

    public FixedPointNumber Subtract(FixedPointNumber other) => new(Clamp(Value - other.Value));

    public override string ToString()
    {
        return $"{Hex}({Value} : {SignedValue})";
    }

    public static FixedPointNumber[] ParseMany(string value)
    {
        BigInteger bigInt;
        
        if (value.FirstOrDefault() == '#')
        {
            bigInt = BigInteger.Parse(value[1..], NumberStyles.HexNumber);
        }
        else
        {
            bigInt = BigInteger.Parse(value);
        }

        if (bigInt.GetByteCount() <= 4)
            return [FixedPointNumber.Parse(value)];
        
        var padding = Enumerable.Repeat(bigInt.Sign == -1 ? (byte)255 : (byte)0, (4 - bigInt.GetByteCount() % 4)%4);

        var numbers = padding
            .Concat(bigInt.ToByteArray().Reverse())
            .Chunk(4);
        var fixedPointNumbers = numbers.Select(x =>
        {
            var hex = BitConverter.ToString(x).Replace("-", "");
            return new FixedPointNumber(hex);
        }).ToArray();
        return fixedPointNumbers;
    }

    public static FixedPointNumber Parse(string value)
    {
        if (value.FirstOrDefault() == '#')
            return new FixedPointNumber(value[1..]);
        return new FixedPointNumber(double.Parse(value));
    }

    public static FixedPointNumber operator +(FixedPointNumber a, FixedPointNumber b) => a.Add(b);

    public static FixedPointNumber operator -(FixedPointNumber a, FixedPointNumber b) => a.Subtract(b);

    public static FixedPointNumber operator -(FixedPointNumber a) => Zero.Subtract(a);

    public static FixedPointNumber operator *(FixedPointNumber a, FixedPointNumber b) => a.Multiply(b);

    public static FixedPointNumber operator /(FixedPointNumber a, FixedPointNumber b) => a.Divide(b);
    public static double Clamp(double value) => Math.Min(Math.Max(value, MinValue.Value), MaxValue.Value);
    public static long Clamp(long value) => Math.Min(Math.Max(value, MinValue.SignedValue), MaxValue.SignedValue);
}