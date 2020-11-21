using System;
using System.Collections.Generic;
using System.Reflection;

namespace Coimbra.Editor
{
    internal static class ReflectionExtensions
    {
        private const BindingFlags DeclaredBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private const BindingFlags DefaultMethodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        private const BindingFlags PrivateMethodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private static readonly Dictionary<Type, FieldInfo[]> DeclaredFieldInfosFromType = new Dictionary<Type, FieldInfo[]>();
        private static readonly Dictionary<Type, List<FieldInfo>> AllFieldInfosFromType = new Dictionary<Type, List<FieldInfo>>();
        private static readonly Dictionary<Type, List<PropertyInfo>> AllPropertyInfosFromType = new Dictionary<Type, List<PropertyInfo>>();
        private static readonly Dictionary<Type, PropertyInfo[]> DeclaredPropertyInfosFromType = new Dictionary<Type, PropertyInfo[]>();
        private static readonly Dictionary<Type, List<MethodInfo>> AllMethodInfosFromType = new Dictionary<Type, List<MethodInfo>>();
        private static readonly Dictionary<Type, MethodInfo[]> DeclaredMethodInfosFromType = new Dictionary<Type, MethodInfo[]>();

        internal static IReadOnlyList<FieldInfo> GetAllFields(this Type type)
        {
            if (AllFieldInfosFromType.TryGetValue(type, out List<FieldInfo> results))
            {
                return results;
            }

            results = new List<FieldInfo>();

            Type current = type;

            do
            {
                if (!DeclaredFieldInfosFromType.TryGetValue(current, out FieldInfo[] declared))
                {
                    declared = current.GetFields(DeclaredBindingFlags | BindingFlags.GetField);
                    DeclaredFieldInfosFromType.Add(current, declared);
                }

                results.AddRange(declared);

                current = current.BaseType;
            }
            while (current != null);

            AllFieldInfosFromType.Add(type, results);

            return results;
        }

        internal static IReadOnlyList<PropertyInfo> GetAllProperties(this Type type)
        {
            if (AllPropertyInfosFromType.TryGetValue(type, out List<PropertyInfo> results))
            {
                return results;
            }

            results = new List<PropertyInfo>();

            Type current = type;

            do
            {
                if (!DeclaredPropertyInfosFromType.TryGetValue(current, out PropertyInfo[] declared))
                {
                    declared = current.GetProperties(DeclaredBindingFlags | BindingFlags.GetProperty);
                    DeclaredPropertyInfosFromType.Add(current, declared);
                }

                results.AddRange(declared);

                current = current.BaseType;
            }
            while (current != null);

            AllPropertyInfosFromType.Add(type, results);

            return results;
        }

        internal static IReadOnlyList<MethodInfo> GetAllMethods(this Type type)
        {
            if (AllMethodInfosFromType.TryGetValue(type, out List<MethodInfo> results))
            {
                return results;
            }

            results = new List<MethodInfo>();

            Type current = type;

            do
            {
                if (!DeclaredMethodInfosFromType.TryGetValue(current, out MethodInfo[] declared))
                {
                    declared = current.GetMethods(DeclaredBindingFlags | BindingFlags.InvokeMethod);
                    DeclaredMethodInfosFromType.Add(current, declared);
                }

                results.AddRange(declared);

                current = current.BaseType;
            }
            while (current != null);

            AllMethodInfosFromType.Add(type, results);

            return results;
        }

        internal static MethodInfo GetAnyMethod(this Type type, string name, bool includeParent)
        {
            MethodInfo result = type.GetMethod(name, DefaultMethodBindingFlags);

            if (result == null && includeParent)
            {
                while (type.BaseType != null)
                {
                    type = type.BaseType;
                    result = type.GetMethod(name, PrivateMethodBindingFlags);

                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
