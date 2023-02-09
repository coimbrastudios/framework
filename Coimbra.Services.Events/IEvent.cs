using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Base interface for any event to be used with <see cref="IEventService"/>.
    /// </summary>
    /// <remarks>
    /// Any implementor requires to be partial as this framework makes use of Source Generators to enforce strongly-typed events.
    /// <para></para>
    /// The methods generated are equivalent to the <see cref="IEventService"/> generic APIs and have analyzers in place to ensure that those generated methods are used instead.
    /// You can allow specific types to still have access to the <see cref="IEventService"/> generic APIs by using adding the <see cref="AllowEventServiceUsageForAttribute"/> in specific <see cref="IEvent"/> definitions (interface) or implementations (struct or class).
    /// <para></para>
    /// Most of the generated API are public, however the generated <see cref="IEventService.Invoke{TEvent}"/> and <see cref="IEventService.RemoveAllListeners{TEvent}"/> equivalent ones are left as internal so that you can encapsulate those API by using different assembly definition files for your <see cref="IEvent"/> implementations.
    /// <para></para>
    /// As each <see cref="IEvent"/> implementation has its API generated statically, it also means that you can easily find usages of a given API call for a specific event type, making easier to execute refactors and debugging issues with your logic.
    /// </remarks>
    /// <seealso cref="AllowEventServiceUsageForAttribute"/>
    /// <seealso cref="IEventService"/>
    /// <seealso cref="EventHandle"/>
    /// <seealso cref="EventHandleTrackerComponent"/>
    /// <seealso cref="EventContext"/>
    /// <seealso cref="EventContextHandler{T}"/>
    /// <seealso cref="EventRelevancyChangedHandler"/>
    /// <seealso cref="EventSettings"/>
    /// <seealso cref="EventSystem"/>
    [RequireImplementors]
    public interface IEvent { }
}
