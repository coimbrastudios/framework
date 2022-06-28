using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Special component that listens to <see cref="Actor"/> initialization.
    /// </summary>
    [RequireComponent(typeof(Actor))]
    public abstract class ActorComponentBase : MonoBehaviour
    {
        private bool _hasActor;

        private Actor _actor;

        /// <summary>
        /// Gets the actor this component belongs to.
        /// </summary>
        public Actor Actor
        {
            get
            {
                if (_hasActor)
                {
                    return _actor;
                }

                _hasActor = true;
                _actor = gameObject.AsActor();

                return _actor;
            }
        }

        internal void PreInitialize(Actor actor)
        {
            _hasActor = true;
            _actor = actor;
            OnPreInitializeActor();
        }

        internal void PostInitialize()
        {
            OnPostInitializeActor();
        }

        /// <summary>
        /// Called before <see cref="Coimbra.Actor.OnInitialize"/>.
        /// </summary>
        protected abstract void OnPreInitializeActor();

        /// <summary>
        /// Called after <see cref="Coimbra.Actor.OnInitialize"/>.
        /// </summary>
        protected abstract void OnPostInitializeActor();
    }
}
