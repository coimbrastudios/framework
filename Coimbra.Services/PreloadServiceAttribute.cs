using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Coimbra.Services
{
    /// <summary>
    /// Apply this on a concrete <see cref="IService"/> implementation to preload the service during <see cref="RuntimeInitializeLoadType.BeforeSceneLoad"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(IService))]
    public sealed class PreloadServiceAttribute : Attribute { }
}
