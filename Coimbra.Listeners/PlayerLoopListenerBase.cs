using System.Runtime.CompilerServices;

namespace Coimbra.Listeners
{
    /// <summary>
    /// Base class to listen to player loop callbacks.
    /// </summary>
    /// <remarks>
    /// You can inherit from this class to create listeners for <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">FixedUpdate</a>, <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">Update</a> and <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html">LateUpdate</a> callbacks.
    /// <para></para>
    /// No default concrete implementation is offered in this module because the framework already offers alternative to those callbacks in another module.
    /// </remarks>
    public abstract class PlayerLoopListenerBase : ActorComponentBase
    {
        public delegate void EventHandler(PlayerLoopListenerBase sender, float deltaTime);

        /// <summary>
        /// Invoked inside <see cref="Trigger"/>.
        /// </summary>
        public virtual event EventHandler OnTrigger
        {
            add => _eventHandler += value;
            remove => _eventHandler -= value;
        }

        private EventHandler _eventHandler;

        /// <summary>
        /// Gets a value indicating whether <see cref="OnTrigger"/> has any listener.
        /// </summary>
        protected bool HasListener => _eventHandler != null;

        /// <summary>
        /// Invokes the <see cref="OnTrigger"/> event.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Trigger(float deltaTime)
        {
            _eventHandler?.Invoke(this, deltaTime);
        }
    }
}
