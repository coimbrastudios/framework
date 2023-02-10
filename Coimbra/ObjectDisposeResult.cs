namespace Coimbra
{
    /// <summary>
    /// Defines the possible results when calling <see cref="ObjectUtility.Dispose"/>, <see cref="GameObjectUtility.Dispose"/> or <see cref="Actor.Dispose"/>.
    /// </summary>
    public enum ObjectDisposeResult
    {
        None = 0,
        Pooled = 1,
        Destroyed = 2,
    }
}
