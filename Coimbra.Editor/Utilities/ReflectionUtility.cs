#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Coimbra.Editor
{
    internal static class ReflectionUtility
    {
        private const string SignatureFormat = "{0}({1})";

        private const BindingFlags DefaultMethodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        private const BindingFlags PrivateMethodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        private static readonly Dictionary<Type, Dictionary<int, MethodInfo?>> MethodsByNameFromType = new Dictionary<Type, Dictionary<int, MethodInfo?>>();

        private static readonly Dictionary<Type, Dictionary<int, MethodInfo?>> MethodsBySignatureFromType = new Dictionary<Type, Dictionary<int, MethodInfo?>>();

        private static readonly Dictionary<Type, Dictionary<int, MethodInfo?>> SetterByNameFromType = new Dictionary<Type, Dictionary<int, MethodInfo?>>();

        internal static MethodInfo? FindMethodByName(this Type type, string name)
        {
            int hash = name.GetHashCode();

            if (!MethodsByNameFromType.TryGetValue(type, out Dictionary<int, MethodInfo?> methods))
            {
                methods = new Dictionary<int, MethodInfo?>();
                MethodsByNameFromType.Add(type, methods);
            }
            else if (methods.TryGetValue(hash, out MethodInfo? result))
            {
                return result;
            }

            MethodInfo? methodInfo = type.GetMethod(name, DefaultMethodBindingFlags);

            if (methodInfo != null)
            {
                methods.Add(hash, methodInfo);

                return methodInfo;
            }

            while (type.BaseType != null)
            {
                type = type.BaseType;
                methodInfo = type.GetMethod(name, PrivateMethodBindingFlags);

                if (methodInfo != null)
                {
                    break;
                }
            }

            methods.Add(hash, methodInfo);

            return methodInfo;
        }

        internal static MethodInfo? FindMethodBySignature(this Type type, string name, params Type[] parameters)
        {
            int hash = GetSignature(name, parameters).GetHashCode();

            if (!MethodsBySignatureFromType.TryGetValue(type, out Dictionary<int, MethodInfo?> methods))
            {
                methods = new Dictionary<int, MethodInfo?>();
                MethodsBySignatureFromType.Add(type, methods);
            }
            else if (methods.TryGetValue(hash, out MethodInfo? result))
            {
                return result;
            }

            MethodInfo? methodInfo = type.GetMethod(name, DefaultMethodBindingFlags, null, parameters, null);

            if (methodInfo != null)
            {
                methods.Add(hash, methodInfo);

                return methodInfo;
            }

            while (type.BaseType != null)
            {
                type = type.BaseType;
                methodInfo = type.GetMethod(name, PrivateMethodBindingFlags, null, parameters, null);

                if (methodInfo != null)
                {
                    break;
                }
            }

            methods.Add(hash, methodInfo);

            return methodInfo;
        }

        internal static MethodInfo? FindSetterByName(this Type type, string name)
        {
            int hash = name.GetHashCode();

            if (!SetterByNameFromType.TryGetValue(type, out Dictionary<int, MethodInfo?> methods))
            {
                methods = new Dictionary<int, MethodInfo?>();
                SetterByNameFromType.Add(type, methods);
            }
            else if (methods.TryGetValue(hash, out MethodInfo? result))
            {
                return result;
            }

            MethodInfo? methodInfo = type.GetProperty(name, DefaultMethodBindingFlags)?.GetSetMethod(true);

            if (methodInfo != null)
            {
                methods.Add(hash, methodInfo);

                return methodInfo;
            }

            while (type.BaseType != null)
            {
                type = type.BaseType;
                methodInfo = type.GetProperty(name, PrivateMethodBindingFlags)?.GetSetMethod(true);

                if (methodInfo != null)
                {
                    break;
                }
            }

            methods.Add(hash, methodInfo);

            return methodInfo;
        }

        private static string GetSignature(string name, IReadOnlyList<Type>? parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return string.Format(SignatureFormat, name, string.Empty);
            }

            using (StringBuilderPool.Pop(out StringBuilder stringBuilder))
            {
                stringBuilder.Append(parameters[0].FullName);

                for (int i = 1; i < parameters.Count; i++)
                {
                    stringBuilder.Append(",");
                    stringBuilder.Append(parameters[i].FullName);
                }

                return string.Format(SignatureFormat, name, stringBuilder);
            }
        }
    }
}
