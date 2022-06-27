using CoimbraInternal.Editor;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Coimbra.Editor
{
    /// <summary>
    /// General editor utilities.
    /// </summary>
    [InitializeOnLoad]
    public static class CoimbraEditorUtility
    {
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

        /// <summary>
        /// Deletes all <see cref="AssetBundle"/> and <see cref="UnityEngine.ProceduralMaterial"/> content that has been cached.
        /// </summary>
        [MenuItem(CoimbraUtility.ToolsMenuPath + "Clear Cache")]
        public static void ClearCache()
        {
            if (Caching.ClearCache())
            {
                Debug.Log("Deleted all AssetBundle and ProceduralMaterial content that has been cached.");
            }
            else
            {
                Debug.LogWarning("Failed to delete all AssetBundle and ProceduralMaterial content that has been cached.");
            }
        }

        /// <summary>
        /// Creates a runtime and an editor assembly for all scripts in the Assets folder.
        /// </summary>
        [MenuItem(CoimbraUtility.ToolsMenuPath + "Create Assets Assembly")]
        public static void CreateAssetsAssemblies()
        {
            AssetsAssemblyCreator.CreateAssetsAssemblies();
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

        /// <summary>
        /// Saves all assets changes to disk.
        /// </summary>
        [MenuItem(CoimbraUtility.ToolsMenuPath + "Save Assets &#S")]
        public static void SaveAllAssets()
        {
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Clears the console windows.
        /// </summary>
        public static void ClearConsoleWindow()
        {
            UnityEditorInternals.ClearLogEntries();
        }

        [MenuItem(ResetPlayModeStartSceneMenuItem, true)]
        private static bool CanResetPlayModeStartScene()
        {
            return EditorSceneManager.playModeStartScene != null;
        }

        private static void HandleBeforeAssemblyReload()
        {
            CoimbraUtility.IsReloadingScripts = true;
        }

        private static void HandleDelayCall()
        {
            CoimbraUtility.IsReloadingScripts = false;
        }
    }
}
