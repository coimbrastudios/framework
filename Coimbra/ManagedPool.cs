using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Generic way to quickly start pooling your managed objects. For pooling <see cref="UnityEngine.GameObject"/> see <see cref="IPoolingService"/>.
    /// </summary>
    [Preserve]
    public sealed class ManagedPool<T> : ManagedPoolBase<T>
        where T : class, new()
    {
        /// <summary>
        /// Default shared pool instance.
        /// </summary>
        [NotNull]
        public static readonly ManagedPool<T> Shared = new ManagedPool<T>();

        protected override T OnCreate()
        {
            return new T();
        }

        protected override void OnDelete(T item) { }

        protected override void OnGet(T item) { }

        protected override void OnRelease(T item) { }
    }
}
