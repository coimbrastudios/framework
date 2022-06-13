using Coimbra.Editor;
using System;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    [Serializable]
    internal struct ServiceWrapper
    {
        [SerializeField]
        internal string Type;

        [SerializeField]
        internal string Factory;

        [SerializeField]
        internal ManagedField<IService> Value;

        internal ServiceWrapper(Type type, ServiceLocator.Service service)
        {
            Type = TypeString.Get(type);
            Factory = service.Factory != null ? TypeString.Get(service.Factory.GetType()) : "<null>";
            Value = new ManagedField<IService>(service.Value);
        }
    }
}
