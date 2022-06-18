using System;
using System.Reflection;
using UnityEngine;

namespace Coimbra.Services.Editor
{
    [Serializable]
    internal struct ServiceClass
    {
        [SerializeField]
        [HideInInspector]
        internal string Type;

        [SerializeField]
        [Tooltip("Is an abstract class?")]
        internal bool IsAbstract;

        [SerializeField]
        [Tooltip("Has DynamicServiceAttribute?")]
        internal bool IsDynamic;

        [SerializeField]
        [Tooltip("Has DisableDefaultFactoryAttribute?")]
        internal bool DisableDefaultFactory;

        [SerializeField]
        [Tooltip("Has PreloadServiceAttribute?")]
        internal bool Preload;

        internal ServiceClass(Type type)
        {
            Type = TypeString.Get(type);
            IsAbstract = type.IsAbstract;
            IsDynamic = type.GetCustomAttribute<DynamicServiceAttribute>() != null;
            DisableDefaultFactory = type.GetCustomAttribute<DisableDefaultFactoryAttribute>() != null;
            Preload = type.GetCustomAttribute<PreloadServiceAttribute>() != null;
        }
    }
}
