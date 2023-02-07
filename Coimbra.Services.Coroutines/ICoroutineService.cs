#nullable enable

using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra.Services.Coroutines
{
    /// <summary>
    /// Provides easy access to Unity <see cref="Coroutine"/> system without requiring the object to be an <see cref="MonoBehaviour"/>.
    /// </summary>
    /// <remarks>
    /// Meant to replace usage of <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>, <see cref="MonoBehaviour.StopCoroutine(Coroutine)"/>, and <see cref="MonoBehaviour.StopAllCoroutines"/>.
    /// </remarks>
    [RequiredService]
    [RequireImplementors]
    public interface ICoroutineService : IService
    {
        /// <inheritdoc cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>
        Coroutine StartCoroutine(IEnumerator routine);

        /// <inheritdoc cref="MonoBehaviour.StopCoroutine(Coroutine)"/>
        void StopCoroutine(Coroutine routine);

        /// <inheritdoc cref="MonoBehaviour.StopAllCoroutines()"/>
        void StopAllCoroutines();
    }
}
