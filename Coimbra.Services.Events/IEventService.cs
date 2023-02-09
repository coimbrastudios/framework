#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Event service for strongly-typed events that also has encapsulation for some of its methods and offers some basic debugging features.
    /// </summary>
    /// <remarks>
    /// This implementation comes with some Roslyn Analyzers to ensure that the APIs are being used as designed, so only non-generic APIs can be used directly.
    /// Generic APIs are required to be accessed by the respective <see cref="IEvent"/> implementation's generated APIs, unless specified otherwise by some <see cref="AllowEventServiceUsageForAttribute"/> usage.
    /// <para></para>
    /// While you can create your own implementation for this <see cref="IEventService"/>, the provided default implementation at <see cref="EventSystem"/> should be both performant and generic enough for most projects.
    /// The default <see cref="EventSystem"/> also offers a custom drawer with useful debug information for each instance, alongside some safety mechanisms to maker easier to detect runtime issues.
    /// <para></para>
    /// You can listen for when an <see cref="IEvent"/> gets its first listener added or its last listener removed through the <see cref="AddRelevancyListener{TEvent}"/> API.
    /// </remarks>
    /// <seealso cref="AllowEventServiceUsageForAttribute"/>
    /// <seealso cref="IEvent"/>
    /// <seealso cref="EventHandle"/>
    /// <seealso cref="EventHandleTrackerComponent"/>
    /// <seealso cref="EventContext"/>
    /// <seealso cref="EventContextHandler{T}"/>
    /// <seealso cref="EventRelevancyChangedHandler"/>
    /// <seealso cref="EventSettings"/>
    /// <seealso cref="EventSystem"/>
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
