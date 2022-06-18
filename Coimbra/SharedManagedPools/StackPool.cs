using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for <see cref="Stack{T}"/>.
    /// </summary>
    [Preserve]
    [SharedManagedPool("Value", "Instance")]
    public static partial class StackPool
    {
        [Preserve]
        private static class Instance<T>
        {
            internal static readonly ManagedPool<Stack<T>> Value;

            static Instance()
            {
                static Stack<T> createCallback()
                {
                    return new Stack<T>();
                }

                Value = ManagedPool<Stack<T>>.CreateShared(createCallback);

                Value.OnPush += delegate(Stack<T> instance)
                {
                    instance.Clear();
                };
            }
        }
    }
}
