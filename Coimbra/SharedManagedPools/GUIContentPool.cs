using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Static implementation of <see cref="ManagedPool{T}"/> for <see cref="GUIContent"/>.
    /// </summary>
    [Preserve]
    [SharedManagedPool(nameof(Value))]
    public static partial class GUIContentPool
    {
        internal static readonly ManagedPool<GUIContent> Value;

        static GUIContentPool()
        {
            static GUIContent createCallback()
            {
                return new GUIContent();
            }

            Value = ManagedPool<GUIContent>.CreateShared(createCallback);

            Value.OnPush += delegate(GUIContent instance)
            {
                instance.image = null;
                instance.text = string.Empty;
                instance.tooltip = string.Empty;
            };
        }
    }
}
