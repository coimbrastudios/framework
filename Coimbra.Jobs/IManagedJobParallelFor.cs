using JetBrains.Annotations;

namespace Coimbra.Jobs
{
    /// <summary>
    /// Implement this interface to create your own <see cref="ManagedJobParallelFor{T}"/>.
    /// </summary>
    [PublicAPI]
    public interface IManagedJobParallelFor
    {
        void Execute(int index);
    }
}
