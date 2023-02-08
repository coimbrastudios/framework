using Coimbra.Services.Events;
using Coimbra.Services.PlayerLoopEvents;
using UnityEngine;

namespace Coimbra.Samples.DifficultySettings
{
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Samples/Difficulty Settings Cube")]
    [RequireComponent(typeof(EventHandleTrackerComponent))]
    public sealed class DifficultySettingsCube : Actor
    {
        [SerializeField]
        private float _maxOriginOffset = 5;

        private Vector3 _origin;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _origin = Transform.position;

            Debug.Assert(TryGetComponent(out EventHandleTrackerComponent eventHandleTrackerComponent));
            eventHandleTrackerComponent.Add(UpdateEvent.AddListener(HandleUpdate));
        }

        private void HandleUpdate(ref EventContext context, in UpdateEvent e)
        {
            if (ScriptableSettings.TryGet(out DifficultySettings difficultySettings))
            {
                float deltaMovement = difficultySettings.CubeMovementSpeed * e.DeltaTime;
                Transform.Translate(Vector3.forward * deltaMovement, Space.Self);

                Vector3 distance = Transform.position - _origin;
                float sqrMaxOriginOffset = _maxOriginOffset * _maxOriginOffset;

                if (distance.sqrMagnitude >= sqrMaxOriginOffset)
                {
                    // turn and get away from bounds
                    Transform.Rotate(Vector3.up, 180);
                    Transform.Translate(Vector3.forward * deltaMovement, Space.Self);
                }
            }
            else
            {
                Debug.LogWarning($"{typeof(DifficultySettings)} is not set!");
            }
        }
    }
}
