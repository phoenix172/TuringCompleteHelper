using static TuringCompleteHelper.Tests.TestHelper;

namespace TuringCompleteHelper.Tests;

public class VectorOperationsTests
{
    [TestCase("[61257709769163094].Wedge([-10517511624487791])", "#775CA1C8")]
    [TestCase("[#ffdf000000000000].Wedge([#fffc0000fffb0000])", "#FF5B0000")] // Wedge - tick 39
    public Task Wedge(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("[5]+[5]", "[10]")]
    [TestCase("[5,5]+[5,17]", "[10,22]")]
    public Task Add(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("[5]*[5]", "[25]")]
    public Task Multiply(string expression, string expectedResult) => Verify(expression, expectedResult);
    
    [TestCase("FixedPointVector.Area([15199648742965248], [7883498371547648], [16325548650135552])", "#75fe00")]
    [TestCase("FixedPointVector.Area([7036874418683904], [15199648742965248], [16325548650135552])", "#FF5B0000")] // Wedge - tick 39
    [TestCase("FixedPointVector.Area([7036874418683904], [7883498371547648], [15199648742965248])", "#ff274400")]
    public Task Area(string expression, string expectedResult) => Verify(expression, expectedResult);
}