#nullable enable

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
