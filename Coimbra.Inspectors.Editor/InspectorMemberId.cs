#nullable enable

using Coimbra.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace Coimbra.Inspectors.Editor
{
    internal sealed class InspectorMemberId : IEquatable<InspectorMemberId>
    {
        private static readonly Dictionary<(string Name, Type Parent), InspectorMemberId> Cache = new();

        internal readonly string Name;

        internal readonly Type Parent;

        private InspectorMemberId(string name, Type parent)
        {
            Name = name;
            Parent = parent;
        }

        public bool Equals(InspectorMemberId other)
        {
            return Name == other.Name
                && Parent == other.Parent;
        }

        public override bool Equals(object? obj)
        {
            return obj is InspectorMemberId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Parent);
        }

        public static bool operator ==(InspectorMemberId left, InspectorMemberId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InspectorMemberId left, InspectorMemberId right)
        {
            return !left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static InspectorMemberId Get(MemberInfo memberInfo)
        {
            return Get(memberInfo.Name, memberInfo.DeclaringType!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static InspectorMemberId Get(SerializedProperty serializedProperty)
        {
            return Get(serializedProperty.GetFieldInfo()!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static InspectorMemberId Get(string name, Type parent)
        {
            (string, Type) tuple = (name, parent);

            if (Cache.TryGetValue(tuple, out InspectorMemberId id))
            {
                return id;
            }

            id = new InspectorMemberId(name, parent);
            Cache.Add(tuple, id);

            return id;
        }
    }
}
