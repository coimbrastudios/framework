using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Samples.DifficultySettings.Editor
{
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath + "/Samples", true, FileDirectory = CoimbraUtility.ProjectSettingsFilePath + ".samples")]
    public sealed class DifficultyListSettings : ScriptableSettings
    {
        [SerializeField]
        [MessageBox("Right click this field and hit 'refresh' to check the current instance.")]
        [ContextMenuItem("Refresh", nameof(Refresh))]
        private DifficultySettings _defaultDifficultySettings;

        [SerializeField]
        [NonReorderable]
        [MessageBox("Right click this field and hit 'refresh' to check the current list.")]
        [ContextMenuItem("Refresh", nameof(Refresh))]
        private List<DifficultySettings> _difficultySettingsList = new();

        private void Refresh()
        {
#if UNITY_EDITOR
            _difficultySettingsList.Clear();

            foreach (string asset in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(DifficultySettings)}"))
            {
                DifficultySettings difficultySettings = UnityEditor.AssetDatabase.LoadAssetAtPath<DifficultySettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(asset));

                if (difficultySettings.IsValid())
                {
                    _difficultySettingsList.Add(difficultySettings);

                    if (difficultySettings.IsDefault)
                    {
                        _defaultDifficultySettings = difficultySettings;
                    }
                }
            }
#endif
        }
    }
}
