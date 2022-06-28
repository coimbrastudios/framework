using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    internal class ScriptableSettingsPostprocessor : AssetPostprocessor
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
                }

                foreach (string guid in AssetDatabase.FindAssets($"t:{nameof(ScriptableSettings)}"))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    ScriptableSettings settings = AssetDatabase.LoadAssetAtPath<ScriptableSettings>(path);

                    if (settings != null && settings.Type.IsEditorOnly())
                    {
                        Debug.LogError($"{settings} is editor-only but exists in the Assets folder. This is not supported!", settings);
                    }
                }
            }

            EditorApplication.delayCall += initialize;
        }
    }
}
