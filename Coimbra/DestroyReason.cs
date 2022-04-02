namespace Coimbra
{
    /// <summary>
    /// Defines the reason why the OnDestroy callback is being called.
    /// </summary>
    public enum DestroyReason
    {
        /// <summary>
        /// Instigated by an user code explicitly.
        /// </summary>
        ExplicitCall,
        /// <summary>
        /// Instigated by a scene change when the object was not flagged to don't destroy on load.
        /// </summary>
        SceneChange,
        /// <summary>
        /// Being called due the application being shutdown.
        /// </summary>
        ApplicationQuit,
    }
}
