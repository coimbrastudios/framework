namespace Coimbra
{
    /// <summary>
    /// Allows to register to the <see cref="IUpdateService"/>.
    /// </summary>
    public interface IUpdateListener
    {
        /// <summary>
        /// Called during Update.
        /// </summary>
        /// <param name="deltaTime">The cached delta time.</param>
        void OnUpdate(float deltaTime);
    }
}
