using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    ///     Generic way to quickly start pooling your non-Unity managed objects.
    /// </summary>
    public sealed class ManagedPool<T> : ManagedPoolBase<T>
        where T : class, new()
    {
        /// <summary>
        ///     Default shared pool instance.
        /// </summary>
        [NotNull]
        public static readonly ManagedPool<T> Global = new ManagedPool<T>();

        protected override T OnCreate()
        {
            return new T();
        }

        protected override void OnDelete(T item) { }

        protected override void OnGet(T item) { }

        protected override void OnRelease(T item) { }
    }
}
