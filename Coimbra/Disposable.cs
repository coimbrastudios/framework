using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    ///     Create a disposable from any type.
    /// </summary>
    public readonly struct Disposable<T> : IDisposable
    {
        public delegate void DisposeHandler([CanBeNull] in T value);

        [CanBeNull]
        public readonly T Value;

        private readonly DisposeHandler _onDispose;

        public Disposable([CanBeNull] in T value, [CanBeNull] DisposeHandler onDispose)
        {
            Value = value;
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose?.Invoke(in Value);
        }
    }
}
