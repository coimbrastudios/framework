using JetBrains.Annotations;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace Coimbra.Jobs
{
    /// <summary>
    /// The handle for any job containing a managed type.
    /// </summary>
    [PublicAPI]
    public struct ManagedJobHandle
    {
        public GCHandle GCHandle;
        public JobHandle JobHandle;

        public void Complete()
        {
            JobHandle.Complete();
            GCHandle.Free();
        }
    }
}
