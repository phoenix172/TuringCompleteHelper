using static TuringCompleteHelper.Tests.TestHelper;

namespace TuringCompleteHelper.Tests;

public class NumberOperationsTests
{
    [TestCase("5+5", "10")]
    public Task Add(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("5*5", "25")]
    public Task Multiply(string expression, string expectedResult) => Verify(expression, expectedResult);
}