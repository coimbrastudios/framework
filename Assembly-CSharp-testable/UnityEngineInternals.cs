using System.Runtime.CompilerServices;
using UnityEngine;

namespace CoimbraInternal
{
    internal static class UnityEngineInternals
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool DoesObjectWithInstanceIDExist(int instanceID)
        {
            return Object.DoesObjectWithInstanceIDExist(instanceID);
        }
    }
}
