using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Coimbra.Samples.DifficultySettings
{
    /// <summary>
    /// A simple editor-only <see cref="ScriptableSettings"/> implementation that showcases how you can easily expose information in the editor using the <see cref="ProjectSettingsAttribute"/> without much code.
    /// </summary>
    /// <seealso cref="DifficultySettings"/>
    /// <seealso cref="DifficultySettingsCube"/>
    [Preferences(CoimbraUtility.UserPreferencesPath + "/Samples", false, FileDirectory = CoimbraUtility.UserPreferencesFilePath + ".samples")]
    public sealed class DifficultyListSettings : ScriptableSettings
    {
        [SerializeField]
        [UsedImplicitly]
        [FormerlySerializedAs("_defaultDifficultySettings")]
        [MessageBox("Reset to check the current instance.")]
        [Disable]
        private DifficultySettings _preloadDifficultySettings;

        [SerializeField]
        [NonReorderable]
        [MessageBox("Reset to check the current list.")]
        [Disable]
        private List<DifficultySettings> _difficultySettingsList = new();

        /// <inheritdoc />
        protected override void OnReset()
        {
            base.OnReset();
            Refresh();
        }

        private void OnDisable()
        {
            Refresh();
        }

        private void Refresh()
        {
#if UNITY_EDITOR
            _difficultySettingsList.Clear();

            foreach (string asset in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(DifficultySettings)}"))
            {
                DifficultySettings difficultySettings = UnityEditor.AssetDatabase.LoadAssetAtPath<DifficultySettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(asset));

                if (!difficultySettings.IsValid())
                {
                    continue;
                }

                _difficultySettingsList.Add(difficultySettings);

                if (difficultySettings.Preload)
                {
                    _preloadDifficultySettings = difficultySettings;
                }
            }
#endif
        }
    }
}
