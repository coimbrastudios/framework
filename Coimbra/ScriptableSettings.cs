using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Class that allows easy access to a <see cref="ScriptableObject"/> and also enables automatic preloading for it.
    /// </summary>
    [RequireDerived]
    public abstract class ScriptableSettings : ScriptableObject
    {
        /// <summary>
        /// Delegate for using with the find methods.
        /// </summary>
        public delegate ScriptableSettings FindHandler(Type type);

        /// <summary>
        /// Finds with <see cref="Resources.FindObjectsOfTypeAll"/>, returning null if none.
        /// </summary>
        public static readonly FindHandler FindFirst = delegate(Type type)
        {
            Object[] rawValues = Resources.FindObjectsOfTypeAll(type);

            if (rawValues.Length == 0)
            {
                return null;
            }

            ScriptableSettings result = (ScriptableSettings)rawValues[0];
            Values[type] = result;

            return result;
        };

        /// <summary>
        /// Finds with <see cref="Resources.FindObjectsOfTypeAll"/>, returning null if none. Also logs a warning if more than 1 is found.
        /// </summary>
        public static readonly FindHandler FindSingle = delegate(Type type)
        {
            Object[] rawValues = Resources.FindObjectsOfTypeAll(type);

            if (rawValues.Length == 0)
            {
                return null;
            }

            if (rawValues.Length > 1)
            {
                Debug.LogWarning($"It was expected a single loaded object of type {type}, but it was found {rawValues.Length}!");
            }

            ScriptableSettings result = (ScriptableSettings)rawValues[0];
            Values[type] = result;

            return result;
        };

        private static readonly Dictionary<Type, ScriptableSettings> Values = new();

        static ScriptableSettings()
        {
            Application.quitting -= HandleApplicationQuitting;
            Application.quitting += HandleApplicationQuitting;
        }

        /// <summary>
        /// Should this setting be included in the preloaded assets?
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Should this setting be included in the preloaded assets?")]
        [PublicAPI]
        public bool Preload { get; protected set; } = true;

        /// <summary>
        /// True when application is quitting.
        /// </summary>
        protected static bool IsQuitting { get; private set; }

        /// <summary>
        /// Gets the last set value for the specified type.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <returns>The settings if set and still valid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptableSettings Get(Type type)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));

            return Values.TryGetValue(type, out ScriptableSettings value) && value.IsValid() ? value : null;
        }

        /// <inheritdoc cref="Get"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>()
            where T : ScriptableSettings
        {
            return Get(typeof(T)) as T;
        }

        /// <summary>
        /// Gets the last set value for the specified type, but also tries to find one if not set.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <param name="findCallback">How to find a new instance. Defaults to use <see cref="FindSingle"/></param>.
        /// <returns>The settings if set and still valid or if a new one could be found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptableSettings GetOrFind(Type type, FindHandler findCallback = null)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));

            if (Values.TryGetValue(type, out ScriptableSettings value) && value.IsValid())
            {
                return value;
            }

            return findCallback?.Invoke(type) ?? FindSingle.Invoke(type);
        }

        /// <inheritdoc cref="GetOrFind"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrFind<T>(FindHandler findCallback = null)
            where T : ScriptableSettings
        {
            return GetOrFind(typeof(T), findCallback) as T;
        }

        /// <summary>
        /// Checks if the value for the specified type has been set and is still valid.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <returns>True if the settings is set and still valid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has(Type type)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));

            return Values.TryGetValue(type, out ScriptableSettings value) && value.IsValid();
        }

        /// <inheritdoc cref="Has"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has<T>()
            where T : ScriptableSettings
        {
            return Has(typeof(T));
        }

        /// <summary>
        /// Sets the value for the specified type, if not set yet.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <param name="value">The new value for the specified type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(Type type, ScriptableSettings value)
        {
            Set(false, type, value);
        }

        /// <inheritdoc cref="Set"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(T value)
            where T : ScriptableSettings
        {
            Set(false, typeof(T), value);
        }

        /// <summary>
        /// Sets the value for the specified type, even if it was already set.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <param name="value">The new value for the specified type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetOrOverwrite(Type type, ScriptableSettings value)
        {
            Set(true, type, value);
        }

        /// <inheritdoc cref="SetOrOverwrite"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetOrOverwrite<T>(T value)
            where T : ScriptableSettings
        {
            Set(true, typeof(T), value);
        }

        /// <summary>
        /// Tries to get the last set value for the specified type.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <param name="result">The settings if set and still valid.</param>
        /// <returns>True if the settings is set and still valid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet(Type type, out ScriptableSettings result)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));

            result = Get(type);

            return result != null;
        }

        /// <inheritdoc cref="TryGet"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>(out T result)
            where T : ScriptableSettings
        {
            result = Get(typeof(T)) as T;

            return result != null;
        }

        /// <summary>
        /// Tries to get the last set value for the specified type, but also tries to find one through <see cref="Resources.FindObjectsOfTypeAll"/> if not set.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <param name="result">The settings if set and still valid or if a new one could be found.</param>
        /// <param name="findCallback">How to find a new instance. Defaults to use <see cref="FindSingle"/></param>.
        /// <returns>The settings if set and still valid or if a new one could be found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetOrFind(Type type, out ScriptableSettings result, FindHandler findCallback = null)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));

            result = GetOrFind(type, findCallback);

            return result != null;
        }

        /// <inheritdoc cref="TryGetOrFind"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetOrFind<T>(out T result, FindHandler findCallback = null)
            where T : ScriptableSettings
        {
            result = GetOrFind(typeof(T), findCallback) as T;

            return result != null;
        }

        protected virtual void Reset()
        {
#if UNITY_EDITOR
            if (Preload)
            {
                EnsurePreload(false);
            }
#endif
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (Preload)
            {
                EnsurePreload(true);
            }
            else
            {
                using (SharedManagedPools.Pop(out List<Object> pooledList))
                {
                    pooledList.Clear();
                    pooledList.AddRange(UnityEditor.PlayerSettings.GetPreloadedAssets());

                    if (!pooledList.Remove(this))
                    {
                        pooledList.Clear();

                        return;
                    }

                    while (pooledList.Remove(this)) { }

                    UnityEditor.PlayerSettings.SetPreloadedAssets(pooledList.ToArray());
                    pooledList.Clear();
                }
            }
#endif
        }

        protected virtual void OnEnable()
        {
            Type type = GetType();
            Set(type, this);
        }

        protected virtual void OnDisable()
        {
            Type type = GetType();

            if (TryGet(type, out ScriptableSettings current) && current == this)
            {
                SetOrOverwrite(type, null);
            }
        }

        private static void HandleApplicationQuitting()
        {
            IsQuitting = true;
        }

        private static void Set(bool forceSet, Type type, ScriptableSettings value)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));

            value = value.GetValid();

            if (TryGet(type, out ScriptableSettings currentValue) && value != currentValue)
            {
                if (forceSet)
                {
                    if (!CoimbraUtility.IsReloadingScripts && !IsQuitting)
                    {
                        Debug.LogWarning($"Overriding {type} in {nameof(ScriptableSettings)} from \"{currentValue}\"!", currentValue);
                        Debug.LogWarning($"Overriding {type} in {nameof(ScriptableSettings)} to \"{value}\"!", value);
                    }
                }
                else
                {
                    Debug.LogWarning($"{type} in {nameof(ScriptableSettings)} is already set to \"{currentValue}\"!", currentValue);
                    Debug.LogWarning($"{type} in {nameof(ScriptableSettings)} can't be overriden to \"{value}\".", value);

                    return;
                }
            }

            if (value != null)
            {
                Debug.Assert(type.IsInstanceOfType(value));
                Values[type] = value;
            }
            else
            {
                Values.Remove(type);
            }
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void Initialize()
        {
            IsQuitting = false;
            Values.Clear();
            UnityEditor.PlayerSettings.GetPreloadedAssets();
        }

        private void EnsurePreload(bool withWarning)
        {
            using (SharedManagedPools.Pop(out List<Object> pooledList))
            {
                pooledList.Clear();
                pooledList.AddRange(UnityEditor.PlayerSettings.GetPreloadedAssets());

                if (pooledList.Contains(this))
                {
                    pooledList.Clear();

                    return;
                }

                if (withWarning)
                {
                    Debug.LogWarning($"Fixing \"{this}\" not being added to the preloaded assets.", this);
                }

                pooledList.Add(this);
                UnityEditor.PlayerSettings.SetPreloadedAssets(pooledList.ToArray());
                pooledList.Clear();
            }
        }
#endif
    }
}
