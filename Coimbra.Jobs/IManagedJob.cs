using JetBrains.Annotations;

namespace Coimbra.Jobs
{
    /// <summary>
    /// Implement this interface to create your own <see cref="ManagedJob{T}"/>.
    /// </summary>
    [PublicAPI]
    public interface IManagedJob
    {
        void Execute();
    }
}
