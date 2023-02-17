using UnityEngine;

namespace Coimbra.Samples.DifficultySettings
{
    /// <summary>
    /// Example of how to create a <see cref="ScriptableSettingsType.Custom"/> <see cref="ScriptableSettings"/> that is expected to have the current instance changed at runtime.
    /// </summary>
    /// <seealso cref="DifficultyListSettings"/>
    /// <seealso cref="DifficultySettingsCube"/>
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + "Samples/Difficulty Settings")]
    public sealed class DifficultySettings : ScriptableSettings
    {
        [SerializeField]
        private float _cubeMovementSpeed = 10;

        public float CubeMovementSpeed => _cubeMovementSpeed;

        /// <inheritdoc/>
        protected override void OnLoaded()
        {
            if (!Preload)
            {
                return;
            }

            base.OnLoaded();

            Application.quitting += HandleApplicationQuitting;
        }

        /// <inheritdoc/>
        protected override void OnUnload()
        {
            Application.quitting -= HandleApplicationQuitting;

            base.OnUnload();
        }

        /// <inheritdoc/>
        protected override void OnValidating()
        {
            base.OnValidating();

#if UNITY_EDITOR
            if (Preload)
            {
                foreach (string asset in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(DifficultySettings)}"))
                {
                    DifficultySettings difficultySettings = UnityEditor.AssetDatabase.LoadAssetAtPath<DifficultySettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(asset));

                    if (difficultySettings.IsValid() && difficultySettings.Preload && difficultySettings != this)
                    {
                        difficultySettings.Preload = false;
                    }
                }

                SetOrOverwrite(this);
            }
#endif
        }

        private void HandleApplicationQuitting()
        {
            if (Preload)
            {
                SetOrOverwrite(this);
            }
        }
    }
}
