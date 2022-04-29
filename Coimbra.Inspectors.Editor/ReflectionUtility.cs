#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Coimbra.Editor
{
    internal static class ReflectionUtility
    {
        private const BindingFlags DeclaredBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        private const BindingFlags DefaultMethodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        private const BindingFlags PrivateMethodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        private static readonly Dictionary<Type, IReadOnlyList<MemberInfo>> AllInspectorMembersFromType = new();

        private static readonly Dictionary<Type, IReadOnlyList<MemberInfo>> DeclaredInspectorMembersFromType = new();

        internal static IReadOnlyList<MemberInfo> GetInspectorMembers(this Type sourceType, BindingFlags additionalBindingFlags)
        {
            if (AllInspectorMembersFromType.TryGetValue(sourceType, out IReadOnlyList<MemberInfo> results))
            {
                return results;
            }

            List<MemberInfo> allList = new();
            Type? currentType = sourceType;

            do
            {
                if (!DeclaredInspectorMembersFromType.TryGetValue(currentType, out IReadOnlyList<MemberInfo> declaredList))
                {
                    declaredList = currentType.GetMembers(DeclaredBindingFlags | additionalBindingFlags | BindingFlags.GetField | BindingFlags.InvokeMethod | BindingFlags.GetProperty);
                    DeclaredInspectorMembersFromType.Add(currentType, declaredList);
                }

                currentType = currentType.BaseType;
                allList.AddRange(declaredList);
            }
            while (currentType != null);

            AllInspectorMembersFromType.Add(sourceType, allList);

            return allList;
        }

        internal static MethodInfo? GetMethodSlow(this Type type, string name, bool includeParent)
        {
            MethodInfo? result = type.GetMethod(name, DefaultMethodBindingFlags);

            if (result != null || !includeParent)
            {
                return result;
            }

            while (type.BaseType != null)
            {
                type = type.BaseType;
                result = type.GetMethod(name, PrivateMethodBindingFlags);

                if (result != null)
                {
                    break;
                }
            }

            return result;
        }
    }
}
