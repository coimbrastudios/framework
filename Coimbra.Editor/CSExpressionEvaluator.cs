using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [InitializeOnLoad]
    internal static class CSExpressionEvaluator
    {
        private const string EvaluateWarning = "Unable to evaluate expression \"{0}\": {1}";
        private static readonly CodingSeb.ExpressionEvaluator.ExpressionEvaluator Instance;

        static CSExpressionEvaluator()
        {
            Instance = new CodingSeb.ExpressionEvaluator.ExpressionEvaluator
            {
                OptionAllowNonPublicMembersAccess = true,
                OptionPropertyOrFieldSetActive = true,
            };
        }

        internal static bool TryEvaluate(string expression, object context = null, IDictionary<string, object> variables = null, bool methodFallback = true)
        {
            try
            {
                Instance.Context = context;
                Instance.Variables = variables;
                Instance.Evaluate(expression);

                return true;
            }
            catch (Exception e)
            {
                if (methodFallback)
                {
                    try
                    {
                        Instance.Evaluate(expression + "()");

                        return true;
                    }
                    catch
                    {
                        Debug.LogWarningFormat(EvaluateWarning, expression, e);
                    }
                }
                else
                {
                    Debug.LogWarningFormat(EvaluateWarning, expression, e);
                }

                return false;
            }
        }

        internal static bool TryEvaluate<T>(string expression, out T result, object context = null, IDictionary<string, object> variables = null, bool methodFallback = true)
        {
            try
            {
                Instance.Context = context;
                Instance.Variables = variables;
                result = Instance.Evaluate<T>(expression);

                return true;
            }
            catch (Exception e)
            {
                if (methodFallback)
                {
                    try
                    {
                        result = Instance.Evaluate<T>(expression + "()");

                        return true;
                    }
                    catch
                    {
                        Debug.LogWarningFormat(EvaluateWarning, expression, e);
                    }
                }
                else
                {
                    Debug.LogWarningFormat(EvaluateWarning, expression, e);
                }

                result = default;

                return false;
            }
        }
    }
}
