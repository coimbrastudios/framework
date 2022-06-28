using JetBrains.Annotations;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace Coimbra.Jobs
{
    /// <summary>
    /// Utility methods for <see cref="IManagedJob"/>.
    /// </summary>
    [PublicAPI]
    public static class ManagedJobExtensions
    {
        /// <inheritdoc cref="IJobExtensions.Run{T}"/>
        public static void Run<T>(this T jobData)
            where T : class, IManagedJob
        {
            GCHandle gcHandle = GCHandle.Alloc(jobData);

            new ManagedJob<T>()
            {
                Handle = gcHandle,
            }.Run();

            gcHandle.Free();
        }

        /// <inheritdoc cref="IJobExtensions.Schedule{T}"/>
        public static ManagedJobHandle Schedule<T>(this T jobData, JobHandle dependsOn = default)
            where T : class, IManagedJob
        {
            GCHandle gcHandle = GCHandle.Alloc(jobData);
            JobHandle jobHandle = new ManagedJob<T>()
            {
                Handle = gcHandle,
            }.Schedule(dependsOn);

            return new ManagedJobHandle()
            {
                GCHandle = gcHandle,
                JobHandle = jobHandle,
            };
        }
    }
}
