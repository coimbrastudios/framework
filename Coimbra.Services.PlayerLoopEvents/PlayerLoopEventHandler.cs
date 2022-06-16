using Coimbra.Services.Events;

namespace Coimbra.Services.PlayerLoopEvents
{
    /// <summary>
    /// Delegate for listening any <see cref="IPlayerLoopEvent"/>.
    /// </summary>
    public delegate void PlayerLoopEventHandler(ref EventContext context, float deltaTime);
}
