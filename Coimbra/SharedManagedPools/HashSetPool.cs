using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for <see cref="HashSet{T}"/>.
    /// </summary>
    [Preserve]
    [SharedManagedPool("Value", "Instance")]
    public static partial class HashSetPool
    {
        [Preserve]
        private static class Instance<T>
        {
            internal static readonly ManagedPool<HashSet<T>> Value;

            static Instance()
            {
                static HashSet<T> createCallback()
                {
                    return new HashSet<T>();
                }

                Value = ManagedPool<HashSet<T>>.CreateShared(createCallback);

                Value.OnPush += delegate(HashSet<T> instance)
                {
                    instance.Clear();
                };
            }
        }
    }
}
