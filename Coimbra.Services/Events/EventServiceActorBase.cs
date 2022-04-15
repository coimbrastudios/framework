using System;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Specialized <see cref="ServiceActorBase{T}"/> prepared for using its <see cref="IService.OwningLocator"/>'s <see cref="IEventService"/>.
    /// </summary>
    public abstract class EventServiceActorBase<T> : ServiceActorBase<T>, IService
        where T : class, IService
    {
        private IEventService _eventService;

        static EventServiceActorBase()
        {
            typeof(T).AssertInterfaceImplementsNotEqual<IService>();
        }

        protected EventServiceActorBase()
        {
            Type type = GetType();

            if (!type.IsAbstract)
            {
                type.AssertNonInterfaceImplements<IService>();
            }
        }

        /// <summary>
        /// Cached version of the current <see cref="IEventService"/> of the <see cref="ServiceActorBase{T}.OwningLocator"/>
        /// </summary>
        protected IEventService EventService
        {
            get => _eventService;
            private set
            {
                if (_eventService == value)
                {
                    return;
                }

                IEventService previous = _eventService;
                _eventService = value;
                OnEventServiceChanged(previous, _eventService);
            }
        }

        /// <summary>
        /// Fired both when the <see cref="ServiceActorBase{T}.OwningLocator"/> or its <see cref="IEventService"/> changes.
        /// </summary>
        /// <param name="previous">The previous <see cref="IEventService"/>. Can be from a different <see cref="ServiceLocator"/>.</param>
        /// <param name="current">The current <see cref="IEventService"/>. Will be from the current <see cref="ServiceActorBase{T}.OwningLocator"/>.</param>
        protected abstract void OnEventServiceChanged(IEventService previous, IEventService current);

        /// <inheritdoc/>
        protected override void OnDestroying()
        {
            base.OnDestroying();
            EventService = null;
        }

        private void HandleEventServiceChanged(IService previous, IService current)
        {
            EventService = current as IEventService;
        }

        void IService.OnOwningLocatorChanged(ServiceLocator previous, ServiceLocator current)
        {
            previous?.RemoveValueChangedListener<IEventService>(HandleEventServiceChanged);
            current?.AddValueChangedListener<IEventService>(HandleEventServiceChanged);
            EventService = current?.Get<IEventService>();
        }
    }
}
