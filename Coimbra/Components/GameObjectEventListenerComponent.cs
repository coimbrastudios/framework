using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace Coimbra
{
    [Preserve]
    [AddComponentMenu("")]
    internal sealed class GameObjectEventListenerComponent : MonoBehaviour
    {
        internal event UnityAction<GameObject, bool> OnActiveStateChanged;

        internal event UnityAction<GameObject, DestroyEventType> OnDestroyEvent;

        private bool _isQuitting;

        private void OnEnable()
        {
            OnActiveStateChanged?.Invoke(gameObject, true);
        }

        private void OnDisable()
        {
            OnActiveStateChanged?.Invoke(gameObject, false);
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
            {
                OnDestroyEvent?.Invoke(gameObject, DestroyEventType.DestroyCall);
            }
            else
            {
                OnDestroyEvent?.Invoke(gameObject, _isQuitting ? DestroyEventType.ApplicationQuit : DestroyEventType.SceneChange);
            }

            gameObject.RemoveCachedEventListener();
        }
    }
}
