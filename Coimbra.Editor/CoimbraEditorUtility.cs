using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra.Editor
{
    /// <summary>
    /// General editor utilities.
    /// </summary>
    [InitializeOnLoad]
    public sealed class CoimbraEditorUtility : AssetPostprocessor
    {
        private const string KeyPrefix = "Coimbra.Editor.FrameworkEditorUtility.";

        private const string PlayModeStartSceneKey = KeyPrefix + nameof(PlayModeStartSceneKey);

        private const string EditorStartupSceneCategory = "Editor Startup Scene";

        [UserSetting(EditorStartupSceneCategory, "Editor Startup Scene Index", "The scene index to use as the startup scene when inside the editor. If invalid, then no startup scene will be used.")]
        private static readonly ProjectSetting<int> StartupSceneIndex = new ProjectSetting<int>("General.EditorStartupSceneIndex", -1);

        static CoimbraEditorUtility()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
            EditorApplication.playModeStateChanged -= ConfigureStartupScene;
            EditorApplication.playModeStateChanged += ConfigureStartupScene;
            EditorApplication.delayCall += () => CoimbraUtility.IsReloadingScripts = false;
        }

        /// <summary>
        /// Asserts that all types that inherits from a serializable type are also serializable.
        /// </summary>
        [MenuItem(CoimbraUtility.ToolsMenuPath + "Assert Serializable Types")]
        public static void AssertSerializableTypes()
        {
            foreach (Type serializableType in TypeCache.GetTypesWithAttribute<SerializableAttribute>())
            {
                if ((serializableType.Attributes & TypeAttributes.Serializable) == 0)
                {
                    continue;
                }

                foreach (Type derivedType in TypeCache.GetTypesDerivedFrom(serializableType))
                {
                    bool condition = (derivedType.Attributes & TypeAttributes.Serializable) != 0;
                    string message = $"{derivedType.FullName} is not serializable and inherits from {serializableType.FullName} that is serializable!";
                    Debug.Assert(condition, message);
                }
            }
        }

        /// <summary>
        /// Requests a script reload.
        /// </summary>
        [MenuItem(CoimbraUtility.ToolsMenuPath + "Reload Scripts")]
        public static void ReloadScripts()
        {
            EditorUtility.RequestScriptReload();
        }

        /// <summary>
        /// Clears the console windows.
        /// </summary>
        public static void ClearConsoleWindow()
        {
            UnityInternals.ClearLogEntries();
        }

        /// <summary>
        /// Create an asset alongside its folder hierarchy if needed.
        /// </summary>
        public static void CreateAssetWithFolderHierarchy(UnityEngine.Object asset, string path)
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

        private static void HandleBeforeAssemblyReload()
        {
            CoimbraUtility.IsReloadingScripts = true;
        }

        private static void ConfigureStartupScene(PlayModeStateChange state)
        {
            if (StartupSceneIndex.value < 0 || StartupSceneIndex.value >= SceneManager.sceneCountInBuildSettings)
            {
                return;
            }

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                {
                    int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

                    if (currentSceneIndex >= 0 && currentSceneIndex != StartupSceneIndex.value)
                    {
                        SessionState.SetString(PlayModeStartSceneKey, AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene));
                        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[StartupSceneIndex.value].path);
                        Debug.LogWarning($"Editor Startup Scene: \"{EditorSceneManager.playModeStartScene}\"");
                    }

                    break;
                }

                case PlayModeStateChange.EnteredPlayMode:
                {
                    const string invalid = "<null>";
                    string playModeStartScene = SessionState.GetString(PlayModeStartSceneKey, invalid);

                    if (playModeStartScene != invalid)
                    {
                        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(playModeStartScene);
                        SessionState.EraseString(PlayModeStartSceneKey);
                    }

                    break;
                }
            }
        }

#if  UNITY_2021_3_OR_NEWER
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
#else
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
#endif
        {
            using (SharedManagedPools.Pop(out List<UnityEngine.Object> pooledList))
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
        }
    }
}
