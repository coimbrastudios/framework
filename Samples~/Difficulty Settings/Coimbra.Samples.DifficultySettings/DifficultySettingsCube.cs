using Coimbra.Services.Events;
using Coimbra.Services.PlayerLoopEvents;
using UnityEngine;

namespace Coimbra.Samples.DifficultySettings
{
    /// <summary>
    /// A simple moving cube implementation that gets its speed from the current <see cref="DifficultySettings"/> set.
    /// </summary>
    /// <remarks>
    /// It also showcases the usage of the <see cref="UpdateEvent"/> and <see cref="EventHandleTrackerComponent"/>.
    /// </remarks>
    /// <seealso cref="DifficultyListSettings"/>
    /// <seealso cref="DifficultySettings"/>
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Samples/Difficulty Settings Cube")]
    [RequireComponent(typeof(EventHandleTrackerComponent))]
    public sealed class DifficultySettingsCube : Actor
    {
        [SerializeField]
        private float _maxOriginOffset = 5;

        private Vector3 _origin;

        /// <inheritdoc/>
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
