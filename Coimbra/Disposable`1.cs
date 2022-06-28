using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Create a disposable from any type.
    /// </summary>
    [Preserve]
    public ref struct Disposable<T>
    {
        public delegate void DisposeHandler([CanBeNull] in T value);

        [CanBeNull]
        public readonly T Value;

        private DisposeHandler _onDispose;

        public Disposable([CanBeNull] in T value, [CanBeNull] DisposeHandler onDispose)
        {
            Value = value;
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose?.Invoke(in Value);
            _onDispose = null;
        }
    }
}
