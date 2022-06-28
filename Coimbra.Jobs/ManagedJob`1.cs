using JetBrains.Annotations;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace Coimbra.Jobs
{
    /// <summary>
    /// Job type that supports managed types.
    /// </summary>
    [PublicAPI]
    public struct ManagedJob<T> : IJob
        where T : IManagedJob
    {
        public GCHandle Handle;

        public void Execute()
        {
            T job = (T)Handle.Target;
            job.Execute();
        }
    }
}
