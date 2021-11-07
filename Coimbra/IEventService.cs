using System;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Event service that also has encapsulation for some of its methods.
    /// </summary>
    [RequireImplementors]
    public interface IEventService : IDisposable
    {
        /// <summary>
        /// Adds a listener to an event type.
        /// </summary>
        /// <param name="callback">The callback to be added.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void AddListener<T>(EventHandler<T> callback);

        /// <summary>
        /// Invokes the specified event type for all its listeners.
        /// </summary>
        /// <param name="sender">The object invoking the event.</param>
        /// <param name="eventData">The event data to be sent.</param>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void Invoke<T>(object sender, T eventData, object eventKey = null);

        /// <summary>
        /// Removes a listener from an event type.
        /// </summary>
        /// <param name="callback">The callback to be removed.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void RemoveListener<T>(EventHandler<T> callback);

        /// <summary>
        /// Removes all listeners from all event types.
        /// </summary>
        /// <param name="serviceKey">The encapsulation key for the service.</param>
        void RemoveAllListeners(object serviceKey = null);

        /// <summary>
        /// Removes all listeners from an event type.
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void RemoveAllListeners<T>(object eventKey = null);

        /// <summary>
        /// Resets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void ResetEventKey<T>(object eventKey);

        /// <summary>
        /// Sets the encapsulation key for an event type;
        /// </summary>
        /// <param name="eventKey">The encapsulation key for the event.</param>
        /// <typeparam name="T">The event type.</typeparam>
        void SetEventKey<T>(object eventKey);
    }
}
