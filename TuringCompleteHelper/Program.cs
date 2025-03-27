using System.Collections.Immutable;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using TuringCompleteHelper.FixedPointMath;

namespace TuringCompleteHelper;

public class Program
{
	public static async Task Main()
	{
		string expression = default;
		do
		{
			expression = Console.ReadLine();
			try
			{
				var result = await Evaluate(expression);
				Console.WriteLine(result?.ToString());
			}
			catch(Exception ex)
			{
				Console.WriteLine($"Failed to execute script: {ex.Message}");
			}

		} while (expression.ToLower() != "exit");
	}

	public static async Task<object> Evaluate(string expression)
	{
		var tokens = Tokenize(expression);
		
		var processedExpression = ProcessTokens(tokens);
		
		var result = await CSharpScript.EvaluateAsync(
			processedExpression,
			ScriptOptions.Default
				.WithReferences(typeof(FixedPointNumber).Assembly)
				.WithImports(typeof(FixedPointNumber).Namespace));

		return result;
	}

	private static string ProcessTokens(List<string> tokens)
	{
		Regex hexRegex = new Regex("^#[0-9A-Fa-f]+$");
		Regex decimalRegex = new Regex("^[0-9]+(.[0-9]+)?$");
		Regex vectorRegex = new Regex($"^\\[({hexRegex.ToString().Trim('$', '^')}|{decimalRegex.ToString().Trim('$', '^')}|,)+\\]$");
		
		var replacedNumericLiterals = tokens.Select(token =>
		{
			if (vectorRegex.IsMatch(token))
			{
				var vectorTokens = token.Split(["],[","[", "]"], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
					.Select(x =>
					{
						if (FixedPointVector.TryParse($"[{x}]", out var _))
							return $"{nameof(FixedPointVector)}.{nameof(FixedPointVector.Parse)}(\"[{x}]\")";
						else
							return x;
					});
				return string.Join(",", vectorTokens);
			}
			if (hexRegex.IsMatch(token) || decimalRegex.IsMatch(token))
			{
				return $"{nameof(FixedPointNumber)}.{nameof(FixedPointNumber.Parse)}(\"{token}\")";
			}
			else
			{
				return token;
			}
		});
		var processedExpression = string.Join("", replacedNumericLiterals);
		return processedExpression;
	}

	public static List<string> Tokenize(string expression)
	{
		expression = expression.Replace(" ", "");
		var operators = new[] { '+', '*', 'x', '/', '(', ')' }.ToImmutableHashSet();
		List<string> tokens = new();
		StringBuilder termBuilder = new StringBuilder();
		foreach (var c in expression.Index())
		{
			if (operators.Contains(c.Item) || (c.Item == '.' && !char.IsDigit(expression[c.Index+1]) && expression[c.Index+1]!= '#'))
			{
				tokens.Add(termBuilder.ToString());
				tokens.Add(c.Item.ToString());
				termBuilder.Clear();
			}
			else
			{
				termBuilder.Append(c.Item);
			}
		}

		if (termBuilder.Length > 0)
			tokens.Add(termBuilder.ToString());
		
		return tokens;
	}
	
	// public static FixedPointNumber[] Get(string expression)
	// {
	// 	char[] operators = ['+','*','x','/'];
	// 	var operands = expression.Split([..operators,' '], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
	// 	if (operands.Length > 2) throw new InvalidOperationException();
	// 	char operation = expression.First(x => operators.Contains(x));
	// 	var result = operation switch
	// 	{
	// 		'+' => FixedPointNumber.TryParseVector(operands[0], out var a) && FixedPointNumber.TryParseVector(operands[1], out var b) 
	// 			? a.Zip(b).Select(x => x.First + x.Second).ToArray()
	// 			: [FixedPointNumber.Parse(operands[0]) + FixedPointNumber.Parse(operands[1])],
	// 		'*' => FixedPointNumber.TryParseVector(operands[0], out var a) && FixedPointNumber.TryParseVector(operands[1], out var b) 
	// 			? a.Zip(b).Select(x => x.First * x.Second).ToArray()
	// 			: [FixedPointNumber.Parse(operands[0]) * FixedPointNumber.Parse(operands[1])],
	// 		'/' => FixedPointNumber.TryParseVector(operands[0], out var a) && FixedPointNumber.TryParseVector(operands[1], out var b) 
	// 			? a.Zip(b).Select(x => x.First / x.Second).ToArray()
	// 			: [FixedPointNumber.Parse(operands[0]) / FixedPointNumber.Parse(operands[1])],
	// 		'x' => FixedPointNumber.TryParseVector(operands[0], out var a) && FixedPointNumber.TryParseVector(operands[1], out var b) 
	// 			? [FixedPointNumber.Dot(a, b)] 
	// 			: throw new ArgumentException(),
	// 		_ => throw new InvalidOperationException()
	// 	};
	// 	return result;
	// }
}