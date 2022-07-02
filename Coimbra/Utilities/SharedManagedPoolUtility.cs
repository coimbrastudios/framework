using System;
using System.Collections.Generic;

namespace Coimbra
{
    internal static class SharedManagedPoolUtility
    {
        internal static readonly List<WeakReference<IManagedPool>> All = new();

        static SharedManagedPoolUtility()
        {
            All.Clear();
        }
    }
}
