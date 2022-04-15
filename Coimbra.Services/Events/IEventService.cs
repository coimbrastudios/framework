using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Event service that also has encapsulation for some of its methods.
    /// </summary>
    [RequireImplementors]
    public interface IEventService : IService
    {
        /// <summary>
        /// Delegate for events related to a specific event type of an <see cref="IEventService"/>.
        /// </summary>
        public delegate void EventHandler(IEventService eventService, Type eventType);

        /// <summary>
        /// Invoked when the first listener of an event is added.
        /// </summary>
        event EventHandler OnFirstListenerAdded;

        /// <summary>
        /// Invoked when the last listener of an event is removed.
        /// </summary>
        event EventHandler OnLastListenerRemoved;

        /// <summary>
        /// Adds a listener to an event type.
        /// </summary>
        /// <param name="eventCallback">The callback to be added.</param>
        /// <typeparam name="T">The event type.</typeparam>
        EventHandle AddListener<T>(EventData<T>.Handler eventCallback)
            where T : IEvent;

        /// <summary>
        /// Adds a listener to an event type.
        /// </summary>
        /// <param name="eventCallback">The callback to be added.</param>
        /// <param name="appendList">List to add the event handle if generated a valid one.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if a valid <see cref="EventHandle"/> was generated.</returns>
        bool AddListener<T>(EventData<T>.Handler eventCallback, List<EventHandle> appendList)
            where T : IEvent;

        /// <summary>
        /// Compares the event key for specified type.
        /// </summary>
        /// <param name="eventKey">The event key to compare to.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if the current event key matches the <see cref="eventKey"/> parameter.</returns>
        bool CompareEventKey<T>(EventKey eventKey)
            where T : IEvent;

        /// <summary>
        /// Compares the event key for specified type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventKey">The event key to compare to.</param>
        /// <returns>True if the current event key matches the <see cref="eventKey"/> parameter.</returns>
        bool CompareEventKey(Type eventType, EventKey eventKey);

        /// <summary>
        /// Checks if an event type contains any listeners.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if there is any listeners for this event on this service.</returns>
        bool HasAnyListeners<T>()
            where T : IEvent;

        /// <summary>
        /// Checks if an event type contains any listeners.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <returns>True if there is any listeners for this event on this service.</returns>
        bool HasAnyListeners(Type eventType);

        /// <summary>
        /// Checks if the event handle belongs to this service.
        /// </summary>
        /// <param name="eventHandle">The event handle.</param>
        /// <returns>True if the event handle was created by this service and is still valid.</returns>
        bool HasListener(in EventHandle eventHandle);

        /// <summary>
        /// Checks if an event is currently being invoked.
        /// </summary>
        /// <typeparam name="T">The event type</typeparam>
        /// <returns>True if the event is currently being invoked.</returns>
        bool IsInvoking<T>()
            where T : IEvent;

        /// <summary>
        /// Checks if an event is currently being invoked.
        /// </summary>
        /// <param name="eventType">The event type</param>
        /// <returns>True if the event is currently being invoked.</returns>
        bool IsInvoking(Type eventType);

        /// <summary>
        /// Invokes the specified event type for all its listeners with a default constructed data.
        /// </summary>
        /// <param name="eventSender">The object invoking the event.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type to be constructed and sent.</typeparam>
        /// <returns>True if the event was actually invoked.</returns>
        bool Invoke<T>(object eventSender, EventKey eventKey = null)
            where T : IEvent, new();

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="eventSender">The object invoking the event.</param>
        /// <param name="eventValue">The event data to be sent.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if the event was actually invoked.</returns>
        bool Invoke<T>(object eventSender, T eventValue, EventKey eventKey = null)
            where T : IEvent;

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="eventSender">The object invoking the event.</param>
        /// <param name="eventValue">The event data to be sent.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if the event was actually invoked.</returns>
        bool Invoke<T>(object eventSender, ref T eventValue, EventKey eventKey = null)
            where T : IEvent;

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="eventData">The event reference being invoked.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if the event was actually invoked.</returns>
        bool Invoke<T>(EventData<T> eventData, EventKey eventKey = null)
            where T : IEvent;

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="eventData">The event reference being invoked.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if the event was actually invoked.</returns>
        bool Invoke<T>(ref EventData<T> eventData, EventKey eventKey = null)
            where T : IEvent;

        /// <summary>
        /// Removes all listeners from an event type.
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if removed any listener for the specified event type.</returns>
        bool RemoveAllListeners<T>(EventKey eventKey = null)
            where T : IEvent;

        /// <summary>
        /// Removes all listeners from an event type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <returns>True if removed any listener for the specified event type.</returns>
        bool RemoveAllListeners(Type eventType, EventKey eventKey = null);

        /// <summary>
        /// Removes all listeners from all event types that don't have the <see cref="EventKey.RestrictionOptions.DisallowRemoveAll"/> restriction.
        /// </summary>
        /// <returns>True if removed any listener.</returns>
        bool RemoveAllListenersAllowed();

        /// <summary>
        /// Removes all listeners from all event types with the matching event key.
        /// </summary>
        /// <param name="eventKey">The encapsulation key to compare to.</param>
        /// <returns>True if removed any listener.</returns>
        bool RemoveAllListenersWithKey(EventKey eventKey);

        /// <summary>
        /// Removes a listener from an event with its handle.
        /// </summary>
        /// <param name="eventHandle">The event handle.</param>
        /// <returns>True if the event handle was a valid listener for this service.</returns>
        bool RemoveListener(in EventHandle eventHandle);

        /// <summary>
        /// Resets the encapsulation key for all event types with the matching key.
        /// </summary>
        /// <param name="eventKey">The encapsulation key to compare to.</param>
        bool ResetAllEventKeys(EventKey eventKey);

        /// <summary>
        /// Resets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        bool ResetEventKey<T>(EventKey eventKey)
            where T : IEvent;

        /// <summary>
        /// Resets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        bool ResetEventKey(Type eventType, EventKey eventKey);

        /// <summary>
        /// Sets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        bool SetEventKey<T>(EventKey eventKey)
            where T : IEvent;
    }
}
