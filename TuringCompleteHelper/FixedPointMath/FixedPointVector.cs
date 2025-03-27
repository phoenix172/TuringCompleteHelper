namespace TuringCompleteHelper.FixedPointMath;

public readonly record struct FixedPointVector(params FixedPointNumber[] Values)
{
    public static readonly FixedPointVector Zero = new(new FixedPointNumber(0));
    
    public static FixedPointVector Parse(string value)
    {
        var vector = value.Split(['[', ']', ',', ' '], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(FixedPointNumber.Parse)
            .ToArray();
        return new FixedPointVector(vector);
    }
    
    public static bool TryParse(string value, out FixedPointVector vector)
    {
        if (value.First() != '[' || value.Last() != ']')
        {
            vector = default;
            return false;
        }

        vector = Parse(value);
        return true;
    }

    public override string ToString()
    {
        return $"[ {string.Join(", ", Values)} ]";
    }

    public FixedPointVector Multiply(FixedPointVector other) => new(Values.Zip(other.Values).Select(x => x.First * x.Second).ToArray());
    
    public FixedPointVector Multiply(FixedPointNumber other) => new(Values.Select(x => x * other).ToArray());
    
    public FixedPointVector Divide(FixedPointVector divisor) => new(Values.Zip(divisor.Values).Select(x => x.First / x.Second).ToArray());
    
    public FixedPointVector Divide(FixedPointNumber divisor) => new(Values.Select(x => x / divisor).ToArray());

    public FixedPointVector Add(FixedPointVector other) => new(Values.Zip(other.Values).Select(x => x.First + x.Second).ToArray());
    
    public FixedPointVector Add(FixedPointNumber other) => new(Values.Select(x => x + other).ToArray());
    
    public FixedPointNumber Dot(FixedPointVector other) => Values.Zip(other.Values).Aggregate(FixedPointNumber.Zero, (result, x) => result + (x.First * x.Second));
    
    public FixedPointNumber Wedge(FixedPointVector other) => Values.Zip(other.Values).Aggregate(FixedPointNumber.Zero, (result, x) => result + (x.First * x.Second));
    
    public static FixedPointVector operator +(FixedPointVector a, FixedPointVector b) => a.Add(b);

    public static FixedPointVector operator *(FixedPointVector a, FixedPointVector b) => a.Multiply(b);

    public static FixedPointVector operator /(FixedPointVector a, FixedPointVector b) => a.Divide(b);
    
    public static FixedPointVector operator +(FixedPointVector a, FixedPointNumber b) => a.Add(b);

    public static FixedPointVector operator *(FixedPointVector a, FixedPointNumber b) => a.Multiply(b);

    public static FixedPointVector operator /(FixedPointVector a, FixedPointNumber b) => a.Divide(b);
}