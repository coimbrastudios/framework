using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for <see cref="List{T}"/>.
    /// </summary>
    [Preserve]
    [SharedManagedPool("Value", "Instance")]
    public static partial class ListPool
    {
        [Preserve]
        private static class Instance<T>
        {
            internal static readonly ManagedPool<List<T>> Value;

            static Instance()
            {
                static List<T> createCallback()
                {
                    return new List<T>();
                }

                Value = ManagedPool<List<T>>.CreateShared(createCallback);

                Value.OnPush += delegate(List<T> instance)
                {
                    instance.Clear();
                };
            }
        }
    }
}
