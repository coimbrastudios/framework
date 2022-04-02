using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    internal sealed class FrameworkEditorUtility : AssetPostprocessor
    {
        internal static void CreateAssetWithFolderHierarchy(Object asset, string path)
        {
            string[] folders = path.Split('/');
            string current = folders[0];

            for (int i = 1; i < folders.Length - 1; i++)
            {
                string target = current + $"/{folders[i]}";

                if (!AssetDatabase.IsValidFolder(target))
                {
                    AssetDatabase.CreateFolder(current, folders[i]);
                }

                current = target;
            }

            AssetDatabase.CreateAsset(asset, path);
        }

        internal static void DeleteDirectory(string directoryPath, bool onlyIfEmpty, bool recursiveDelete)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            bool isEmpty = !Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories).Any()
                        && !Directory.EnumerateDirectories(directoryPath, "*", SearchOption.AllDirectories).Any();

            if (onlyIfEmpty && !isEmpty)
            {
                return;
            }

            string relativePath = directoryPath.Replace("\\", "/").Replace(Application.dataPath, "Assets");

            if (AssetDatabase.IsValidFolder(relativePath))
            {
                AssetDatabase.DeleteAsset(relativePath);
            }
            else
            {
                Directory.Delete(directoryPath, recursiveDelete);
            }
        }

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
