using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    ///     Create a disposable from any type.
    /// </summary>
    public readonly struct Disposable<T> : IDisposable
    {
        public delegate void DisposeHandler([NotNull] T value);

        private readonly DisposeHandler _onDispose;

        public Disposable([NotNull] T value, DisposeHandler onDispose)
        {
            Value = value;
            _onDispose = onDispose;
        }

        public T Value { get; }

        public void Dispose()
        {
            _onDispose?.Invoke(Value);
        }
    }
}
