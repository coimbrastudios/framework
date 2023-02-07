namespace Coimbra.Services.ApplicationStateEvents
{
    /// <summary>
    /// Invoked during <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnApplicationQuit.html">OnApplicationQuit</a>.
    /// </summary>
    /// <seealso cref="IApplicationStateEvent"/>
    public readonly partial struct ApplicationQuitEvent : IApplicationStateEvent { }
}
