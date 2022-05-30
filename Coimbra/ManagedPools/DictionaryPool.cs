using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    [Preserve]
    [SharedManagedPool("Value", "Instance")]
    public static partial class DictionaryPool
    {
        [Preserve]
        private static class Instance<TKey, TValue>
        {
            internal static readonly ManagedPool<Dictionary<TKey, TValue>> Value;

            static Instance()
            {
                static Dictionary<TKey, TValue> createCallback()
                {
                    return new Dictionary<TKey, TValue>();
                }

                Value = new ManagedPool<Dictionary<TKey, TValue>>(createCallback);

                Value.OnPush += delegate(Dictionary<TKey, TValue> instance)
                {
                    instance.Clear();
                };
            }
        }
    }
}
