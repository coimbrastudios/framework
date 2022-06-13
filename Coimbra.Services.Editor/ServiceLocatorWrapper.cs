using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    [Serializable]
    internal struct ServiceLocatorWrapper
    {
        internal readonly WeakReference<ServiceLocator> ServiceLocatorReference;

        [SerializeField]
        internal string Id;

        [SerializeField]
        internal bool AllowFallbackToShared;

        [SerializeField]
        internal List<ServiceWrapper> Services;

        internal ServiceLocatorWrapper(ServiceLocator serviceLocator, List<ServiceWrapper> services, WeakReference<ServiceLocator> serviceLocatorReference)
        {
            Id = serviceLocator.Id;
            AllowFallbackToShared = serviceLocator.AllowFallbackToShared;
            Services = services;
            ServiceLocatorReference = serviceLocatorReference;
        }
    }
}
