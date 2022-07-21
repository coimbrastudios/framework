#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Event service that also has encapsulation for some of its methods.
    /// </summary>
    [RequiredService]
    [RequireImplementors]
    public interface IEventService : IService
    {
        /// <summary>
        /// Adds a listener to an event type.
        /// </summary>
        /// <param name="eventHandler">The callback to be added.</param>
        /// <typeparam name="TEvent">The event type.</typeparam>
        EventHandle AddListener<TEvent>(in EventContextHandler<TEvent> eventHandler)
            where TEvent : IEvent;

        /// <summary>
        /// Adds a listener for when an event starts/stops being relevant.
        /// </summary>
        /// <param name="relevancyChangedHandler">The handler to add.</param>
        /// <typeparam name="TEvent">The event type.</typeparam>
        void AddRelevancyListener<TEvent>(in EventRelevancyChangedHandler relevancyChangedHandler)
            where TEvent : IEvent;

        /// <summary>
        /// Checks if an event type contains any listeners.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>The number of listeners for this event on this service.</returns>
        int GetListenerCount<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        /// Checks if an event type contains any listeners.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <returns>The number of listeners for this event on this service.</returns>
        int GetListenerCount(Type eventType);

        /// <summary>
        /// Gets the listeners for the given event handle.
        /// </summary>
        int GetListeners(in EventHandle eventHandle, List<DelegateListener> listeners);

        /// <summary>
        /// Gets the listeners for the given event type.
        /// </summary>
        int GetListeners<TEvent>(List<DelegateListener> listeners)
            where TEvent : IEvent;

        /// <summary>
        /// Gets the listeners for the given event type.
        /// </summary>
        int GetListeners(Type eventType, List<DelegateListener> listeners);

        /// <summary>
        /// Gets the relevancy listeners for the given event type.
        /// </summary>
        int GetRelevancyListeners<TEvent>(List<DelegateListener> listeners)
            where TEvent : IEvent;

        /// <summary>
        /// Gets the relevancy listeners for the given event type.
        /// </summary>
        int GetRelevancyListeners(Type eventType, List<DelegateListener> listeners);

        /// <summary>
        /// Checks if the event handle belongs to this service.
        /// </summary>
        /// <param name="eventHandle">The event handle.</param>
        /// <returns>True if the event handle was created by this service and is still valid.</returns>
        bool HasListener(in EventHandle eventHandle);

        /// <summary>
        /// Checks if an event is currently being invoked.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>True if the event is currently being invoked.</returns>
        bool IsInvoking<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        /// Checks if an event is currently being invoked.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <returns>True if the event is currently being invoked.</returns>
        bool IsInvoking(Type eventType);

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="eventSender">The object invoking the event.</param>
        /// <param name="eventData">The event data to be sent.</param>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>True if the event was actually invoked.</returns>
        bool Invoke<TEvent>(object eventSender, in TEvent eventData)
            where TEvent : IEvent;

        /// <summary>
        /// Removes all listeners from an event type.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>True if removed any listener for the specified event type.</returns>
        bool RemoveAllListeners<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        /// Removes a listener from an event with its handle.
        /// </summary>
        /// <param name="eventHandle">The event handle.</param>
        /// <returns>True if the event handle was a valid listener for this service.</returns>
        bool RemoveListener(in EventHandle eventHandle);

        /// <summary>
        /// Removes a listener for when an event starts/stops being relevant.
        /// </summary>
        /// <param name="relevancyChangedHandler">The handler to remove.</param>
        /// <typeparam name="TEvent">The event type.</typeparam>
        void RemoveRelevancyListener<TEvent>(in EventRelevancyChangedHandler relevancyChangedHandler)
            where TEvent : IEvent;
    }
}
