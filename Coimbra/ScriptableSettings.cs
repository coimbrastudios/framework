#nullable enable

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
    /// <seealso cref="ScriptableSettingsTypeUtility"/>
    /// <seealso cref="ScriptableSettingsProviderAttribute"/>
    /// <seealso cref="IScriptableSettingsProvider"/>
    /// <seealso cref="FindAnywhereScriptableSettingsProvider"/>
    /// <seealso cref="LoadOrCreateScriptableSettingsProvider"/>
    [RequireDerived]
    public abstract class ScriptableSettings : ScriptableObject
    {
        internal sealed class Instance
        {
            internal readonly IScriptableSettingsProvider Provider;

            internal Instance(IScriptableSettingsProvider provider)
            {
                Provider = provider;
            }

            internal ScriptableSettings? Current { get; set; }

            internal ScriptableSettings? Default { get; set; }
        }

#pragma warning disable CS0618
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(nameof(ScriptableSettings) + "." + nameof(FindHandler) + " shouldn't be used anymore, use the new " + nameof(ScriptableSettingsProviderAttribute) + " instead.")]
        public delegate ScriptableSettings FindHandler(Type type);

        // ReSharper disable once UnassignedReadonlyField
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(nameof(ScriptableSettings) + "." + nameof(FindSingle) + " shouldn't be used anymore, use the new " + nameof(ScriptableSettingsProviderAttribute) + " instead.")]
        public static readonly FindHandler? FindSingle;
#pragma warning restore CS0618

        internal static readonly Dictionary<Type, Instance> Map = new();

        private static readonly Object?[] SaveTarget = new Object?[1];

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
            Type = GetTypeData(GetType());

            if (IsDefault || Type.IsEditorOnly())
            {
                _preload = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this setting was created as a default setting.
        /// </summary>
        [PublicAPI]
        public bool IsDefault { get; private set; }

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
        /// Gets the last set value for the specified type. If none, will fallbacks to its type <see cref="IScriptableSettingsProvider"/>.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <returns>The settings if set and still valid or if a new one could be found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptableSettings? Get(Type type)
        {
            Get(type, out ScriptableSettings? value, false);

            return value;
        }

        /// <inheritdoc cref="Get"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Get<T>()
            where T : ScriptableSettings
        {
            Get(typeof(T), out ScriptableSettings? value, false);

            return (T?)value;
        }

        /// <summary>
        /// Tries to get a value with <see cref="Get"/>. If none, fallbacks to a default created instance.
        /// </summary>
        /// <returns>Always a valid instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrDefault<T>()
            where T : ScriptableSettings
        {
            Get(typeof(T), out ScriptableSettings? value, true);

            return (T)value!;
        }

        /// <inheritdoc cref="GetOrDefault{T}()"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetOrDefault<T>(out T value)
            where T : ScriptableSettings
        {
            Get(typeof(T), out ScriptableSettings? raw, true);

            value = (T)raw!;
        }

#pragma warning disable CS0618
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettings) + "." + nameof(GetOrFind) + " shouldn't be used anymore, use either " + nameof(Get) + ", " + nameof(GetOrDefault) + ", or " + nameof(IsSet) + " instead.")]
        public static ScriptableSettings? GetOrFind(Type type, FindHandler? findHandler = null)
        {
            return Get(type);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettings) + "." + nameof(GetOrFind) + " shouldn't be used anymore, use either " + nameof(Get) + ", " + nameof(GetOrDefault) + ", or " + nameof(IsSet) + " instead.")]
        public static T? GetOrFind<T>(FindHandler? findHandler = null)
            where T : ScriptableSettings
        {
            return Get<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettings) + "." + nameof(GetType) + " shouldn't be used anymore, use " + nameof(GetTypeData) + " instead.")]
        public static ScriptableSettingsType GetType(Type type)
        {
            return GetTypeData(type);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettings) + "." + nameof(GetType) + " shouldn't be used anymore, use " + nameof(GetTypeData) + " instead.")]
        public static ScriptableSettingsType GetType<T>()
            where T : ScriptableSettings
        {
            return GetTypeData(typeof(T));
        }
