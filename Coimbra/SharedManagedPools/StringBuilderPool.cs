using System.Text;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for <see cref="StringBuilder"/>.
    /// </summary>
    [Preserve]
    [SharedManagedPool(nameof(Value))]
    public static partial class StringBuilderPool
    {
        internal static readonly ManagedPool<StringBuilder> Value;

        static StringBuilderPool()
        {
            static StringBuilder createCallback()
            {
                return new StringBuilder();
            }

            Value = ManagedPool<StringBuilder>.CreateShared(createCallback);

            Value.OnPush += delegate(StringBuilder instance)
            {
                instance.Clear();
            };
        }
    }
}
