using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for <see cref="Queue{T}"/>.
    /// </summary>
    [Preserve]
    [SharedManagedPool("Value", "Instance")]
    public static partial class QueuePool
    {
        [Preserve]
        private static class Instance<T>
        {
            internal static readonly ManagedPool<Queue<T>> Value;

            static Instance()
            {
                static Queue<T> createCallback()
                {
                    return new Queue<T>();
                }

                Value = ManagedPool<Queue<T>>.CreateShared(createCallback);

                Value.OnPush += delegate(Queue<T> instance)
                {
                    instance.Clear();
                };
            }
        }
    }
}
