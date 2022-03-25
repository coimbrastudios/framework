using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Event service that also has encapsulation for some of its methods.
    /// </summary>
    [RequireImplementors]
    public interface IEventService : IService
    {
        /// <summary>
        /// Adds a listener to an event type.
        /// </summary>
        /// <param name="eventCallback">The callback to be added.</param>
        /// <typeparam name="T">The event type.</typeparam>
        EventHandle AddListener<T>(EventListenerHandler<T> eventCallback)
            where T : IEvent;

        /// <summary>
        /// Adds a listener to an event type.
        /// </summary>
        /// <param name="eventCallback">The callback to be added.</param>
        /// <param name="appendList">List to add the event handle if generated a valid one.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if a valid <see cref="EventHandle"/> was generated.</returns>
        bool AddListener<T>(EventListenerHandler<T> eventCallback, List<EventHandle> appendList)
            where T : IEvent;

        /// <summary>
        /// Compares the event key for specified type.
        /// </summary>
        /// <param name="eventKey">The event key to compare to.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if the current event key matches the <see cref="eventKey"/> parameter.</returns>
        bool CompareEventKey<T>(object eventKey)
            where T : IEvent;

        /// <summary>
        /// Compares the event key for specified type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventKey">The event key to compare to.</param>
        /// <returns>True if the current event key matches the <see cref="eventKey"/> parameter.</returns>
        bool CompareEventKey(Type eventType, object eventKey);

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
        /// Invokes the specified event type for all its listeners with a default constructed data.
        /// </summary>
        /// <param name="eventSender">The object invoking the event.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type to be constructed and sent.</typeparam>
        /// <returns>True if the event was actually invoked.</returns>
        bool Invoke<T>(object eventSender, object eventKey = null)
            where T : IEvent, new();

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="eventSender">The object invoking the event.</param>
        /// <param name="eventData">The event data to be sent.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if the event was actually invoked.</returns>
        bool Invoke<T>(object eventSender, T eventData, object eventKey = null)
            where T : IEvent;

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="eventSender">The object invoking the event.</param>
        /// <param name="eventData">The event data to be sent.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if the event was actually invoked.</returns>
        bool Invoke<T>(object eventSender, ref T eventData, object eventKey = null)
            where T : IEvent;

        /// <summary>
        /// Removes all listeners from all event types.
        /// </summary>
        /// <param name="serviceKey">The encapsulation key for the service.</param>
        /// <returns>True if removed any listener.</returns>
        bool RemoveAllListeners(object serviceKey = null);

        /// <summary>
        /// Removes all listeners from an event type.
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if removed any listener for the specified event type.</returns>
        bool RemoveAllListeners<T>(object eventKey = null)
            where T : IEvent;

        /// <summary>
        /// Removes all listeners from an event type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <returns>True if removed any listener for the specified event type.</returns>
        bool RemoveAllListeners(Type eventType, object eventKey = null);

        /// <summary>
        /// Removes a listener from an event with its handle.
        /// </summary>
        /// <param name="eventHandle">The event handle.</param>
        /// <returns>True if the event handle was a valid listener for this service.</returns>
        bool RemoveListener(in EventHandle eventHandle);

        /// <summary>
        /// Resets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        bool ResetEventKey<T>(object eventKey)
            where T : IEvent;

        /// <summary>
        /// Resets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        bool ResetEventKey(Type eventType, object eventKey);

        /// <summary>
        /// Sets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        bool SetEventKey<T>(object eventKey)
            where T : IEvent;
    }
}
