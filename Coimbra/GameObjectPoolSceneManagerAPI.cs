using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra
{
    /// <summary>
    /// Specialized <see cref="SceneManagerAPI"/> to avoid issues when using <see cref="GameObjectPool"/>.
    /// </summary>
    public class GameObjectPoolSceneManagerAPI : SceneManagerAPI
    {
        /// <inheritdoc/>
        protected override AsyncOperation UnloadSceneAsyncByNameOrIndex(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess)
        {
            IReadOnlyList<Actor> pooledActors = Actor.GetPooledActors();
            Scene scene;

            if (sceneBuildIndex < 0)
            {
                scene = SceneManager.GetSceneByName(sceneName);

                if (!scene.IsValid())
                {
                    scene = SceneManager.GetSceneByPath(sceneName);
                }
            }
            else
            {
                scene = GetSceneByBuildIndex(sceneBuildIndex);
            }

            if (!scene.IsValid())
            {
                return base.UnloadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, immediately, options, out outSuccess);
            }

            for (int i = 0; i < pooledActors.Count; i++)
            {
                Actor actor = pooledActors[i];

                if (actor.CachedGameObject.scene.handle != scene.handle)
                {
                    if (actor.Pool.CachedGameObject.scene.handle == scene.handle)
                    {
                        actor.Pool = null;
                    }

                    continue;
                }

                if (actor.Pool.CachedGameObject.scene.handle == scene.handle)
                {
                    continue;
                }

                if (actor.Pool.KeepParentOnDespawn)
                {
                    actor.CachedTransform.SetParent(actor.Pool.CachedTransform, false);
                }

                actor.Despawn();
            }

            return base.UnloadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, immediately, options, out outSuccess);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void HandleBeforeSceneLoad()
        {
            overrideAPI ??= new GameObjectPoolSceneManagerAPI();
        }
    }
}
