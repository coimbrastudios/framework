using JetBrains.Annotations;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for objects with a default constructor.
    /// </summary>
    [Preserve]
    public static class SharedManagedPool
    {
        [Preserve]
        internal static class Instance<T>
            where T : class, new()
        {
            internal static readonly ManagedPool<T> Value;

            static Instance()
            {
                static T createCallback()
                {
                    return new T();
                }

                Action<T> disposeCallback = null;

                if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
                {
                    disposeCallback += delegate(T obj)
                    {
                        if (obj.TryGetValid(out T valid))
                        {
                            ((IDisposable)valid).Dispose();
                        }
                    };
                }

                if (typeof(Object).IsAssignableFrom(typeof(T)))
                {
                    disposeCallback += delegate(T obj)
                    {
                        if (CoimbraUtility.IsPlayMode)
                        {
                            if ((obj as Object).TryGetValid(out Object valid))
                            {
                                Object.Destroy(valid);
                            }
                        }
                        else if ((obj as Object).TryGetValid(out Object valid))

                        {
                            Object.DestroyImmediate(valid);
                        }
                    };
                }

                Value = new ManagedPool<T>(createCallback, disposeCallback);
            }
        }

        /// <inheritdoc cref="ManagedPool{T}.MaxCapacity"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMaxCapacity<T>()
            where T : class, new()
        {
            return Instance<T>.Value.MaxCapacity;
        }

        /// <inheritdoc cref="ManagedPool{T}.PreloadCount"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPreloadCount<T>()
            where T : class, new()
        {
            return Instance<T>.Value.PreloadCount;
        }

        /// <inheritdoc cref="ManagedPool{T}.Initialize"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize<T>(int? preloadCount = null, int? maxCapacity = null)
            where T : class, new()
        {
            Instance<T>.Value.Initialize(preloadCount, maxCapacity);
        }

        /// <inheritdoc cref="ManagedPool{T}.Pop()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Pop<T>()
            where T : class, new()
        {
            return Instance<T>.Value.Pop();
        }

        /// <inheritdoc cref="ManagedPool{T}.Pop()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ManagedPool<T>.Instance Pop<T>([NotNull] out T instance)
            where T : class, new()
        {
            return Instance<T>.Value.Pop(out instance);
        }

        /// <inheritdoc cref="ManagedPool{T}.Push"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Push<T>([NotNull] in T instance)
            where T : class, new()
        {
            Instance<T>.Value.Push(in instance);
        }
    }
}
