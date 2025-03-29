using TuringCompleteHelper.FixedPointMath;

namespace TuringCompleteHelper.Tests;

public class TestHelper
{
    public static async Task Verify(string expression, string expectedResult)
    {
        ExpressionEvaluator evaluator = new();
        var result = await evaluator.Evaluate(expression);
        if(result is FixedPointVector vectorResult && FixedPointVector.TryParse(expectedResult, out var expectedVectorResult))
            Assert.That(vectorResult.Values, Is.EqualTo(expectedVectorResult.Values).AsCollection);
        else
            Assert.That(result, Is.EqualTo(FixedPointNumber.Parse(expectedResult)));
    }
}