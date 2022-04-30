#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Coimbra.Inspectors.Editor
{
    internal static class ReflectionUtility
    {
        private const BindingFlags DeclaredBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        private const BindingFlags DefaultMethodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        private const BindingFlags PrivateMethodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        private static readonly Dictionary<Type, IReadOnlyList<MemberInfo>> AllInstanceInspectorMembersFromType = new();

        private static readonly Dictionary<Type, IReadOnlyList<MemberInfo>> AllStaticInspectorMembersFromType = new();

        private static readonly Dictionary<Type, IReadOnlyList<MemberInfo>> DeclaredInstanceInspectorMembersFromType = new();

        private static readonly Dictionary<Type, IReadOnlyList<MemberInfo>> DeclaredStaticInspectorMembersFromType = new();

        internal static IReadOnlyList<MemberInfo> GetInstanceInspectorMembers(this Type sourceType)
        {
            return GetInspectorMembers(sourceType, AllInstanceInspectorMembersFromType, DeclaredInstanceInspectorMembersFromType, BindingFlags.Instance);
        }

        internal static IReadOnlyList<MemberInfo> GetStaticInspectorMembers(this Type sourceType)
        {
            return GetInspectorMembers(sourceType, AllStaticInspectorMembersFromType, DeclaredStaticInspectorMembersFromType, BindingFlags.Static);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IReadOnlyList<MemberInfo> GetInspectorMembers(Type sourceType,
                                                                      Dictionary<Type, IReadOnlyList<MemberInfo>> allInspectorMembersFromType,
                                                                      Dictionary<Type, IReadOnlyList<MemberInfo>> declaredInspectorMembersFromType,
                                                                      BindingFlags additionalBindingFlags)
        {
            if (allInspectorMembersFromType.TryGetValue(sourceType, out IReadOnlyList<MemberInfo> results))
            {
                return results;
            }

            List<MemberInfo> allList = new();
            Type? currentType = sourceType;

            do
            {
                if (!declaredInspectorMembersFromType.TryGetValue(currentType, out IReadOnlyList<MemberInfo> declaredList))
                {
                    declaredList = currentType.GetMembers(DeclaredBindingFlags | additionalBindingFlags | BindingFlags.GetField | BindingFlags.InvokeMethod | BindingFlags.GetProperty);
                    declaredInspectorMembersFromType.Add(currentType, declaredList);
                }

                currentType = currentType.BaseType;
                allList.AddRange(declaredList);
            }
            while (currentType != null);

            allInspectorMembersFromType.Add(sourceType, allList);

            return allList;
        }
    }
}
