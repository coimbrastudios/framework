#nullable enable

using System;

namespace Coimbra.Inspectors
{
    /// <summary>
    /// Customize the condition to a decorator take effect.
    /// </summary>
    [Flags]
    public enum DecoratorConditions
    {
        /// <seealso cref="AnyMode"/>
        /// <seealso cref="AnyObject"/>
        Default = AnyMode | AnyObject,

        /// <seealso cref="EditMode"/>
        /// <seealso cref="PlayMode"/>
        AnyMode = EditMode | PlayMode,

        /// <seealso cref="Assets"/>
        /// <seealso cref="Instances"/>
        AnyObject = Assets | Instances,

        /// <seealso cref="OriginalAsset"/>
        /// <seealso cref="VariantAsset"/>
        Assets = OriginalAsset | VariantAsset,

        /// <seealso cref="PrefabInstance"/>
        /// <seealso cref="SceneInstance"/>
        Instances = PrefabInstance | SceneInstance,

        /// <summary>
        /// Takes effect while in edit mode.
        /// </summary>
        EditMode = 1 << 0,

        /// <summary>
        /// Takes effect while in play mode.
        /// </summary>
        PlayMode = 1 << 1,

        /// <summary>
        /// Takes effect while editing an asset or a non-variant prefab.
        /// </summary>
        OriginalAsset = 1 << 3,

        /// <summary>
        /// Takes effect while editing a prefab variant.
        /// </summary>
        VariantAsset = 1 << 4,

        /// <summary>
        /// Takes effect while editing a scene object with a prefab.
        /// </summary>
        PrefabInstance = 1 << 5,

        /// <summary>
        /// Takes effect while editing a scene object without a prefab.
        /// </summary>
        SceneInstance = 1 << 6,
    }
}
