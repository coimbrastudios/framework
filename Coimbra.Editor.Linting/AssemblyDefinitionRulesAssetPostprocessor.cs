using UnityEditor;
using UnityEditor.Callbacks;

namespace Coimbra.Editor.Linting
{
    internal sealed class AssemblyDefinitionRulesAssetPostprocessor : AssetPostprocessor
    {
        [RunAfterClass(typeof(LintingSettings))]
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            LintingSettings.InitializeAssemblyDefinitionRules();
        }
    }
}
