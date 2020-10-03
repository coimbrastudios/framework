using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    ///     Create a disposable from any type.
    /// </summary>
    public readonly struct Disposable<T> : IDisposable
    {
        [PublicAPI]
        public delegate void DisposeHandler([NotNull] T value);

        private readonly DisposeHandler _onDispose;

        [PublicAPI]
        public Disposable([NotNull] T value, DisposeHandler onDispose)
        {
            Value = value;
            _onDispose = onDispose;
        }

        [PublicAPI]
        public T Value { get; }

        [PublicAPI]
        public void Dispose()
        {
            _onDispose?.Invoke(Value);
        }
    }
}
