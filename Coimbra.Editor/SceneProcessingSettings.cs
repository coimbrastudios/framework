using JetBrains.Annotations;
using UnityEditor.Build;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// Settings to customize the behaviour of <see cref="ISceneProcessorComponent"/> and <see cref="IScenePostProcessorComponent"/>.
    /// </summary>
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath, true)]
    public sealed class SceneProcessingSettings : ScriptableSettings
    {
        /// <inheritdoc cref="IOrderedCallback.callbackOrder"/>
        [PublicAPI]
        [field: SerializeField]
        [field: Tooltip("Callbacks with lower values are called before ones with higher values.")]
        public int ProcessSceneCallbackOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="ISceneProcessorComponent.OnProcessScene"/> should be called.
        /// </summary>
        [PublicAPI]
        [field: SerializeField]
        [field: Tooltip("If true then the scene processor callback will never be called.")]
        public bool DisableSceneProcessorComponentCallback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="IScenePostProcessorComponent.OnPostProcessScene"/> should be called.
        /// </summary>
        [PublicAPI]
        [field: SerializeField]
        [field: Tooltip("If true then the scene post processor callback will never be called.")]
        public bool DisableScenePostProcessorComponentCallback { get; set; }
    }
}
