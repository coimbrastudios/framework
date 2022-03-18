using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    internal sealed class FrameworkEditorUtility : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            using Disposable<List<Object>> pooledList = ManagedPool<List<Object>>.Shared.GetDisposable();
            pooledList.Value.Clear();
            pooledList.Value.AddRange(PlayerSettings.GetPreloadedAssets());

            int count = pooledList.Value.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                if (pooledList.Value[i] == null)
                {
                    pooledList.Value.RemoveAt(i);
                }
            }

            if (count != pooledList.Value.Count)
            {
                PlayerSettings.SetPreloadedAssets(pooledList.Value.ToArray());
            }

            pooledList.Value.Clear();
        }
    }
}
