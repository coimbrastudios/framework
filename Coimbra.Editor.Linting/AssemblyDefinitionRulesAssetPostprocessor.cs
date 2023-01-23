using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Coimbra.Editor.Linting
{
    internal sealed class AssemblyDefinitionRulesAssetPostprocessor : AssetPostprocessor
    {
        [RunAfterClass(typeof(LintingSettings))]
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool hasExtension = false;

            void checkExtension(string[] assets)
            {
                foreach (string asset in assets)
                {
                    if (hasExtension)
                    {
                        return;
                    }

                    if (!asset.EndsWith(".asmdef"))
                    {
                        continue;
                    }

                    hasExtension = true;

                    return;
                }
            }

            Parallel.Invoke(() => checkExtension(importedAssets), () => checkExtension(deletedAssets), () => checkExtension(movedAssets));

            if (hasExtension)
            {
                LintingSettings.InitializeAssemblyDefinitionRules();
            }
        }
    }
}
