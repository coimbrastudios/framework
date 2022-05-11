using UnityEngine.Scripting;

namespace Coimbra.Services.Events
{
    /// <summary>
    /// Base interface for any event to be used with <see cref="IEventService"/>.
    /// </summary>
    [RequireImplementors]
    public interface IEvent { }
}
