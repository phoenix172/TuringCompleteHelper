
namespace TuringCompleteHelper;

public class Program
{
	public static async Task Main()
	{
		var evaluator = new ExpressionEvaluator();
		string? expression;
		do
		{
			expression = Console.ReadLine();
			if(expression == null) continue;
			try
			{
				var result = await evaluator.Evaluate(expression);
				Console.WriteLine(result.ToString());
			}
			catch(Exception ex)
			{
				Console.WriteLine($"Failed to execute script: {ex.Message}");
			}

		} while (expression?.ToLower() != "exit");
	}
}