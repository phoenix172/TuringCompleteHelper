namespace TuringCompleteHelper.Tests;

public class FixedPointNumberConverterTests
{
    [TestCase("788B9E5B")]
    [TestCase("99CA6A61")]
    public void HexToDouble(string hex)
    {
        var result = FixedPointNumberConverter.ConvertQ16_16ToDoubleString(hex);
        var expected = FixedPointNumberConverter.ConvertDoubleStringToQ16_16(result);
        Assert.That(expected, Is.EqualTo(hex));
    }
    
    [TestCase("28033.49920624648")]
    [TestCase("30859.61857568242")]
    public void DoubleToHex(string doubleString)
    {
        var result = FixedPointNumberConverter.ConvertDoubleStringToQ16_16(doubleString);
        var expected = FixedPointNumberConverter.ConvertQ16_16ToDoubleString(result);
        Assert.That(expected, Is.EqualTo(doubleString));
    }
}