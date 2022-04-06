using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Responsible for the application lifetime cycle, meant to be used to fire Unity's callbacks events.
    /// </summary>
    [RequireImplementors]
    public interface IApplicationService : IService
    {
        /// <summary>
        /// True when application is quitting.
        /// </summary>
        bool IsQuitting { get; }
    }
}
