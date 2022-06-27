#if COIMBRA_LOG_ASSETS_MODIFIED
using UnityEditor.Experimental;
using Debug = UnityEngine.Debug;

namespace Coimbra.Editor
{
    internal sealed class CoimbraAssetsModifiedProcessor : AssetsModifiedProcessor
    {
        /// <inheritdoc/>
        protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
        {
            Debug.Log($"Changed {changedAssets.Length} assets:");

            foreach (string changedAsset in changedAssets)
            {
                Debug.Log(changedAsset);
            }

            Debug.Log($"Added {addedAssets.Length} assets:");

            foreach (string addedAsset in addedAssets)
            {
                Debug.Log(addedAsset);
            }

            Debug.Log($"Deleted {deletedAssets.Length} assets:");

            foreach (string deletedAsset in deletedAssets)
            {
                Debug.Log(deletedAsset);
            }

            Debug.Log($"Moved {movedAssets.Length} assets:");

            foreach (AssetMoveInfo movedAsset in movedAssets)
            {
                Debug.Log($"{movedAsset.sourceAssetPath} to {movedAsset.destinationAssetPath}");
            }
        }
    }
}
#endif
