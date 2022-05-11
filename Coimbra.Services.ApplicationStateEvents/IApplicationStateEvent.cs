using Coimbra.Services.Events;
using UnityEngine.Scripting;

namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Base event interface for the Unity's application callbacks events.
    /// </summary>
    [RequireImplementors]
    public interface IApplicationStateEvent : IEvent { }
}
