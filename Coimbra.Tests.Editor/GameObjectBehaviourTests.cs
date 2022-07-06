using NUnit.Framework;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Coimbra.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(Actor))]
    public class GameObjectBehaviourTests
    {
        [UnityTest]
        [Timeout(1000)]
        public IEnumerator GivenActiveInstance_WhenDestroyedByExitPlayMode_ThenResultIsApplicationQuit()
        {
            yield return new EnterPlayMode();

            const string logFormat = "OnDestroyed.reason = {0}";
            Actor instance = new GameObject().AddComponent<Actor>();
            instance.OnDestroying += delegate(Actor sender, Actor.DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.ignoreFailingMessages = true;
            LogAssert.Expect(LogType.Log, string.Format(logFormat, Actor.DestroyReason.ApplicationQuit));

            yield return new ExitPlayMode();
        }

        [UnityTest]
        [Timeout(1000)]
        [SuppressMessage("ReSharper", "Unity.LoadSceneWrongIndex", Justification = "Scene index is added during test execution.")]
        public IEnumerator GivenActiveInstance_WhenDestroyedBySceneChange_ThenResultIsSceneChange()
        {
            string emptyScene = AssetDatabase.GUIDToAssetPath("85c5db32df4e15a44abbf3f73a58c060");
            SceneAsset savedStartScene = EditorSceneManager.playModeStartScene;
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(emptyScene);
            EditorBuildSettingsScene[] savedBuildScenes = EditorBuildSettings.scenes;
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
            {
                new(emptyScene, true),
            };

            yield return new EnterPlayMode();

            const string logFormat = "OnDestroyed.reason = {0}";
            Actor instance = new GameObject().AddComponent<Actor>();
            instance.OnDestroying += delegate(Actor sender, Actor.DestroyReason reason)
            {
                Debug.LogFormat(logFormat, reason);
            };

            LogAssert.ignoreFailingMessages = true;
            LogAssert.Expect(LogType.Log, string.Format(logFormat, Actor.DestroyReason.SceneChange));
            SceneManager.LoadScene(0);

            yield return null;
            yield return new ExitPlayMode();

            EditorSceneManager.playModeStartScene = savedStartScene;
            EditorBuildSettings.scenes = savedBuildScenes;
        }
    }
}
