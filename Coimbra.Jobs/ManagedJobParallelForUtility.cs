using JetBrains.Annotations;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace Coimbra.Jobs
{
    /// <summary>
    /// Utility methods for <see cref="IManagedJobParallelFor"/>.
    /// </summary>
    [PublicAPI]
    public static class ManagedJobParallelForUtility
    {
        /// <inheritdoc cref="IJobParallelForExtensions.Run{T}"/>
        public static void Run<T>(this T jobData, int arrayLength)
            where T : class, IManagedJobParallelFor
        {
            GCHandle gcHandle = GCHandle.Alloc(jobData);

            new ManagedJobParallelFor<T>()
            {
                Handle = gcHandle,
            }.Run(arrayLength);

            gcHandle.Free();
        }

        /// <inheritdoc cref="IJobParallelForExtensions.Schedule{T}"/>
        public static ManagedJobHandle Schedule<T>(this T jobData, int arrayLength, int innerLoopBatchCount, JobHandle dependsOn = default)
            where T : class, IManagedJobParallelFor
        {
            GCHandle gcHandle = GCHandle.Alloc(jobData);
            JobHandle jobHandle = new ManagedJobParallelFor<T>()
            {
                Handle = gcHandle,
            }.Schedule(arrayLength, innerLoopBatchCount, dependsOn);

            return new ManagedJobHandle()
            {
                GCHandle = gcHandle,
                JobHandle = jobHandle,
            };
        }
    }
}
