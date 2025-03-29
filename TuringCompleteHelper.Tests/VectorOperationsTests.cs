using static TuringCompleteHelper.Tests.TestHelper;

namespace TuringCompleteHelper.Tests;

public class VectorOperationsTests
{
    [TestCase("[61257709769163094].Wedge([-10517511624487791])", "#775CA1C8")]
    public Task Wedge(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("[5]+[5]", "[10]")]
    public Task Add(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("[5]*[5]", "[25]")]
    public Task Multiply(string expression, string expectedResult) => Verify(expression, expectedResult);
}