using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Coimbra.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(GameObjectBehaviour))]
    public class GameObjectBehaviourTests
    {
        private SceneAsset _savedStartScene;
        private EditorBuildSettingsScene[] _savedBuildSettingsScenes;

        [SetUp]
        public void SetUp()
        {
            _savedStartScene = EditorSceneManager.playModeStartScene;
            _savedBuildSettingsScenes = EditorBuildSettings.scenes;
        }

        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.playModeStartScene = _savedStartScene;
            EditorBuildSettings.scenes = _savedBuildSettingsScenes;
        }

        [UnityTest]
        [Timeout(10)]
        public IEnumerator GivenActiveInstance_WhenDestroyedByExitPlayMode_ThenResultIsApplicationQuit()
        {
            yield return new EnterPlayMode();

            const string logFormat = "OnDestroyed.reason = {0}";
            GameObjectBehaviour instance = new GameObject().AddComponent<GameObjectBehaviour>();
            instance.OnDestroyed += delegate(GameObject sender, DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.Expect(LogType.Log, string.Format(logFormat, DestroyReason.ApplicationQuit));

            yield return new ExitPlayMode();
        }

        [UnityTest]
        [Timeout(10)]
        public IEnumerator GivenActiveInstance_WhenDestroyedBySceneChange_ThenResultIsSceneChange()
        {
            string emptyScene = AssetDatabase.GUIDToAssetPath("85c5db32df4e15a44abbf3f73a58c060");
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(emptyScene);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(emptyScene, true),
            };

            yield return new EnterPlayMode();

            const string logFormat = "OnDestroyed.reason = {0}";
            GameObjectBehaviour instance = new GameObject().AddComponent<GameObjectBehaviour>();
            instance.OnDestroyed += delegate(GameObject sender, DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.Expect(LogType.Log, string.Format(logFormat, DestroyReason.SceneChange));
            SceneManager.LoadScene(0);

            yield return null;
            yield return new ExitPlayMode();
        }
    }
}
