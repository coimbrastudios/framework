using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Provides easy access to Unity's <see cref="Coroutine"/> system without requiring the object to be an <see cref="MonoBehaviour"/>.
    /// </summary>
    [RequireImplementors]
    public interface ICoroutineService
    {
        /// <inheritdoc cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>
        Coroutine StartCoroutine(IEnumerator routine);

        /// <inheritdoc cref="MonoBehaviour.StopCoroutine(Coroutine)"/>
        void StopCoroutine(Coroutine routine);

        /// <inheritdoc cref="MonoBehaviour.StopAllCoroutines()"/>
        void StopAllCoroutines();
    }
}
