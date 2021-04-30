using UnityEngine;
using UnityEngine.Events;

namespace Coimbra
{
    [AddComponentMenu(CSFrameworkUtility.AddComponentMenuPath + "Game Object Event Listener")]
    public sealed class GameObjectEventListener : MonoBehaviour
    {
        public event UnityAction<GameObjectEventListener, bool> OnActiveStateChanged
        {
            add => _activeStateChangeEvent.AddListener(value);
            remove => _activeStateChangeEvent.RemoveListener(value);
        }

        public event UnityAction<GameObjectEventListener, DestroyEventType> OnDestroyEvent
        {
            add => _destroyEvent.AddListener(value);
            remove => _destroyEvent.RemoveListener(value);
        }

        [SerializeField] private UnityEvent<GameObjectEventListener, bool> _activeStateChangeEvent = new UnityEvent<GameObjectEventListener, bool>();
        [SerializeField] private UnityEvent<GameObjectEventListener, DestroyEventType> _destroyEvent = new UnityEvent<GameObjectEventListener, DestroyEventType>();

        private bool _isQuitting;

        private void OnEnable()
        {
            _activeStateChangeEvent?.Invoke(this, true);
        }

        private void OnDisable()
        {
            _activeStateChangeEvent?.Invoke(this, false);
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
            {
                _destroyEvent?.Invoke(this, DestroyEventType.DestroyCall);
            }
            else
            {
                _destroyEvent?.Invoke(this, _isQuitting ? DestroyEventType.ApplicationQuit : DestroyEventType.SceneChange);
            }
        }
    }
}
