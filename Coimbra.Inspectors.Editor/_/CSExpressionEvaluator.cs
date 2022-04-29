#if false
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [InitializeOnLoad]
    internal static class CSExpressionEvaluator
    {
        private const char ExpressionChar = '?';
        private const string ExpressionIsNullOrWhiteSpaceMessage = "Expression is null or white space.";
        private const string InvalidActionSignatureMessageFormat = "Inavlid method signature, expected \"{0}():void\"";
        private const string InvalidConditionSignatureMessageFormat = "Inavlid method signature, expected \"{0}():bool\"";
        private const string InvalidExpressionMessageFormat = "Invalid expression \"{0}\"";
        private const string InvalidMethodMessageFormat = "Invalid method \"{0}\"";
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

        internal static void InvokeAction(IReadOnlyList<object> scopes, string expression)
        {
            string error = Validate(ref expression, out bool hadExpressionChar);

            if (error != null)
            {
                Debug.LogError(error);

                return;
            }

            if (hadExpressionChar)
            {
                try
                {
                    for (int i = 0; i < scopes.Count; i++)
                    {
                        Evaluate(expression, scopes[i], null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                return;
            }

            MethodInfo methodInfo = scopes[0].GetType().GetMethod(expression, true);

            if (methodInfo == null)
            {
                Debug.LogErrorFormat(InvalidMethodMessageFormat, expression);

                return;
            }

            if (methodInfo.GetParameters().Length != 0)
            {
                Debug.LogErrorFormat(InvalidActionSignatureMessageFormat, expression);

                return;
            }

            if (methodInfo.IsStatic)
            {
                methodInfo.Invoke(null, null);
            }
            else
            {
                for (int i = 0; i < scopes.Count; i++)
                {
                    methodInfo.Invoke(scopes[i], null);
                }
            }
        }

        internal static bool InvokeCondition(IReadOnlyList<object> scopes, string conditionExpression)
        {
            string error = Validate(ref conditionExpression, out bool hadExpressionChar);

            if (error != null)
            {
                Debug.LogError(error);

                return false;
            }

            if (hadExpressionChar)
            {
                try
                {
                    for (int i = 0; i < scopes.Count; i++)
                    {
                        if (!(bool)Evaluate(conditionExpression, scopes[i], null))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);

                    return false;
                }
            }

            MethodInfo methodInfo = scopes[0].GetType().GetMethod(conditionExpression, true);

            if (methodInfo == null)
            {
                Debug.LogErrorFormat(InvalidMethodMessageFormat, conditionExpression);

                return false;
            }

            if (methodInfo.GetParameters().Length != 0 || methodInfo.ReturnType != typeof(bool))
            {
                Debug.LogErrorFormat(InvalidConditionSignatureMessageFormat, conditionExpression);

                return false;
            }

            if (methodInfo.IsStatic)
            {
                return (bool)methodInfo.Invoke(null, null);
            }

            for (int i = 0; i < scopes.Count; i++)
            {
                if (!(bool)methodInfo.Invoke(scopes[i], null))
                {
                    return false;
                }
            }

            return true;
        }

        internal static string Validate(ref string expression, out bool hadExpressionChar)
        {
            hadExpressionChar = false;

            if (string.IsNullOrWhiteSpace(expression))
            {
                return ExpressionIsNullOrWhiteSpaceMessage;
            }

            if (expression[0] != ExpressionChar)
            {
                return null;
            }

            expression = expression.Substring(1, expression.Length - 1);
            hadExpressionChar = true;

            return string.IsNullOrWhiteSpace(expression) ? ExpressionIsNullOrWhiteSpaceMessage : null;
        }
    }
}
#endif
