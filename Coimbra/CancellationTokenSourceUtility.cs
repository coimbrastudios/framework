#nullable enable

using System.Runtime.CompilerServices;
using System.Threading;

namespace Coimbra
{
    /// <summary>
    /// Utility methods for <see cref="CancellationTokenSource"/> type.
    /// </summary>
    public static class CancellationTokenSourceUtility
    {
        /// <summary>
        /// Cancel, dispose, and set to null the specified <see cref="CancellationTokenSource"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Collect(ref CancellationTokenSource? cancellationTokenSource)
        {
            if (cancellationTokenSource is null)
            {
                return;
            }

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }
}
