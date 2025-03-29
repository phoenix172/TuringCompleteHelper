using static TuringCompleteHelper.Tests.TestHelper;

namespace TuringCompleteHelper.Tests;

public class ParsingTests
{
    [TestCase("[5]", "[#00050000]")]
    [TestCase("[-10517511624487791]", "[#ffdaa260, #FFb38c91]")]
    public Task ParseVector(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("5", "#00050000")]
    public Task ParseNumber(string expression, string expectedResult) => Verify(expression, expectedResult);
}