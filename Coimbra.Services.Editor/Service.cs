using System;
using System.Reflection;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    [Serializable]
    internal struct Service
    {
        [SerializeField]
        [HideInInspector]
        internal string Type;

        [SerializeField]
        [Tooltip("Has DynamicServiceAttribute?")]
        internal bool IsDynamic;

        [SerializeField]
        [Disable]
        [Tooltip("The Factory set for this type.")]
        internal string Factory;

        [SerializeField]
        [Disable]
        [Tooltip("The current value set.")]
        internal ManagedField<IService> Value;

        internal Service(Type type, ServiceLocator.Service service)
        {
            IsDynamic = type.GetCustomAttribute<DynamicServiceAttribute>() != null;
            Type = TypeString.Get(type);
            Factory = service.Factory != null ? TypeString.Get(service.Factory.GetType()) : "<null>";
            Value = new ManagedField<IService>(service.Value);
        }
    }
}
