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
        [UnityTest]
        [Timeout(10)]
        public IEnumerator GivenActiveInstance_WhenDestroyedByExitPlayMode_ThenResultIsApplicationQuit()
        {
            yield return new EnterPlayMode();

            const string logFormat = "OnDestroyed.reason = {0}";
            GameObjectBehaviour instance = new GameObject().AddComponent<GameObjectBehaviour>();
            instance.OnDestroyed += delegate(GameObjectBehaviour sender, DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.ignoreFailingMessages = true;
            LogAssert.Expect(LogType.Log, string.Format(logFormat, DestroyReason.ApplicationQuit));

            yield return new ExitPlayMode();
        }

        [UnityTest]
        [Timeout(10)]
        public IEnumerator GivenActiveInstance_WhenDestroyedBySceneChange_ThenResultIsSceneChange()
        {
            string emptyScene = AssetDatabase.GUIDToAssetPath("85c5db32df4e15a44abbf3f73a58c060");

            SceneAsset savedStartScene = EditorSceneManager.playModeStartScene;
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(emptyScene);
            EditorBuildSettingsScene[] savedBuildScenes = EditorBuildSettings.scenes;
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(emptyScene, true),
            };

            yield return new EnterPlayMode();

            const string logFormat = "OnDestroyed.reason = {0}";
            GameObjectBehaviour instance = new GameObject().AddComponent<GameObjectBehaviour>();
            instance.OnDestroyed += delegate(GameObjectBehaviour sender, DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.ignoreFailingMessages = true;
            LogAssert.Expect(LogType.Log, string.Format(logFormat, DestroyReason.SceneChange));
            SceneManager.LoadScene(0);

            yield return null;
            yield return new ExitPlayMode();

            EditorSceneManager.playModeStartScene = savedStartScene;
            EditorBuildSettings.scenes = savedBuildScenes;
        }
    }
}
