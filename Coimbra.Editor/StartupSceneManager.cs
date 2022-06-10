using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra.Editor
{
    /// <summary>
    /// Helper class for defining a startup scene for projects that requires some special initialization even when inside the editor.
    /// </summary>
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath, true, FileDirectory = CoimbraUtility.ProjectSettingsFilePath)]
    public sealed class StartupSceneManager : ScriptableSettings
    {
        [UserSetting]
        private static readonly CoimbraUserSetting<bool> HasSavedStartupScene;

        [UserSetting]
        private static readonly CoimbraUserSetting<SceneAsset> SavedStartupScene;

        [SerializeField]
        [Tooltip("The scene to use as the startup scene when inside the editor. If null, then no startup scene will be used.")]
        private SceneAsset _startupScene;

        static StartupSceneManager()
        {
            HasSavedStartupScene = new CoimbraUserSetting<bool>($"{typeof(StartupSceneManager).FullName}.{nameof(HasSavedStartupScene)}", false, SettingsScope.Project);
            SavedStartupScene = new CoimbraUserSetting<SceneAsset>($"{typeof(StartupSceneManager).FullName}.{nameof(SavedStartupScene)}", null, SettingsScope.Project);
            EditorApplication.playModeStateChanged -= ConfigureStartupScene;
            EditorApplication.playModeStateChanged += ConfigureStartupScene;
        }

        /// <summary>
        /// The scene to use as the startup scene when inside the editor. If null, then no startup scene will be used.
        /// </summary>
        public SceneAsset StartupScene
        {
            get => _startupScene;
            set => _startupScene = value;
        }

        private static void ConfigureStartupScene(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingPlayMode:
                case PlayModeStateChange.EnteredEditMode:
                {
                    if (HasSavedStartupScene.value)
                    {
                        EditorSceneManager.playModeStartScene = SavedStartupScene.value;
                        HasSavedStartupScene.SetValue(false, true);
                    }

                    break;
                }
            }

            ScriptableSettingsUtility.TryLoadOrCreate(out StartupSceneManager settings, FindSingle);
            Debug.Assert(settings);

            if (settings.StartupScene == null)
            {
                return;
            }

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                {
                    Scene currentScene = SceneManager.GetActiveScene();

                    if (HasSavedStartupScene.value || currentScene.buildIndex < 0 || currentScene.path == AssetDatabase.GetAssetPath(settings.StartupScene))
                    {
                        break;
                    }

                    SavedStartupScene.value = EditorSceneManager.playModeStartScene;
                    EditorSceneManager.playModeStartScene = settings.StartupScene;
                    Debug.Log($"Overriding Startup Scene to \"{EditorSceneManager.playModeStartScene}\"");
                    HasSavedStartupScene.SetValue(true, true);

                    break;
                }

                case PlayModeStateChange.EnteredPlayMode:
                {
                    if (!HasSavedStartupScene.value)
                    {
                        break;
                    }

                    EditorSceneManager.playModeStartScene = SavedStartupScene.value;
                    Debug.Log($"Reverting Startup Scene to \"{EditorSceneManager.playModeStartScene}\"");
                    HasSavedStartupScene.SetValue(false, true);

                    break;
                }
            }
        }
    }
}
