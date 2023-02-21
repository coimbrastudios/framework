#nullable enable

using CoimbraInternal.Editor;
using System;
using System.Collections.Generic;
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
        private const string ResetPlayModeStartSceneMenuItem = CoimbraUtility.ToolsMenuPath + "Reset Play Mode Start Scene";

        static CoimbraEditorUtility()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
            UnityEditorInternals.OnEditorApplicationFocusChanged -= HandleEditorApplicationFocusChanged;
            UnityEditorInternals.OnEditorApplicationFocusChanged += HandleEditorApplicationFocusChanged;
            EditorApplication.delayCall += HandleDelayCall;
            ScriptableSettings.IsQuitting = false;
            PlayerSettings.GetPreloadedAssets();
        }

        /// <summary>
        /// Asserts that all types that inherits from a serializable type also contains the <see cref="SerializableAttribute"/>. It will also log a message if everything is correct.
        /// </summary>
        [MenuItem(CoimbraUtility.ToolsMenuPath + "Assert Serializable Types")]
        public static void AssertSerializableTypes()
        {
            bool hadIssue = false;

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
                    hadIssue |= condition;
                    Debug.Assert(condition, message);
                }
            }

            if (!hadIssue)
            {
                Debug.Log("All types that inherits from a serializable type contains the expected SerializableAttribute.");
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
        /// Generate assemblies for all scripts in the Assets folder while also taking into consideration Editor folders. Needs to be triggered everytime a new third-party is imported.
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

        [MenuItem(ResetPlayModeStartSceneMenuItem, true)]
        private static bool CanResetPlayModeStartScene()
        {
            return EditorSceneManager.playModeStartScene != null;
        }

        private static void HandleBeforeAssemblyReload()
        {
            ApplicationUtility.IsReloadingScripts = true;
        }

        private static void HandleDelayCall()
        {
            ApplicationUtility.IsReloadingScripts = false;
        }

        private static void HandleEditorApplicationFocusChanged(bool isFocused)
        {
            foreach (KeyValuePair<Type, ScriptableSettings?> pair in ScriptableSettings.Instances)
            {
                if (pair.Value != null && pair.Value.TryGetValid(out ScriptableSettings? value))
                {
                    value.ReloadAsset();
                }
            }
        }

        private static void HandlePlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                {
                    if (Object.FindObjectOfType<ActorManager>().TryGetValid(out ActorManager actorManager))
                    {
                        Object.DestroyImmediate(actorManager.gameObject);
                    }

                    ScriptableSettings.IsQuitting = false;
                    Actor.OnPlayModeStateChanged();

                    break;
                }

                case PlayModeStateChange.ExitingEditMode:
                {
                    Actor.OnPlayModeStateChanged();
                    PlayerSettings.GetPreloadedAssets();

                    if (EditorSettings.enterPlayModeOptionsEnabled && (EditorSettings.enterPlayModeOptions & EnterPlayModeOptions.DisableSceneReload) != 0)
                    {
                        _ = new GameObject(nameof(ActorManager), typeof(ActorManager));
                    }

                    break;
                }
            }
        }
    }
}
