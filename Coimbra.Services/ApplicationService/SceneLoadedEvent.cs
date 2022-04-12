using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Coimbra.Services
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html">sceneLoaded</a>.
    /// </summary>
    [PublicAPI]
    public readonly struct SceneLoadedEvent : IEvent
    {
        /// <summary>
        /// The <see cref="Scene"/> loaded.
        /// </summary>
        public Scene Scene { get; }

        /// <summary>
        /// The <see cref="LoadSceneMode"/> used.
        /// </summary>
        public LoadSceneMode LoadSceneMode { get; }

        public SceneLoadedEvent(Scene scene, LoadSceneMode loadSceneMode)
        {
            Scene = scene;
            LoadSceneMode = loadSceneMode;
        }
    }
}
