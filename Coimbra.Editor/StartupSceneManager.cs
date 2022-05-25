using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra.Editor
{
    [InitializeOnLoad]
    internal static class StartupSceneManager
    {
        private const string EditorStartupSceneCategory = " ";

        private const string KeyPrefix = "Coimbra.Editor.FrameworkEditorUtility.";

        private const string PlayModeStartSceneKey = KeyPrefix + nameof(PlayModeStartSceneKey);

        [UserSetting(EditorStartupSceneCategory, "Editor Startup Scene", "The scene to use as the startup scene when inside the editor. If null, then no startup scene will be used.")]
        private static readonly ProjectSetting<SceneAsset> StartupScene = new ProjectSetting<SceneAsset>("General.EditorStartupScene", null);

        static StartupSceneManager()
        {
            EditorApplication.playModeStateChanged -= ConfigureStartupScene;
            EditorApplication.playModeStateChanged += ConfigureStartupScene;
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
    }
}
