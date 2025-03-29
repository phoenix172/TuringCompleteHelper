using static TuringCompleteHelper.Tests.TestHelper;

namespace TuringCompleteHelper.Tests;

public class NumberOperationsTests
{
    [TestCase("5+5", "10")]
    public Task Add(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("5*5", "25")]
    [TestCase("#fe8b8156 * #ffdaa260", "#365e8964")] // Wedge product - tick 17
    [TestCase("#ffb38c91 * #d9a18f", "#bf01e79c")] // Wedge product - tick 17
    public Task Multiply(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("5/5", "1")]
    [TestCase("#FF96D44B / #02A1DC59", "#FFFFD80C")] // Fixed-Point Division - tick 20
    [TestCase("#00182358 / #018DCFF7", "#00000F88")] // Fixed-Point Division - tick 69
    public Task Divide(string expression, string expectedResult) => Verify(expression, expectedResult);
}