using UnityEngine;

namespace Coimbra
{
    [AddComponentMenu("")]
    internal sealed class ActorManager : MonoBehaviour
    {
        private void Awake()
        {
            foreach (Actor actor in FindObjectsOfType<Actor>(true))
            {
                actor.Initialize();
            }
        }

        private void Start()
        {
            gameObject.Dispose(true);
        }
    }
}
