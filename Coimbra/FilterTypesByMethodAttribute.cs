using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// <see cref="FilterTypesAttributeBase"/> that forward its implementation to the owning class.
    /// </summary>
    /// <seealso cref="FilterTypesAttributeBase"/>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FilterTypesByMethodAttribute : FilterTypesAttributeBase
    {
        /// <summary>
        /// The method name to forward the implementation to. It is expected to have the signature <see cref="bool"/>(<see cref="PropertyPathInfo"/>, <see cref="UnityEngine.Object"/>, <see cref="Type"/>).
        /// </summary>
        public readonly string MethodName;

        private static readonly object[] InvokeParameters = new object[3];

        private static readonly Type[] MethodInfoParameters =
        {
            typeof(PropertyPathInfo),
            typeof(Object),
            typeof(Type),
        };

        public FilterTypesByMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        /// <inheritdoc/>
        public override bool Validate(PropertyPathInfo context, Object[] targets, Type type)
        {
            using (ListPool.Pop(out List<object> scopes))
            {
                context.GetScopes(targets, scopes, true);

                for (int i = 0; i < scopes.Count; i++)
                {
                    object scope = scopes[i];

                    if (scope == null)
                    {
                        return false;
                    }

                    MethodInfo methodInfo = scope.GetType().FindMethodBySignature(in MethodName, MethodInfoParameters);

                    if (methodInfo == null || methodInfo.ReturnType != typeof(bool))
                    {
                        Debug.LogWarning($"{scope} doesn't contain a method {MethodName} with the signature {typeof(bool)}({typeof(PropertyInfo)}, {typeof(Object)}, {typeof(Type)})");

                        return false;
                    }

                    InvokeParameters[0] = context;
                    InvokeParameters[1] = targets[i];
                    InvokeParameters[2] = type;

                    if (!(bool)methodInfo.Invoke(scope, InvokeParameters))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
