using UnityEditor;

namespace Coimbra.Editor.Linting
{
    internal sealed class AssemblyDefinitionRulesAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            LintingSettings.InitializeAssemblyDefinitionRules();
        }
    }
}