#pragma warning restore CS0618

        /// <summary>
        /// Gets the <see cref="ScriptableSettingsType"/> for the <paramref name="type"/>.
        /// </summary>
        public static ScriptableSettingsType GetTypeData(Type type)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));

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

        /// <summary>
        /// Gets the <see cref="ProjectSettingsAttribute"/> or <see cref="PreferencesAttribute"/> data for a <see cref="ScriptableSettings"/> type.
        /// </summary>
        /// <returns>The <see cref="ScriptableSettingsType"/> for the specified <paramref name="type"/>.</returns>
        public static ScriptableSettingsType GetTypeData(Type type, out string? windowPath, out string? filePath, out string[]? keywords)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));

            windowPath = null;
            filePath = null;
            keywords = null;

            ProjectSettingsAttribute projectSettingsAttribute = type.GetCustomAttribute<ProjectSettingsAttribute>();

            if (projectSettingsAttribute != null)
            {
                windowPath = $"{projectSettingsAttribute.WindowPath}/{projectSettingsAttribute.NameOverride ?? ApplicationUtility.GetDisplayName(type.Name)}";
                filePath = projectSettingsAttribute is { IsEditorOnly: true, FileDirectory: { } } ? $"{projectSettingsAttribute.FileDirectory}/{projectSettingsAttribute.FileNameOverride ?? $"{type.Name}.asset"}" : null;
                keywords = projectSettingsAttribute.Keywords;

                return projectSettingsAttribute.IsEditorOnly ? ScriptableSettingsType.EditorProjectSettings : ScriptableSettingsType.RuntimeProjectSettings;
            }

            PreferencesAttribute preferencesAttribute = type.GetCustomAttribute<PreferencesAttribute>();

            if (preferencesAttribute != null)
            {
                windowPath = preferencesAttribute.WindowPath != null ? $"{preferencesAttribute.WindowPath}/{preferencesAttribute.NameOverride ?? ApplicationUtility.GetDisplayName(type.Name)}" : null;
                filePath = preferencesAttribute.UseEditorPrefs ? null : $"{preferencesAttribute.FileDirectory}/{preferencesAttribute.FileNameOverride ?? $"{type.Name}.asset"}";
                keywords = preferencesAttribute.Keywords;

                return preferencesAttribute.UseEditorPrefs ? ScriptableSettingsType.EditorUserPreferences : ScriptableSettingsType.ProjectUserPreferences;
            }

            return ScriptableSettingsType.Custom;
        }

        /// <summary>
        /// Checks if the value is set for the specified type.
        /// </summary>
        /// <param name="type">The type of the settings.</param>
        /// <param name="result">The settings if set and still valid.</param>
        /// <returns>True if the settings is set and still valid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(Type type, [NotNullWhen(true)] out ScriptableSettings? result)
        {
            if (Map.TryGetValue(type, out Instance instance) && instance.Current.TryGetValid(out result) && !result.IsDefault)
            {
                return true;
            }

            result = null;

            return false;
        }

        /// <inheritdoc cref="IsSet"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet<T>([NotNullWhen(true)] out T? result)
            where T : ScriptableSettings
        {
            if (IsSet(typeof(T), out ScriptableSettings? rawResult))
            {
                result = (T)rawResult;

                return true;
            }

            result = null;

            return false;
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
        public static void Set(Type type, ScriptableSettings? value)
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
        public static void SetOrOverwrite(Type type, ScriptableSettings? value)
        {
            Debug.Assert(typeof(ScriptableSettings).IsAssignableFrom(type));
            Set(true, type, value);
        }

        /// <inheritdoc cref="SetOrOverwrite"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetOrOverwrite<T>(T? value)
            where T : ScriptableSettings
        {
            Set(true, typeof(T), value);
        }

        /// <summary>
        /// Tries to get a value with <see cref="Get"/>. If none, return false and the <paramref name="value"/> will be null.
        /// </summary>
        /// <returns>Always a valid instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet(Type type, [NotNullWhen(true)] out ScriptableSettings? value)
        {
            return Get(type, out value, false);
        }

        /// <inheritdoc cref="TryGet"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>([NotNullWhen(true)] out T? value)
            where T : ScriptableSettings
        {
            bool isSet = Get(typeof(T), out ScriptableSettings? raw, false);
            value = (T?)raw;

            return isSet;
        }

#pragma warning disable CS0618
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettings) + "." + nameof(TryGetOrFind) + " shouldn't be used anymore, use " + nameof(TryGet) + " instead.")]
        public static bool TryGetOrFind(Type type, [NotNullWhen(true)] out ScriptableSettings? value, FindHandler? findHandler = null)
        {
            return TryGet(type, out value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettings) + "." + nameof(TryGetOrFind) + " shouldn't be used anymore, use " + nameof(TryGet) + " instead.")]
        public static bool TryGetOrFind<T>([NotNullWhen(true)] out T? value, FindHandler? findHandler = null)
            where T : ScriptableSettings
        {
            return TryGet(out value);
        }
