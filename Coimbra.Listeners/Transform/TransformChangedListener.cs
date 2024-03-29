﻿using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Listen to <see cref="Transform"/>'s changes by managing the <see cref="Transform.hasChanged"/> property.
    /// </summary>
    /// <seealso cref="TransformChildrenChangedListener"/>
    /// <seealso cref="TransformParentChangedListener"/>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerLoopListenerBase))]
    [AddComponentMenu(CoimbraListenersUtility.TransformMenuPath + "Transform Changed Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Transform-hasChanged.html")]
    [MovedFrom(true, "Coimbra", "Coimbra")]
    public sealed class TransformChangedListener : ActorComponentBase, IDisposable
    {
        public delegate void EventHandler(TransformChangedListener sender);

        /// <summary>
        /// Invoked inside the given <see cref="PlayerLoopListener"/>.
        /// </summary>
        public event EventHandler OnTrigger;

        [SerializeField]
        [DisableOnPlayMode]
        private bool _resetPostInitializeActor = true;

        private PlayerLoopListenerBase _playerLoopListener;

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Transform.hasChanged"/> should be reset during <see cref="OnPostInitializeActor"/>.
        /// </summary>
        public bool ResetOnPostInitializeActor
        {
            get => _resetPostInitializeActor;
            set => _resetPostInitializeActor = value;
        }

        /// <summary>
        /// Gets the player loop listener this component depends on.
        /// </summary>
        public PlayerLoopListenerBase PlayerLoopListener => _playerLoopListener != null ? _playerLoopListener : _playerLoopListener = GetComponent<PlayerLoopListenerBase>();

        /// <inheritdoc/>
        protected override void OnPreInitializeActor()
        {
            Actor.OnDestroying += HandleActorDestroying;
            PlayerLoopListener.OnTrigger += HandlePlayerLoop;
        }

        /// <inheritdoc/>
        protected override void OnPostInitializeActor()
        {
            if (_resetPostInitializeActor)
            {
                Actor.Transform.hasChanged = false;
            }
        }

        private void HandleActorDestroying(Actor sender, Actor.DestroyReason reason)
        {
            PlayerLoopListener.OnTrigger -= HandlePlayerLoop;
        }

        private void HandlePlayerLoop(PlayerLoopListenerBase sender, float deltaTime)
        {
            if (!Actor.Transform.hasChanged)
            {
                return;
            }

            Actor.Transform.hasChanged = false;
            OnTrigger?.Invoke(this);
        }

        /// <inheritdoc/>
        void IDisposable.Dispose()
        {
            PlayerLoopListener.OnTrigger -= HandlePlayerLoop;
        }
    }
}
