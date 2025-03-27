using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using TuringCompleteHelper.FixedPointMath;

namespace TuringCompleteHelper;

public class ExpressionEvaluator
{
    public async Task<object> Evaluate(string expression)
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
        Regex vectorRegex =
            new Regex($"^\\[({hexRegex.ToString().Trim('$', '^')}|{decimalRegex.ToString().Trim('$', '^')}|,)+\\]$");

        var replacedNumericLiterals = tokens.Select(token =>
        {
            if (vectorRegex.IsMatch(token))
            {
                var vectorTokens = token.Split(["],[", "[", "]"],
                        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
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

    private static List<string> Tokenize(string expression)
    {
        expression = expression.Replace(" ", "");
        var operators = new[] { '+', '*', 'x', '/', '(', ')' }.ToImmutableHashSet();
        List<string> tokens = new();
        StringBuilder termBuilder = new StringBuilder();
        foreach (var c in expression.Index())
        {
            if (operators.Contains(c.Item) || (c.Item == '.' && !char.IsDigit(expression[c.Index + 1]) &&
                                               expression[c.Index + 1] != '#'))
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
}