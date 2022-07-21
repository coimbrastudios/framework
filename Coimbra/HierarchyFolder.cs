using System.Diagnostics;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Special <see cref="Actor"/> that only exists in edit-mode. It detaches all its children before destroying itself.
    /// </summary>
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Hierarchy Folder")]
    public sealed class HierarchyFolder : Actor, ISceneProcessorComponent
    {
        [SerializeField]
        [DisableOnPlayMode]
        [Tooltip("If false, the object will not be destroyed when inside the editor.")]
        private bool _dontDestroyInEditor;

        /// <summary>
        /// Gets or sets a value indicating whether this actor should be destroyed when playing inside the editor. Changing this value at runtime has no effect.
        /// </summary>
        public bool DontDestroyInEditor
        {
            [DebuggerStepThrough]
            get => _dontDestroyInEditor;
            [DebuggerStepThrough]
            set => _dontDestroyInEditor = value;
        }

        /// <inheritdoc/>
        protected override void OnInitialize()
        {
            base.OnInitialize();

#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                Apply();
            }
        }

        private void Apply()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && _dontDestroyInEditor)
            {
                return;
            }
#endif
            Transform parent = Transform.parent;
            int count = Transform.childCount;

            for (int i = 0; i < count; i++)
            {
                Transform.GetChild(0).parent = parent;
            }

            Destroy();
        }

        /// <inheritdoc/>
        void ISceneProcessorComponent.OnProcessScene()
        {
            Apply();
        }
    }
}