#pragma warning restore CS0618

        /// <summary>
        /// Tries to load a <see cref="ScriptableSettings"/> from the disk.
        /// </summary>
        /// <returns>True if loaded with success, false if it fails.</returns>
        public static bool TryLoad(Type type, string? filePath, ScriptableSettingsType filter, [NotNullWhen(true)] out ScriptableSettings? scriptableSettings)
        {
#if UNITY_EDITOR
            Object[] objects = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(filePath);

            foreach (Object o in objects)
            {
                if (o is ScriptableSettings match && match.Type == filter)
                {
                    Set(type, match);
                    scriptableSettings = match;

                    return true;
                }
            }
#endif

            scriptableSettings = null;

            return false;
        }

        /// <summary>
        /// Reloads this <see cref="ScriptableSettings"/> from the disk, if supported.
        /// </summary>
        public void Reload()
        {
#if UNITY_EDITOR
            ScriptableSettingsType type = GetTypeData(GetType(), out _, out string? filePath, out _);

            if (type == ScriptableSettingsType.Custom || IsDefault || !string.IsNullOrWhiteSpace(UnityEditor.AssetDatabase.GetAssetPath(this)))
            {
                return;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                if (!TryLoad(GetType(), filePath, Type, out ScriptableSettings? target))
                {
                    target = (ScriptableSettings)CreateInstance(GetType());
                }

                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(target), this);
                DestroyImmediate(target, false);
            }
            else if (type.IsUserPreferences())
            {
                ScriptableSettings target = (ScriptableSettings)CreateInstance(GetType());
                string defaultValue = UnityEditor.EditorJsonUtility.ToJson(target, false);
                string newValue = UnityEditor.EditorPrefs.GetString(ApplicationUtility.GetPrefsKey(GetType()), defaultValue);
                UnityEditor.EditorJsonUtility.FromJsonOverwrite(newValue, this);

                if (Type != type)
                {
                    JsonUtility.FromJsonOverwrite(defaultValue, this);
                }

                DestroyImmediate(target, false);
            }
#endif
        }

        /// <summary>
        /// Saves this <see cref="ScriptableSettings"/> to the disk, if supported.
        /// </summary>
        public void Save()
        {
#if UNITY_EDITOR
            ScriptableSettingsType type = GetTypeData(GetType(), out _, out string? filePath, out _);

            if (type == ScriptableSettingsType.Custom || IsDefault)
            {
                return;
            }

            if (filePath == null)
            {
                if (type.IsProjectSettings())
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssetIfDirty(this);

                    return;
                }

                string value = UnityEditor.EditorJsonUtility.ToJson(this, true);
                UnityEditor.EditorPrefs.SetString(ApplicationUtility.GetPrefsKey(GetType()), value);

                return;
            }

            if (type.IsProjectSettings() && !string.IsNullOrWhiteSpace(UnityEditor.AssetDatabase.GetAssetPath(this)))
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(this);

                return;
            }

            string? directoryName = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            SaveTarget[0] = this;
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(SaveTarget, filePath, true);

            SaveTarget[0] = null;
