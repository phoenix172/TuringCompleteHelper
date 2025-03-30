using static TuringCompleteHelper.Tests.TestHelper;

namespace TuringCompleteHelper.Tests;

public class ParsingTests
{
    [TestCase("[5]", "[#00050000]")]
    [TestCase("[-10517511624487791]", "[#ffdaa260, #FFb38c91]")]
    [TestCase("[-3201696213298670931]", "[#d3914a41, #df9e92ad]")]
    [TestCase("[8686210422573525601]", "[#788b9e5b, #99ca6a61]")]
    public Task ParseVector(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("[8686210422573525601].Int()", "8686210422573525601")]
    [TestCase("[#788b9e5b, #99ca6a61].Int()", "8686210422573525601")]
    public Task VectorToInt(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("5", "#00050000")]
    public Task ParseNumber(string expression, string expectedResult) => Verify(expression, expectedResult);
}