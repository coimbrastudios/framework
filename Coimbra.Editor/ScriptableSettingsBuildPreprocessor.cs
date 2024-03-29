using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Coimbra.Editor
{
    internal sealed class ScriptableSettingsBuildPreprocessor : IPreprocessBuildWithReport
    {
        /// <inheritdoc/>
        public int callbackOrder { get; }

        /// <inheritdoc/>
        public void OnPreprocessBuild(BuildReport report)
        {
            foreach (string asset in AssetDatabase.FindAssets($"t:{nameof(ScriptableSettings)}", ScriptableSettings.FindAssetsFolders))
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                ScriptableSettings scriptableSettings = AssetDatabase.LoadAssetAtPath<ScriptableSettings>(path);
                scriptableSettings.ValidatePreload(true);
            }
        }
    }
}
