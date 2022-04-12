using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="Type"/> type.
    /// </summary>
    public static class TypeUtility
    {
        /// <summary>
        /// Asserts that the type meets all the following requirements:
        /// <para>
        /// - Is an interface<br/>
        /// - Is not equal to the generic argument<br/>
        /// - Does implement the generic argument
        /// </para>
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertInterfaceImplementsNotEqual<TInterface>(this Type type, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
            where TInterface : class
        {
            if (!type.IsInterface)
            {
                throw new ArgumentOutOfRangeException($"\"{memberName}\" at \"{filePath}({lineNumber})\" requires an interface type argument!");
            }

            if (type == typeof(TInterface))
            {
                throw new ArgumentOutOfRangeException($"\"{memberName}\" at \"{filePath}({lineNumber})\" requires a type different than \"{typeof(TInterface)}\" itself!");
            }

            if (!typeof(TInterface).IsAssignableFrom(type))
            {
                throw new ArgumentOutOfRangeException($"\"{memberName}\" at \"{filePath}({lineNumber})\" requires a type that implements \"{typeof(TInterface)}\"!");
            }
        }

        /// <summary>
        /// Asserts that the type meets all the following requirements:
        /// <para>
        /// - is not an interface<br/>
        /// - does implement the generic argument
        /// </para>
        /// </summary>
        [Conditional("UNITY_ASSERTIONS")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertNonInterfaceImplements<TInterface>(this Type type, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
            where TInterface : class
        {
            if (type.IsInterface)
            {
                throw new ArgumentOutOfRangeException($"\"{memberName}\" at \"{filePath}({lineNumber})\" requires a non-interface type argument!");
            }

            if (!typeof(TInterface).IsAssignableFrom(type))
            {
                throw new ArgumentOutOfRangeException($"\"{memberName}\" at \"{filePath}({lineNumber})\" requires a type that implements \"{typeof(TInterface)}\"!");
            }
        }
    }
}
