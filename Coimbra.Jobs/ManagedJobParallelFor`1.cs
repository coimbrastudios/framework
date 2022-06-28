using JetBrains.Annotations;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace Coimbra.Jobs
{
    /// <summary>
    /// Job type that supports managed types.
    /// </summary>
    [PublicAPI]
    public struct ManagedJobParallelFor<T> : IJobParallelFor
        where T : IManagedJobParallelFor
    {
        public GCHandle Handle;

        public void Execute(int index)
        {
            T job = (T)Handle.Target;
            job.Execute(index);
        }
    }
}
