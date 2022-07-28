using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra.Editor
{
    /// <summary>
    /// Helper class for defining a startup scene for projects that requires some special initialization even when inside the editor.
    /// </summary>
    [InitializeOnLoad]
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath, true, FileDirectory = CoimbraUtility.ProjectSettingsFilePath)]
    public sealed class StartupSceneManager : ScriptableSettings
    {
        [SerializeField]
        [Tooltip("The scene to use as the startup scene when inside the editor. If null, then no startup scene will be used.")]
        private SceneAsset _startupScene;

        static StartupSceneManager()
        {
            EditorApplication.playModeStateChanged -= ConfigureStartupScene;
            EditorApplication.playModeStateChanged += ConfigureStartupScene;
        }

        /// <summary>
        /// Gets or sets the scene to use as the startup scene when inside the editor. If null, then no startup scene will be used.
        /// </summary>
        public SceneAsset StartupScene
        {
            get => _startupScene;
            set => _startupScene = value;
        }

        private static void ConfigureStartupScene(PlayModeStateChange state)
        {
            StartupSceneInternalSettings internalSettings = GetInternalSettings(state);
            Debug.Assert(ScriptableSettingsUtility.TryLoadOrCreate(out StartupSceneManager settings));

            if (settings.StartupScene == null)
            {
                return;
            }

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                {
                    Scene currentScene = SceneManager.GetActiveScene();

                    if (internalSettings.HasSavedStartupScene || currentScene.buildIndex < 0 || currentScene.path == AssetDatabase.GetAssetPath(settings.StartupScene))
                    {
                        break;
                    }

                    internalSettings.SavedStartupScene = EditorSceneManager.playModeStartScene;
                    internalSettings.HasSavedStartupScene = true;
                    EditorSceneManager.playModeStartScene = settings.StartupScene;
                    Debug.Log($"Overriding Startup Scene to \"{EditorSceneManager.playModeStartScene}\"");
                    internalSettings.Save();

                    break;
                }

                case PlayModeStateChange.EnteredPlayMode:
                {
                    if (!internalSettings.HasSavedStartupScene)
                    {
                        break;
                    }

                    EditorSceneManager.playModeStartScene = internalSettings.SavedStartupScene;
                    Debug.Log($"Reverting Startup Scene to \"{EditorSceneManager.playModeStartScene}\"");

                    internalSettings.HasSavedStartupScene = false;
                    internalSettings.Save();

                    break;
                }
            }
        }

        private static StartupSceneInternalSettings GetInternalSettings(PlayModeStateChange state)
        {
            Debug.Assert(ScriptableSettingsUtility.TryLoadOrCreate(out StartupSceneInternalSettings internalSettings));

            switch (state)
            {
                case PlayModeStateChange.ExitingPlayMode:
                case PlayModeStateChange.EnteredEditMode:
                {
                    if (internalSettings.HasSavedStartupScene)
                    {
                        EditorSceneManager.playModeStartScene = internalSettings.SavedStartupScene;
                        internalSettings.HasSavedStartupScene = false;
                        internalSettings.Save();
                    }

                    break;
                }
            }

            return internalSettings;
        }
    }
}
