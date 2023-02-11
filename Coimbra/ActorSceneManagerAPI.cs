using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra
{
    /// <summary>
    /// Custom <see cref="SceneManagerAPI"/> that does some pre-processing when unloading a scene.
    /// </summary>
    public class ActorSceneManagerAPI : SceneManagerAPI
    {
        /// <summary>
        /// Call this method from your custom <see cref="SceneManagerAPI"/> if not using the <see cref="ActorSceneManagerAPI"/>.
        /// </summary>
        public static void PreprocessUnloadScene(Scene scene)
        {
            Actor.InitializeActors();

            foreach (Actor actor in Actor.GetCachedActors())
            {
                actor.OnUnloadScene(scene);
            }
        }

        /// <summary>
        /// Utility method to get a valid <see cref="Scene"/> using either the name of the build index.
        /// </summary>
        /// <param name="name">The scene name. Used if build index is negative.</param>
        /// <param name="buildIndex">The scene build index. Used if non-negative.</param>
        /// <param name="scene">The scene found, if any.</param>
        /// <returns>True if found a valid scene.</returns>
        public static bool TryGetScene(string name, int buildIndex, out Scene scene)
        {
            if (buildIndex < 0)
            {
                scene = SceneManager.GetSceneByName(name);

                if (!scene.IsValid())
                {
                    scene = SceneManager.GetSceneByPath(name);
                }
            }
            else
            {
                scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            }

            return scene.IsValid();
        }

        /// <inheritdoc/>
        protected override AsyncOperation UnloadSceneAsyncByNameOrIndex(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess)
        {
            if (TryGetScene(sceneName, sceneBuildIndex, out Scene scene))
            {
                PreprocessUnloadScene(scene);
            }

            return base.UnloadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, immediately, options, out outSuccess);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void HandleBeforeSceneLoad()
        {
            overrideAPI ??= new ActorSceneManagerAPI();
        }
    }
}
