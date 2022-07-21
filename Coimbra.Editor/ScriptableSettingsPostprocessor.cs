using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    internal sealed class ScriptableSettingsPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            Initialize();
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            static void initialize()
            {
                using (ListPool.Pop(out List<Object> pooledList))
                {
                    pooledList.AddRange(PlayerSettings.GetPreloadedAssets());

                    int count = pooledList.Count;

                    for (int i = count - 1; i >= 0; i--)
                    {
                        if (pooledList[i] == null)
                        {
                            pooledList.RemoveAt(i);
                        }
                    }

                    if (count != pooledList.Count)
                    {
                        PlayerSettings.SetPreloadedAssets(pooledList.ToArray());
                    }

                    foreach (Object o in pooledList)
                    {
                        if (o is ScriptableSettings settings && settings.Type.IsEditorOnly())
                        {
                            Debug.LogError($"{settings} is editor-only but is set to preload. This is not supported!", settings);
                        }
                    }
                }
            }

            EditorApplication.delayCall += initialize;
        }
    }
}
