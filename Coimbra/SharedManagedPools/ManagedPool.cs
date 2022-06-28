using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for objects with a default constructor that implements <see cref="IManagedPoolHandler"/>.
    /// </summary>
    [Preserve]
    [SharedManagedPool("Value", "Instance")]
    public static partial class ManagedPool
    {
        [Preserve]
        private static class Instance<T>
            where T : class, IManagedPoolHandler, new()
        {
            internal static readonly ManagedPool<T> Value;

            static Instance()
            {
                static T onCreate()
                {
                    return new T();
                }

                static void onDispose(T instance)
                {
                    instance.Dispose();
                }

                Value = ManagedPool<T>.CreateShared(onCreate, onDispose);

                Value.OnPop += delegate(T instance)
                {
                    instance.OnPop();
                };

                Value.OnPush += delegate(T instance)
                {
                    instance.OnPush();
                };
            }
        }
    }
}
