using Coimbra.Services.Events;
using UnityEngine.Scripting;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Base event interface for the Unity's application callbacks events. They are meant to be fired by <see cref="IApplicationStateService"/>.
    /// </summary>
    /// <seealso cref="ApplicationFocusEvent"/>
    /// <seealso cref="ApplicationPauseEvent"/>
    /// <seealso cref="ApplicationQuitEvent"/>
    /// <seealso cref="IApplicationStateService"/>
    /// <seealso cref="ApplicationStateSystem"/>
    [RequireImplementors]
    [AllowEventServiceUsageFor(typeof(IApplicationStateService))]
    public interface IApplicationStateEvent : IEvent { }
}
