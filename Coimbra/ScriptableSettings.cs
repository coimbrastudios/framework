using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using Debug = UnityEngine.Debug;
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
        /// Finds with <see cref="Resources.FindObjectsOfTypeAll"/>, returning null if none. Also logs a warning if more than 1 is found.
        /// </summary>
        public static readonly FindHandler FindSingle = delegate(Type type)
        {
            Object[] rawValues = ObjectUtility.FindAll(type);

            if (rawValues.Length == 0)
            {
                return null;
            }

            if (rawValues.Length > 1)
            {
                Debug.LogWarning($"It was expected a single loaded object of type {type}, but it was found {rawValues.Length}!");
#if UNITY_EDITOR
                foreach (Object rawValue in rawValues)
                {
                    Debug.Log(UnityEditor.AssetDatabase.GetAssetPath(rawValue), rawValue);
                }
#endif
            }

            ScriptableSettings result = (ScriptableSettings)rawValues[0];
            Map[type] = result;

            return result;
        };

        internal static readonly Dictionary<Type, ScriptableSettings> Map = new();

        [SerializeField]
        [FormerlySerializedAsBackingFieldOf("Preload")]
        [Tooltip("Should this setting be included in the preloaded assets?")]
        private bool _preload = true;

        [SerializeField]
        [HideInInspector]
        [FormerlySerializedAsBackingFieldOf("Type")]
        private ScriptableSettingsType _type;

        static ScriptableSettings()
        {
            Application.quitting -= HandleApplicationQuitting;
            Application.quitting += HandleApplicationQuitting;
        }

        protected ScriptableSettings()
        {
            Type = GetType(GetType());

            if (Type.IsEditorOnly())
            {
                Preload = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this setting should be included in the preloaded assets.
        /// </summary>
        [PublicAPI]
        public bool Preload
        {
            [DebuggerStepThrough]
            get => _preload;
            [DebuggerStepThrough]
            protected set => _preload = value;
        }

        /// <summary>
        /// Gets the type of <see cref="ScriptableSettings"/> based on the presence of either <see cref="PreferencesAttribute"/> or <see cref="ProjectSettingsAttribute"/>.
        /// </summary>
        public ScriptableSettingsType Type
        {
            [DebuggerStepThrough]
            get => _type;
            [DebuggerStepThrough]
            private set => _type = value;
        }

        /// <summary>
        /// Gets a value indicating whether the application is quitting.
        /// </summary>
        protected static bool IsQuitting { get; private set; }

        /// <summary>
        /// Gets the last set value for the specified type, but also tries to find one if not set.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <param name="findCallback">How to find a new instance. Defaults to use <see cref="FindSingle"/>.</param>.
        /// <returns>The settings if set and still valid or if a new one could be found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptableSettings GetOrFind(Type type, FindHandler findCallback = null)
        {
            if (Map.TryGetValue(type, out ScriptableSettings value) && value.IsValid())
            {
                return value;
            }

            findCallback ??= FindSingle;

            return findCallback.Invoke(type);
        }

        /// <inheritdoc cref="GetOrFind"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrFind<T>(FindHandler findCallback = null)
            where T : ScriptableSettings
        {
            return GetOrFind(typeof(T), findCallback) as T;
        }

        /// <summary>
        /// Gets the <see cref="ScriptableSettingsType"/> for the specified <paramref name="type"/>.
        /// </summary>
        public static ScriptableSettingsType GetType(Type type)
        {
            ProjectSettingsAttribute projectSettingsAttribute = type.GetCustomAttribute<ProjectSettingsAttribute>();

            if (projectSettingsAttribute != null)
            {
                return projectSettingsAttribute.IsEditorOnly ? ScriptableSettingsType.EditorProjectSettings : ScriptableSettingsType.RuntimeProjectSettings;
            }

            PreferencesAttribute preferencesAttribute = type.GetCustomAttribute<PreferencesAttribute>();

            if (preferencesAttribute != null)
            {
                return preferencesAttribute.UseEditorPrefs ? ScriptableSettingsType.EditorUserPreferences : ScriptableSettingsType.ProjectUserPreferences;
            }

            return ScriptableSettingsType.Custom;
        }

        /// <inheritdoc cref="GetType(System.Type)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptableSettingsType GetType<T>()
            where T : ScriptableSettings
        {
            return GetType(typeof(T));
        }

        /// <summary>
        /// Sets the value for the specified type, if not set yet.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <param name="value">The new value for the specified type.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(Type type, ScriptableSettings value)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
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
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
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
        public static bool TryGet(Type type, [NotNullWhen(true)] out ScriptableSettings result)
        {
            if (Map.TryGetValue(type, out ScriptableSettings value))
            {
                return value.TryGetValid(out result);
            }

            result = null;

            return false;
        }

        /// <inheritdoc cref="TryGet"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>([NotNullWhen(true)] out T result)
            where T : ScriptableSettings
        {
            if (TryGet(typeof(T), out ScriptableSettings rawResult))
            {
                result = (T)rawResult;

                return true;
            }

            result = null;

            return false;
        }

        /// <summary>
        /// Tries to get the last set value for the specified type, but also tries to find one through <see cref="Resources.FindObjectsOfTypeAll"/> if not set.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <param name="result">The settings if set and still valid or if a new one could be found.</param>
        /// <param name="findCallback">How to find a new instance. Defaults to use <see cref="FindSingle"/>.</param>.
        /// <returns>True if the settings is set and still valid or if a new one could be found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetOrFind(Type type, [NotNullWhen(true)] out ScriptableSettings result, FindHandler findCallback = null)
        {
            result = GetOrFind(type, findCallback);

            return result.IsValid();
        }

        /// <inheritdoc cref="TryGetOrFind"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetOrFind<T>([NotNullWhen(true)] out T result, FindHandler findCallback = null)
            where T : ScriptableSettings
        {
            result = GetOrFind(typeof(T), findCallback) as T;

            return result.IsValid();
        }

        protected virtual void Reset()
        {
#if UNITY_EDITOR
            if (Type.IsEditorOnly())
            {
                Preload = false;
            }

            if (Preload)
            {
                EnsurePreload(false);
            }
#endif
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            GUI.changed = true;
            UnityEditor.EditorUtility.SetDirty(this);

            if (Type.IsEditorOnly())
            {
                Preload = false;
            }

            if (Preload)
            {
                EnsurePreload(true);
            }
            else
            {
                using (ListPool.Pop(out List<Object> pooledList))
                {
                    pooledList.AddRange(UnityEditor.PlayerSettings.GetPreloadedAssets());

                    if (!pooledList.Remove(this))
                    {
                        pooledList.Clear();

                        return;
                    }

                    while (pooledList.Remove(this))
                    {
                        // remove all occurrences
                    }

                    UnityEditor.PlayerSettings.SetPreloadedAssets(pooledList.ToArray());
                }
            }
#endif
        }

        protected virtual void OnEnable()
        {
            Type type = GetType();

            if (string.IsNullOrWhiteSpace(name))
            {
                name = type.Name;
            }

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
            value = value.GetValid();

            if (TryGet(type, out ScriptableSettings currentValue) && value != currentValue)
            {
                if (forceSet)
                {
                    LogIfOverriding(type, value, currentValue);
                }
                else
                {
                    if (GetType(type).IsEditorOnly())
                    {
                        return;
                    }

                    Debug.LogWarning($"{type} in {nameof(ScriptableSettings)} is already set to \"{currentValue}\"!", currentValue);
                    Debug.LogWarning($"{type} in {nameof(ScriptableSettings)} can't be overriden to \"{value}\".", value);

                    return;
                }
            }

            if (value != null)
            {
                Debug.Assert(type.IsInstanceOfType(value));
                Map[type] = value;
            }
            else
            {
                Map.Remove(type);
            }
        }

        private static void LogIfOverriding(Type type, ScriptableSettings value, ScriptableSettings currentValue)
        {
            if (CoimbraUtility.IsReloadingScripts || IsQuitting || GetType(type).IsEditorOnly())
            {
                return;
            }

            Debug.LogWarning($"Overriding {type} in {nameof(ScriptableSettings)} from \"{currentValue}\"!", currentValue);
            Debug.LogWarning($"Overriding {type} in {nameof(ScriptableSettings)} to \"{value}\"!", value);
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void Initialize()
        {
            IsQuitting = false;
            Map.Clear();
            UnityEditor.PlayerSettings.GetPreloadedAssets();
        }

        private void EnsurePreload(bool withWarning)
        {
            using (ListPool.Pop(out List<Object> pooledList))
            {
                pooledList.AddRange(UnityEditor.PlayerSettings.GetPreloadedAssets());

                if (pooledList.Contains(this))
                {
                    return;
                }

                if (withWarning)
                {
                    Debug.LogWarning($"Fixing \"{this}\" not being added to the preloaded assets.", this);
                }

                pooledList.Add(this);
                UnityEditor.PlayerSettings.SetPreloadedAssets(pooledList.ToArray());
            }
        }
#endif
    }
}
