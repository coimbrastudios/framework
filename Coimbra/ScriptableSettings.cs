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
    /// Class that allows easy access to a <see cref="ScriptableObject"/>.
    /// </summary>
    /// <remarks>
    /// This class is meant for globally-accessible read-only data containers. Those data containers can have different level os scopes:
    /// <list type="bullet">
    /// <item><see cref="ScriptableSettingsType.Custom"/>: those are dynamic objects generally either created at runtime or that the current instance changes at different stages of the application (i.e. a different asset according to selected difficulty).</item>
    /// <item><see cref="ScriptableSettingsType.EditorProjectSettings"/>: configured in the <b>Project Settings</b> window and saved at the <b>ProjectSettings</b> folder. Those settings are shared between all users of the current project and are never available outside the editor.</item>
    /// <item><see cref="ScriptableSettingsType.RuntimeProjectSettings"/>: configured in the <b>Project Settings</b> window and saved in the <b>Assets</b> folder (in a sub-folder of your choice). Those settings have the option to be included with the Preloaded Assets to be available since the application startup in the builds.</item>
    /// <item><see cref="ScriptableSettingsType.EditorUserPreferences"/>: configured in the <b>Preferences</b> window and saved using <b>EditorPrefs</b>. Those settings are local for the user machine, meaning that they are shader across all projects using the same Unity editor, and are never available outside the editor.</item>
    /// <item><see cref="ScriptableSettingsType.ProjectUserPreferences"/>: configured in the <b>Preferences</b> window and saved at the <b>UserSettings</b> folder. Those settings are local for the project, meaning that they won't affect other projects or other users, and are never available outside the editor.</item>
    /// </list>
    /// You can check all currently loaded <see cref="ScriptableSettings"/> by going to <b>Window/Coimbra Framework/Scriptable Settings</b>.
    /// </remarks>
    /// <seealso cref="PreferencesAttribute"/>
    /// <seealso cref="ProjectSettingsAttribute"/>
    /// <seealso cref="ScriptableSettingsType"/>
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
            Object[] rawValues = ObjectUtility.FindAllAnywhere(type);

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
                _preload = false;
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
            protected set
            {
                _preload = value;
                ValidatePreload();
            }
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
        protected internal static bool IsQuitting { get; internal set; }

        /// <summary>
        /// Gets the last set value for the specified type, but also tries to find one if not set.
        /// </summary>
        /// <remarks>
        /// It will never call <see cref="Set"/> or <see cref="SetOrOverwrite"/> for you, but you can use a custom <paramref name="findCallback"/> to do it if no value is found.
        /// </remarks>
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
        /// <remarks>
        /// If another value is already set then it will emit a warning and don't do anything.
        /// <para></para>
        /// If overwriting is intended use <see cref="SetOrOverwrite"/>.
        /// </remarks>
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
        /// <remarks>
        /// It will set the new value even if another value is already set.
        /// <para></para>
        /// If overwriting is intended use <see cref="SetOrOverwrite"/>.
        /// </remarks>
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

        internal void ValidatePreload()
        {
#if UNITY_EDITOR
            GUI.changed = true;
            UnityEditor.EditorUtility.SetDirty(this);

            if (Type.IsEditorOnly())
            {
                _preload = false;
            }

            if (_preload)
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

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            if (Type.IsEditorOnly())
            {
                _preload = false;
            }

            if (_preload)
            {
                EnsurePreload(false);
            }
#endif
        }

        /// <summary>
        /// Use this for one-time initializations instead of <see cref="OnEnable"/> callback. This method can be called inside edit-mode when inside the editor.
        /// </summary>
        protected virtual void OnLoaded()
        {
            Set(GetType(), this);
        }

        /// <summary>
        /// Use this for one-time un-initializations instead of <see cref="OnDisable"/> callback. This method can be called inside edit-mode when inside the editor.
        /// </summary>
        protected virtual void OnUnload(bool wasCurrentInstance) { }

        /// <summary>
        /// Unity callback.
        /// </summary>
        protected virtual void OnValidate()
        {
            ValidatePreload();
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnLoaded"/> instead.
        /// </summary>
        protected void OnEnable()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = GetType().Name;
            }

            OnLoaded();
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnUnload"/> instead.
        /// </summary>
        protected void OnDisable()
        {
            Type type = GetType();
            bool wasCurrentInstance = TryGet(type, out ScriptableSettings current) && current == this;

            if (wasCurrentInstance)
            {
                SetOrOverwrite(type, null);
            }

            OnUnload(wasCurrentInstance);
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
                    if (!ApplicationUtility.IsReloadingScripts && !IsQuitting && !GetType(type).IsEditorOnly())
                    {
                        Debug.Log($"Overriding {type} in {nameof(ScriptableSettings)} from \"{currentValue}\"!", currentValue);
                        Debug.Log($"Overriding {type} in {nameof(ScriptableSettings)} to \"{value}\"!", value);
                    }
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

#if UNITY_EDITOR
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