#endif
        }

        internal void ValidatePreload()
        {
#if UNITY_EDITOR
            GUI.changed = true;
            UnityEditor.EditorUtility.SetDirty(this);

            if (IsDefault || Type.IsEditorOnly())
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
        /// Use this for one-time initializations instead of <see cref="OnEnable"/> callback. This method can be called inside edit-mode when inside the editor.
        /// </summary>
        protected virtual void OnLoaded()
        {
            if (!IsDefault)
            {
                Set(GetType(), this);
            }
        }

        /// <summary>
        /// Use this instead of the standard <see cref="Reset"/> callback.
        /// </summary>
        protected virtual void OnReset() { }

        /// <summary>
        /// Use this for one-time un-initializations instead of <see cref="OnDisable"/> callback. This method may be called inside edit-mode when inside the editor.
        /// </summary>
        protected virtual void OnUnload() { }

        /// <summary>
        /// Use this instead of the standard <see cref="OnValidate"/> callback.
        /// </summary>
        protected virtual void OnValidating() { }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnUnload"/> instead.
        /// </summary>
        protected void OnDisable()
        {
            OnUnload();
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
        /// Non-virtual by design, use <see cref="OnReset"/> instead.
        /// </summary>
        protected void Reset()
        {
#if UNITY_EDITOR
            if (IsDefault || Type.IsEditorOnly())
            {
                _preload = false;
            }

            if (_preload)
            {
                EnsurePreload(false);
            }
#endif

            OnReset();
        }

        /// <summary>
        /// Non-virtual by design, use <see cref="OnValidating"/> instead.
        /// </summary>
        protected void OnValidate()
        {
            ValidatePreload();
            OnValidating();
        }

        private static void HandleApplicationQuitting()
        {
            IsQuitting = true;
        }

        private static bool Get(Type type, out ScriptableSettings? value, bool allowDefault)
        {
            if (Map.TryGetValue(type, out Instance instance))
            {
                if (instance.Current.TryGetValid(out value))
                {
                    if (allowDefault || !value.IsDefault)
                    {
                        return true;
                    }
                }
            }
            else
            {
                instance = InitializeInstance(type);
            }

            {
                value = instance.Provider.GetScriptableSettings(type);

                if (value.TryGetValid(out value))
                {
                    instance.Current = value;

                    return true;
                }

                if (!allowDefault)
                {
                    return false;
                }

                if (instance.Default == null)
                {
                    instance.Default = (ScriptableSettings)CreateInstance(type);
                    instance.Default.IsDefault = true;
                }

                value = instance.Default;
                instance.Current = value;

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Instance InitializeInstance(Type type)
        {
            ScriptableSettingsProviderAttribute? providerAttribute = type.GetCustomAttribute<ScriptableSettingsProviderAttribute>();

            if (providerAttribute == null || !providerAttribute.Type.TryCreateInstance(out IScriptableSettingsProvider provider))
            {
                provider = FindAnywhereScriptableSettingsProvider.Default;
            }

            Instance value = new(provider);
            Map.Add(type, value);

            return value;
        }

        private static void Set(bool forceSet, Type type, ScriptableSettings? value)
        {
            value = value.GetValid();

            if (Map.TryGetValue(type, out Instance instance))
            {
                if (instance.Current.TryGetValid(out ScriptableSettings? current) && !current.IsDefault && value != current)
                {
                    if (forceSet)
                    {
                        if (!ApplicationUtility.IsReloadingScripts && !IsQuitting && !GetTypeData(type).IsEditorOnly())
                        {
                            Debug.Log($"Overriding {type} in {nameof(ScriptableSettings)} from \"{current}\"!", current);
                            Debug.Log($"Overriding {type} in {nameof(ScriptableSettings)} to \"{value}\"!", value);
                        }
                    }
                    else
                    {
                        if (GetTypeData(type).IsEditorOnly())
                        {
                            return;
                        }

                        Debug.LogWarning($"{type} in {nameof(ScriptableSettings)} is already set to \"{current}\"!", current);
                        Debug.LogWarning($"{type} in {nameof(ScriptableSettings)} can't be overriden to \"{value}\".", value);

                        return;
                    }
                }
            }
            else
            {
                instance = InitializeInstance(type);
            }

            if (value != null)
            {
                Debug.Assert(type.IsInstanceOfType(value));
                instance.Current = value;
            }
            else
            {
                instance.Current = null;
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
