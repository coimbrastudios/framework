using UnityEditor;

namespace Coimbra.Editor.UPM
{
    internal sealed class UPMAuthenticatorAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            UPMAuthenticator.Update();
        }
    }
}
