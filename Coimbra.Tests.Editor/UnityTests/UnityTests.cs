using Coimbra.Tests.UnityTests;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Coimbra.Tests.Editor.UnityTests
{
    [TestFixture]
    public class UnityTests
    {
        [UnityTest]
        [Timeout(1000)]
        [SuppressMessage("ReSharper", "Unity.LoadSceneWrongIndex", Justification = "Scene index is added during test execution.")]
        public IEnumerator GivenEmptyScene_WhenSceneLoaded_ThenSceneChangedTriggersAfterAwake_AndBeforeStart()
        {
            string emptyScene = AssetDatabase.GUIDToAssetPath("85c5db32df4e15a44abbf3f73a58c060");
            string logBehaviourScene = AssetDatabase.GUIDToAssetPath("ee2ecbf268ef12940864202362b6e8b7");
            SceneAsset savedStartScene = EditorSceneManager.playModeStartScene;
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(emptyScene);
            EditorBuildSettingsScene[] savedBuildScenes = EditorBuildSettings.scenes;
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
            {
                new(emptyScene, true),
                new(logBehaviourScene, true),
            };

            yield return new EnterPlayMode();

            SceneManager.sceneLoaded += HandleSceneLoaded;
            LogAssert.Expect(LogType.Log, LogBehaviour.AwakeLog);
            LogAssert.Expect(LogType.Log, LogBehaviour.EnableLog);
            LogAssert.Expect(LogType.Log, nameof(HandleSceneLoaded));
            LogAssert.Expect(LogType.Log, LogBehaviour.StartLog);
            SceneManager.LoadScene(1, LoadSceneMode.Single);

            yield return null;

            SceneManager.sceneLoaded -= HandleSceneLoaded;
            LogAssert.NoUnexpectedReceived();

            yield return new ExitPlayMode();

            EditorSceneManager.playModeStartScene = savedStartScene;
            EditorBuildSettings.scenes = savedBuildScenes;
        }

        [UnityTest]
        [Timeout(1000)]
        [SuppressMessage("ReSharper", "Unity.LoadSceneWrongIndex", Justification = "Scene index is added during test execution.")]
        public IEnumerator GivenEmptyScene_WhenSceneLoadedAsAdditive_ThenSceneChangedTriggersAfterAwake_AndBeforeStart()
        {
            string emptyScene = AssetDatabase.GUIDToAssetPath("85c5db32df4e15a44abbf3f73a58c060");
            string logBehaviourScene = AssetDatabase.GUIDToAssetPath("ee2ecbf268ef12940864202362b6e8b7");
            SceneAsset savedStartScene = EditorSceneManager.playModeStartScene;
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(emptyScene);
            EditorBuildSettingsScene[] savedBuildScenes = EditorBuildSettings.scenes;
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
            {
                new(emptyScene, true),
                new(logBehaviourScene, true),
            };

            yield return new EnterPlayMode();

            SceneManager.sceneLoaded += HandleSceneLoaded;
            LogAssert.Expect(LogType.Log, LogBehaviour.AwakeLog);
            LogAssert.Expect(LogType.Log, LogBehaviour.EnableLog);
            LogAssert.Expect(LogType.Log, nameof(HandleSceneLoaded));
            LogAssert.Expect(LogType.Log, LogBehaviour.StartLog);
            SceneManager.LoadScene(1, LoadSceneMode.Additive);

            yield return null;

            SceneManager.sceneLoaded -= HandleSceneLoaded;
            LogAssert.NoUnexpectedReceived();

            yield return new ExitPlayMode();

            EditorSceneManager.playModeStartScene = savedStartScene;
            EditorBuildSettings.scenes = savedBuildScenes;
        }

        [UnityTest]
        [Timeout(1000)]
        [SuppressMessage("ReSharper", "Unity.LoadSceneWrongIndex", Justification = "Scene index is added during test execution.")]
        public IEnumerator GivenEmptyScene_WhenSceneLoadedAsync_ThenSceneChangedTriggersAfterAwake_AndBeforeStart()
        {
            string emptyScene = AssetDatabase.GUIDToAssetPath("85c5db32df4e15a44abbf3f73a58c060");
            string logBehaviourScene = AssetDatabase.GUIDToAssetPath("ee2ecbf268ef12940864202362b6e8b7");
            SceneAsset savedStartScene = EditorSceneManager.playModeStartScene;
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(emptyScene);
            EditorBuildSettingsScene[] savedBuildScenes = EditorBuildSettings.scenes;
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
            {
                new(emptyScene, true),
                new(logBehaviourScene, true),
            };

            yield return new EnterPlayMode();

            SceneManager.sceneLoaded += HandleSceneLoaded;
            LogAssert.Expect(LogType.Log, LogBehaviour.AwakeLog);
            LogAssert.Expect(LogType.Log, LogBehaviour.EnableLog);
            LogAssert.Expect(LogType.Log, nameof(HandleSceneLoaded));
            LogAssert.Expect(LogType.Log, LogBehaviour.StartLog);

            yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

            SceneManager.sceneLoaded -= HandleSceneLoaded;
            LogAssert.NoUnexpectedReceived();

            yield return new ExitPlayMode();

            EditorSceneManager.playModeStartScene = savedStartScene;
            EditorBuildSettings.scenes = savedBuildScenes;
        }

        [UnityTest]
        [Timeout(1000)]
        [SuppressMessage("ReSharper", "Unity.LoadSceneWrongIndex", Justification = "Scene index is added during test execution.")]
        public IEnumerator GivenEmptyScene_WhenSceneLoadedAsyncAsAdditive_ThenSceneChangedTriggersAfterAwake_AndBeforeStart()
        {
            string emptyScene = AssetDatabase.GUIDToAssetPath("85c5db32df4e15a44abbf3f73a58c060");
            string logBehaviourScene = AssetDatabase.GUIDToAssetPath("ee2ecbf268ef12940864202362b6e8b7");
            SceneAsset savedStartScene = EditorSceneManager.playModeStartScene;
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(emptyScene);
            EditorBuildSettingsScene[] savedBuildScenes = EditorBuildSettings.scenes;
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
            {
                new(emptyScene, true),
                new(logBehaviourScene, true),
            };

            yield return new EnterPlayMode();

            SceneManager.sceneLoaded += HandleSceneLoaded;
            LogAssert.Expect(LogType.Log, LogBehaviour.AwakeLog);
            LogAssert.Expect(LogType.Log, LogBehaviour.EnableLog);
            LogAssert.Expect(LogType.Log, nameof(HandleSceneLoaded));
            LogAssert.Expect(LogType.Log, LogBehaviour.StartLog);

            yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

            SceneManager.sceneLoaded -= HandleSceneLoaded;
            LogAssert.NoUnexpectedReceived();

            yield return new ExitPlayMode();

            EditorSceneManager.playModeStartScene = savedStartScene;
            EditorBuildSettings.scenes = savedBuildScenes;
        }

        private static void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Debug.Log(nameof(HandleSceneLoaded));
        }
    }
}
