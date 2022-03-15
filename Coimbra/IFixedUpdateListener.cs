namespace Coimbra
{
    /// <summary>
    /// Allows to register to the <see cref="IFixedUpdateService"/>.
    /// </summary>
    public interface IFixedUpdateListener
    {
        /// <summary>
        /// Called during FixedUpdate.
        /// </summary>
        /// <param name="deltaTime">The cached delta time.</param>
        void OnFixedUpdate(float deltaTime);
    }
}
