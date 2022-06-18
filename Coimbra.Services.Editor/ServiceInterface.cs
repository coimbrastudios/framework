using System;
using System.Reflection;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    [Serializable]
    internal struct ServiceInterface
    {
        [SerializeField]
        [HideInInspector]
        internal string Type;

        [SerializeField]
        [Tooltip("Has AbstractServiceAttribute?")]
        internal bool IsAbstract;

        [SerializeField]
        [Tooltip("Has DynamicServiceAttribute?")]
        internal bool IsDynamic;

        internal ServiceInterface(Type type)
        {
            Type = TypeString.Get(type);
            IsAbstract = type.GetCustomAttribute<AbstractServiceAttribute>() != null;
            IsDynamic = type.GetCustomAttribute<DynamicServiceAttribute>() != null;
        }
    }
}
