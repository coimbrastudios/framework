using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// General editor utilities.
    /// </summary>
    [InitializeOnLoad]
    public sealed class CoimbraEditorUtility : AssetPostprocessor
    {
#if !UNITY_2021_3_OR_NEWER
        private const string ClearConsoleOnReloadKey = KeyPrefix + nameof(ClearConsoleOnReloadKey);

        private const string ClearConsoleOnReloadItem = CoimbraUtility.PreferencesMenuPath + "Clear Console On Reload";
#endif

        private const string KeyPrefix = "Coimbra.Editor.FrameworkEditorUtility.";

        private const string PlayModeStartSceneKey = KeyPrefix + nameof(PlayModeStartSceneKey);

        private const string EditorStartupSceneCategory = "Editor Startup Scene";

        private const string ResetPlayModeStartSceneMenuItem = CoimbraUtility.ToolsMenuPath + "Reset Play Mode Start Scene";

        [UserSetting(EditorStartupSceneCategory, "Editor Startup Scene", "The scene to use as the startup scene when inside the editor. If invalid, then no startup scene will be used.")]
        private static readonly ProjectSetting<SceneAsset> StartupScene = new ProjectSetting<SceneAsset>("General.EditorStartupScene", null);

        static CoimbraEditorUtility()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
            EditorApplication.playModeStateChanged -= ConfigureStartupScene;
            EditorApplication.playModeStateChanged += ConfigureStartupScene;
            EditorApplication.delayCall += HandleDelayCall;
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
        /// Reset the <see cref="EditorSceneManager.playModeStartScene"/> back to null.
        /// </summary>
        [MenuItem(ResetPlayModeStartSceneMenuItem)]
        public static void ResetPlayModeStartScene()
        {
            EditorSceneManager.playModeStartScene = null;
        }

#if !UNITY_2021_3_OR_NEWER
        /// <summary>
        /// Toggles the option set for clearing the console on script reloads.
        /// </summary>
        [MenuItem(ClearConsoleOnReloadItem)]
        public static void ToggleClearConsoleOnReload()
        {
            bool value = !EditorPrefs.GetBool(ClearConsoleOnReloadKey, false);
            EditorPrefs.SetBool(ClearConsoleOnReloadKey, value);
            Menu.SetChecked(ClearConsoleOnReloadItem, value);
        }
#endif

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
        public static void CreateAssetWithFolderHierarchy(Object asset, string path)
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

        [MenuItem(ResetPlayModeStartSceneMenuItem, true)]
        private static bool CanResetPlayModeStartScene()
        {
            return EditorSceneManager.playModeStartScene != null;
        }

        private static void ConfigureStartupScene(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingPlayMode:
                case PlayModeStateChange.EnteredEditMode:
                {
                    EditorSceneManager.playModeStartScene = null;

                    break;
                }
            }

            if (StartupScene.value == null || EditorSceneManager.playModeStartScene != null)
            {
                return;
            }

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                {
                    Scene currentScene = SceneManager.GetActiveScene();

                    if (currentScene.buildIndex < 0 || currentScene.path == AssetDatabase.GetAssetPath(StartupScene.value))
                    {
                        break;
                    }

                    EditorSceneManager.playModeStartScene = StartupScene.value;
                    Debug.LogWarning($"Editor Startup Scene: \"{EditorSceneManager.playModeStartScene}\"");

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

        private static void HandleBeforeAssemblyReload()
        {
#if !UNITY_2021_3_OR_NEWER
            if (EditorPrefs.GetBool(ClearConsoleOnReloadKey, false))
            {
                ClearConsoleWindow();
            }
#endif
            CoimbraUtility.IsReloadingScripts = true;
        }

        private static void HandleDelayCall()
        {
#if !UNITY_2021_3_OR_NEWER
            bool value = EditorPrefs.GetBool(ClearConsoleOnReloadKey, false);
            Menu.SetChecked(ClearConsoleOnReloadItem, value);
#endif
            CoimbraUtility.IsReloadingScripts = false;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
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
        }
    }
}
