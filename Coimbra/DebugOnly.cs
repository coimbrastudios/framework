using System.Diagnostics;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Makes an <see cref="Actor"/> only exist in the editor or development builds. It destroys itself during scene processing if specified condition is met.
    /// </summary>
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Debug Only")]
    public sealed class DebugOnly : ActorComponentBase, ISceneProcessorComponent
    {
        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip("If false, the object will not be destroyed when generating a development build.")]
        private bool _destroyInDevelopmentBuild;

        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip("If false, the object will not be destroyed when inside the editor.")]
        private bool _destroyInPlayMode;

        /// <summary>
        /// Gets or sets a value indicating whether this actor should be destroyed when generating a development build. Changing this value outside the editor has no effect.
        /// </summary>
        public bool DestroyInDevelopmentBuild
        {
            [DebuggerStepThrough]
            get => _destroyInDevelopmentBuild;
            [DebuggerStepThrough]
            set => _destroyInDevelopmentBuild = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this actor should be destroyed when playing inside the editor. Changing this value outside the editor has no effect.
        /// </summary>
        public bool DestroyInPlayMode
        {
            [DebuggerStepThrough]
            get => _destroyInPlayMode;
            [DebuggerStepThrough]
            set => _destroyInPlayMode = value;
        }

        /// <inheritdoc/>
        protected override void OnPreInitializeActor()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                Apply();
            }
        }

        /// <inheritdoc/>
        protected override void OnPostInitializeActor() { }

        private void Apply()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && !_destroyInPlayMode)
            {
                return;
            }
#endif
#if DEBUG
            if (!Application.isEditor && !_destroyInDevelopmentBuild)
            {
                return;
            }
#endif
            Actor.Destroy();
        }

        /// <inheritdoc/>
        void ISceneProcessorComponent.OnProcessScene()
        {
            Apply();
        }
    }
}
