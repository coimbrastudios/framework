using UnityEngine;

namespace Coimbra.Samples.DifficultySettings
{
    [CreateAssetMenu(menuName = CoimbraUtility.GeneralMenuPath + "Samples/Difficulty Settings")]
    public sealed class DifficultySettings : ScriptableSettings
    {
        [SerializeField]
        private bool _isDefault;

        [SerializeField]
        private float _cubeMovementSpeed = 10;

        public bool IsDefault
        {
            get => _isDefault;
            private set => _isDefault = value;
        }

        public float CubeMovementSpeed => _cubeMovementSpeed;

        protected override void OnLoaded()
        {
            if (!_isDefault)
            {
                return;
            }

            base.OnLoaded();

            Application.quitting += HandleApplicationQuitting;
        }

        protected override void OnUnload(bool wasCurrentInstance)
        {
            Application.quitting -= HandleApplicationQuitting;

            base.OnUnload(wasCurrentInstance);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

#if UNITY_EDITOR
            if (_isDefault)
            {
                foreach (string asset in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(DifficultySettings)}"))
                {
                    DifficultySettings difficultySettings = UnityEditor.AssetDatabase.LoadAssetAtPath<DifficultySettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(asset));

                    if (difficultySettings.IsValid() && difficultySettings.IsDefault && difficultySettings != this)
                    {
                        difficultySettings.IsDefault = false;
                    }
                }

                SetOrOverwrite(this);
            }
#endif
        }

        private void HandleApplicationQuitting()
        {
            if (_isDefault)
            {
                SetOrOverwrite(this);
            }
        }
    }
}
