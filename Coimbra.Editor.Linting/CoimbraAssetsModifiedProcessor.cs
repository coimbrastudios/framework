using UnityEditor;
using UnityEditor.Experimental;

namespace Coimbra.Editor.Linting
{
    internal sealed class CoimbraAssetsModifiedProcessor : AssetsModifiedProcessor
    {
        /// <inheritdoc/>
        protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
        {
            EditorApplication.update += delegate
            {
                if (!ScriptableSettings.TryGetOrFind(out LintingSettings settings) || !settings.ForceUpdateAssetsAssembly)
                {
                    return;
                }

                foreach (string asset in addedAssets)
                {
                    if (!asset.EndsWith(".cs"))
                    {
                        continue;
                    }

                    CoimbraEditorUtility.CreateAssetsAssemblies();

                    return;
                }
            };
        }
    }
}
