using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Helper component to keep track of <see cref="EventHandle"/> for an <see cref="Actor"/>.
    /// </summary>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="IEventService"/>
    /// <seealso cref="EventHandle"/>
    /// <seealso cref="ActorComponentBase"/>
    public sealed class EventHandleTrackerComponent : ActorComponentBase, IDisposable, ISerializationCallbackReceiver
    {
        private readonly HashSet<EventHandle> _trackedHandles = new();

        [SerializeField]
        [Tooltip("Whether the tracked handles should be cleared if this component is being destroyed even if its Actor is not.")]
        private bool _clearOnComponentDestroyed = true;

        [SerializeField]
        [UsedImplicitly]
        [Tooltip("Current tracked handles.")]
        private List<EventHandle> _eventHandles = new();

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Clear"/> should be called also if the component is being destroyed without its <see cref="Actor"/>.
        /// </summary>
        public bool ClearOnComponentDestroyed
        {
            [DebuggerStepThrough]
            get => _clearOnComponentDestroyed;
            [DebuggerStepThrough]
            set => _clearOnComponentDestroyed = value;
        }

        /// <summary>
        /// Adds a new <see cref="EventHandle"/> to be tracked.
        /// </summary>
        /// <returns>False if the handle is already being tracked.</returns>
        public bool Add(in EventHandle eventHandle)
        {
            return _trackedHandles.Add(eventHandle);
        }

        /// <summary>
        /// Clears the tracked handles.
        /// </summary>
        /// <param name="removeListeners">If true, will also call <see cref="IEventService.RemoveListener"/> for each tracked handle.</param>
        public void Clear(bool removeListeners)
        {
            if (removeListeners)
            {
                foreach (EventHandle eventHandle in _trackedHandles)
                {
                    eventHandle.Service.GetValid()?.RemoveListener(in eventHandle);
                }
            }

            _trackedHandles.Clear();
        }

        /// <summary>
        /// Removes an existing <see cref="EventHandle"/> from the tracked handles.
        /// </summary>
        /// <returns>False if the handle wasn't being tracked already.</returns>
        public bool Remove(in EventHandle eventHandle)
        {
            return _trackedHandles.Remove(eventHandle);
        }

        /// <inheritdoc/>
        protected override void OnPreInitializeActor()
        {
            Actor.OnDestroying += HandleActorDestroying;
        }

        /// <inheritdoc/>
        protected override void OnPostInitializeActor() { }

        private void HandleActorDestroying(Actor sender, Actor.DestroyReason reason)
        {
            Clear(true);
        }

        /// <inheritdoc/>
        void IDisposable.Dispose()
        {
            if (Actor != null)
            {
                Actor.OnDestroying -= HandleActorDestroying;
            }

            if (_clearOnComponentDestroyed)
            {
                Clear(true);
            }
        }

        /// <inheritdoc/>
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        /// <inheritdoc/>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _eventHandles = _trackedHandles.ToList();
        }
    }
}
