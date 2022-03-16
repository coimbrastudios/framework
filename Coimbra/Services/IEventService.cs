using System;
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
        EventHandle AddListener<T>(EventHandler<T> eventCallback);

        /// <summary>
        /// Compares the event key for specified type.
        /// </summary>
        /// <param name="eventKey">The event key to compare to.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if the current event key matches the <see cref="eventKey"/> parameter.</returns>
        bool CompareEventKey<T>(object eventKey);

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
        bool HasAnyListeners<T>();

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
        /// <param name="eventType">The event type to be constructed and sent.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <param name="ignoreException">If true, the method will fail silently if <see cref="eventKey"/> doesn't match the current event key.</param>
        /// <exception cref="InvalidOperationException">If <see cref="ignoreException"/> is false and <see cref="eventKey"/> doesn't match the current event key.</exception>
        void Invoke(object eventSender, Type eventType, object eventKey = null, bool ignoreException = false);

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="eventSender">The object invoking the event.</param>
        /// <param name="eventData">The event data to be sent.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <param name="ignoreException">If true, the method will fail silently if <see cref="eventKey"/> doesn't match the current event key.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <exception cref="InvalidOperationException">If <see cref="ignoreException"/> is false and <see cref="eventKey"/> doesn't match the current event key.</exception>
        void Invoke<T>(object eventSender, T eventData, object eventKey = null, bool ignoreException = false);

        /// <summary>
        /// Removes all listeners from all event types.
        /// </summary>
        /// <param name="serviceKey">The encapsulation key for the service.</param>
        /// <param name="ignoreException">If true, the method will fail silently if <see cref="serviceKey"/> doesn't match the current service key.</param>
        /// <exception cref="InvalidOperationException">If <see cref="ignoreException"/> is false and <see cref="serviceKey"/> doesn't match the current service key.</exception>
        void RemoveAllListeners(object serviceKey = null, bool ignoreException = false);

        /// <summary>
        /// Removes all listeners from an event type.
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <param name="ignoreException">If true, the method will fail silently if <see cref="eventKey"/> doesn't match the current event key.</param>
        /// <typeparam name="T">The event type.</typeparam>
        /// <exception cref="InvalidOperationException">If <see cref="ignoreException"/> is false and <see cref="eventKey"/> doesn't match the current event key.</exception>
        void RemoveAllListeners<T>(object eventKey = null, bool ignoreException = false);

        /// <summary>
        /// Removes all listeners from an event type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <param name="ignoreException">If true, the method will fail silently if <see cref="eventKey"/> doesn't match the current event key.</param>
        /// <exception cref="InvalidOperationException">If <see cref="ignoreException"/> is false and <see cref="eventKey"/> doesn't match the current event key.</exception>
        void RemoveAllListeners(Type eventType, object eventKey = null, bool ignoreException = false);

        /// <summary>
        /// Removes a listener from an event with its handle.
        /// </summary>
        /// <param name="eventHandle">The event handle.</param>
        void RemoveListener(in EventHandle eventHandle);

        /// <summary>
        /// Resets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void ResetEventKey<T>(object eventKey);

        /// <summary>
        /// Resets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        void ResetEventKey(Type eventType, object eventKey);

        /// <summary>
        /// Sets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void SetEventKey<T>(object eventKey);
    }
}
