namespace Coimbra
{
    /// <summary>
    /// Allows to register to the <see cref="ILateUpdateService"/>.
    /// </summary>
    public interface ILateUpdateListener
    {
        /// <summary>
        /// Called during LateUpdate.
        /// </summary>
        /// <param name="deltaTime">The cached delta time.</param>
        void OnLateUpdate(float deltaTime);
    }
}
