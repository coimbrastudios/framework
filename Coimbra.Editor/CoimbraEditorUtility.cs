using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra.Editor
{
    /// <summary>
    /// General editor utilities.
    /// </summary>
    [InitializeOnLoad]
    public static class CoimbraEditorUtility
    {
#if !UNITY_2021_3_OR_NEWER
        private const string ClearConsoleOnReloadKey = KeyPrefix + nameof(ClearConsoleOnReloadKey);

        private const string ClearConsoleOnReloadItem = CoimbraUtility.PreferencesMenuPath + "Clear Console On Reload";
#endif

        private const string KeyPrefix = "Coimbra.Editor.FrameworkEditorUtility.";

        private const string ResetPlayModeStartSceneMenuItem = CoimbraUtility.ToolsMenuPath + "Reset Play Mode Start Scene";

        static CoimbraEditorUtility()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
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

        /// <inheritdoc cref="AssetDatabase.ForceReserializeAssets()"/>
        [MenuItem(CoimbraUtility.ToolsMenuPath + "Force Reserialize Assets")]
        public static void ForceReserializeAssets()
        {
            AssetDatabase.ForceReserializeAssets();
        }

        /// <inheritdoc cref="EditorUtility.RequestScriptReload()"/>
        [MenuItem(CoimbraUtility.ToolsMenuPath + "Request Script Reload")]
        public static void RequestScriptReload()
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
    }
}
