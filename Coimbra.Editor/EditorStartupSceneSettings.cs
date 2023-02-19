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
    [ScriptableSettingsProvider(typeof(LoadOrCreateScriptableSettingsProvider))]
    public sealed class EditorStartupSceneSettings : ScriptableSettings
    {
        [SerializeField]
        [Tooltip("The scene to use as the startup scene when inside the editor. If null, then no startup scene will be used.")]
        private SceneAsset _startupScene;

        static EditorStartupSceneSettings()
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
            EditorStartupSceneStateSettings stateSettings = GetInternalSettings(state);
            EditorStartupSceneSettings settings = Get<EditorStartupSceneSettings>(true);

            if (settings.StartupScene == null)
            {
                return;
            }

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                {
                    Scene currentScene = SceneManager.GetActiveScene();

                    if (stateSettings.HasSavedStartupScene || currentScene.buildIndex < 0 || currentScene.path == AssetDatabase.GetAssetPath(settings.StartupScene))
                    {
                        break;
                    }

                    stateSettings.SavedStartupScene = EditorSceneManager.playModeStartScene;
                    stateSettings.HasSavedStartupScene = true;
                    EditorSceneManager.playModeStartScene = settings.StartupScene;
                    Debug.Log($"Overriding Startup Scene to \"{EditorSceneManager.playModeStartScene}\"");
                    stateSettings.Save();

                    break;
                }

                case PlayModeStateChange.EnteredPlayMode:
                {
                    if (!stateSettings.HasSavedStartupScene)
                    {
                        break;
                    }

                    EditorSceneManager.playModeStartScene = stateSettings.SavedStartupScene;
                    Debug.Log($"Reverting Startup Scene to \"{EditorSceneManager.playModeStartScene}\"");

                    stateSettings.HasSavedStartupScene = false;
                    stateSettings.Save();

                    break;
                }
            }
        }

        private static EditorStartupSceneStateSettings GetInternalSettings(PlayModeStateChange state)
        {
            EditorStartupSceneStateSettings internalSettings = Get<EditorStartupSceneStateSettings>(true);

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
