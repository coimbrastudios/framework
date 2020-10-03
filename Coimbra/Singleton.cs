using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    ///     Create a singleton from any managed type.
    /// </summary>
    public static class Singleton<T>
        where T : class
    {
        [PublicAPI]
        public delegate void ValueChangeEventHandler([CanBeNull] T oldValue, [CanBeNull] T newValue);

        /// <summary>
        ///     Invoked when the singleton value changes.
        /// </summary>
        [PublicAPI]
        public static event ValueChangeEventHandler OnValueChanged;

        private static T _value;
        private static Func<T> _createCallback;

        /// <summary>
        ///     Gets the existing singleton value.
        /// </summary>
        /// <returns>The singleton value.</returns>
        [CanBeNull] [PublicAPI] [Pure]
        public static T Get()
        {
            return _value;
        }

        /// <summary>
        ///     Gets the existing singleton value or creates a new one if none.
        /// </summary>
        /// <returns>The singleton value.</returns>
        [CanBeNull] [PublicAPI]
        public static T GetOrCreate()
        {
            if (_value == null && _createCallback != null)
            {
                _value = _createCallback.Invoke();
            }

            return _value;
        }

        /// <summary>
        ///     Gets if the create callback is currently set.
        /// </summary>
        /// <returns>True if the create callback is not null.</returns>
        [PublicAPI] [Pure]
        public static bool HasCreateCallback()
        {
            return _createCallback != null;
        }

        /// <summary>
        ///     Sets the current singleton value.
        /// </summary>
        /// <param name="singleton">The new singleton value.</param>
        [PublicAPI]
        public static void Set([CanBeNull] T singleton)
        {
            T oldValue = _value;
            _value = singleton;
            OnValueChanged?.Invoke(oldValue, _value);
        }

        /// <summary>
        ///     Sets the callback for when the singleton value needs to be created.
        /// </summary>
        [PublicAPI]
        public static void SetCreateCallback([CanBeNull] Func<T> createCallback)
        {
            _createCallback = createCallback;
        }

        /// <summary>
        ///     Tries to get the current singleton value.
        /// </summary>
        /// <param name="value">The current singleton value.</param>
        /// <returns>False if the value is null.</returns>
        [PublicAPI] [Pure]
        public static bool TryGet([CanBeNull] out T value)
        {
            value = _value;

            return value != null;
        }

        /// <summary>
        ///     Tries to get the existing singleton value or create a new one if none.
        /// </summary>
        /// <returns>False if the value is null.</returns>
        [PublicAPI]
        public static bool TryGetOrCreate([CanBeNull] out T value)
        {
            value = GetOrCreate();

            return value != null;
        }
    }
}
