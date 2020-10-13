using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Coimbra.Editor
{
    [InitializeOnLoad]
    internal static class CSExpressionEvaluator
    {
        private const char OptionalExpressionChar = '?';
        private const string ExpressionIsNullOrWhiteSpaceMessage = "Expression is null or white space.";
        private const string InvalidExpressionMessageFormat = "Invalid expression \"{0}\"";
        private static readonly CodingSeb.ExpressionEvaluator.ExpressionEvaluator Instance;

        static CSExpressionEvaluator()
        {
            Instance = new CodingSeb.ExpressionEvaluator.ExpressionEvaluator
            {
                OptionAllowNonPublicMembersAccess = true,
                OptionPropertyOrFieldSetActive = true,
            };
        }

        internal static object Evaluate(string expression, object context, IDictionary<string, object> variables)
        {
            try
            {
                Instance.Context = context;
                Instance.Variables = variables;

                return Instance.Evaluate(expression);
            }
            catch (Exception e)
            {
                throw new ArgumentException(string.Format(InvalidExpressionMessageFormat, expression), nameof(expression), e);
            }
        }

        internal static string Validate(ref string expression, out bool hadExpressionChar)
        {
            hadExpressionChar = false;

            if (string.IsNullOrWhiteSpace(expression))
            {
                return ExpressionIsNullOrWhiteSpaceMessage;
            }

            if (expression[0] != OptionalExpressionChar)
            {
                return null;
            }

            expression = expression.Substring(1, expression.Length - 1);
            hadExpressionChar = true;

            return string.IsNullOrWhiteSpace(expression) ? ExpressionIsNullOrWhiteSpaceMessage : null;
        }
    }
}
