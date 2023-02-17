#nullable enable

using CoimbraInternal.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace Coimbra.Editor
{
    [InitializeOnLoad]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ScriptableSettingsUtility
    {
        static ScriptableSettingsUtility()
        {
            UnityEditorInternals.OnEditorApplicationFocusChanged -= HandleEditorApplicationFocusChanged;
            UnityEditorInternals.OnEditorApplicationFocusChanged += HandleEditorApplicationFocusChanged;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettingsUtility) + "." + nameof(Reload) + " shouldn't be used anymore, use " + nameof(ScriptableSettings) + "." + nameof(ScriptableSettings.Reload) + " instead.")]
        public static void Reload(this ScriptableSettings scriptableSettings)
        {
            scriptableSettings.Reload();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettingsUtility) + "." + nameof(LoadOrCreate) + " shouldn't be used anymore, use " + nameof(ScriptableSettings) + "." + nameof(ScriptableSettings.Get) + " instead.")]
        public static ScriptableSettings? LoadOrCreate(Type type, ScriptableSettings.FindHandler? findCallback = null)
        {
            return ScriptableSettings.Get(type);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettingsUtility) + "." + nameof(LoadOrCreate) + " shouldn't be used anymore, use " + nameof(ScriptableSettings) + "." + nameof(ScriptableSettings.Get) + " instead.")]
        public static T? LoadOrCreate<T>(ScriptableSettings.FindHandler? findCallback = null)
            where T : ScriptableSettings
        {
            return ScriptableSettings.Get<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettingsUtility) + "." + nameof(Save) + " shouldn't be used anymore, use " + nameof(ScriptableSettings) + "." + nameof(ScriptableSettings.Save) + " instead.")]
        public static void Save(this ScriptableSettings scriptableSettings)
        {
            scriptableSettings.Save();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettingsUtility) + "." + nameof(TryLoadOrCreate) + " shouldn't be used anymore, use " + nameof(ScriptableSettings) + "." + nameof(ScriptableSettings.TryGet) + " instead.")]
        public static bool TryGetAttributeData(Type type, [NotNullWhen(true)] out SettingsScope? settingsScope, out string? windowPath, out string? filePath, out string[]? keywords)
        {
            switch (ScriptableSettings.GetTypeData(type, out windowPath, out filePath, out keywords))
            {
                case ScriptableSettingsType.EditorProjectSettings:
                case ScriptableSettingsType.RuntimeProjectSettings:
                {
                    settingsScope = SettingsScope.Project;

                    break;
                }

                case ScriptableSettingsType.EditorUserPreferences:
                case ScriptableSettingsType.ProjectUserPreferences:
                {
                    settingsScope = SettingsScope.User;

                    break;
                }

                default:
                {
                    settingsScope = null;

                    break;
                }
            }

            return settingsScope != null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettingsUtility) + "." + nameof(TryLoadOrCreate) + " shouldn't be used anymore, use " + nameof(ScriptableSettings) + "." + nameof(ScriptableSettings.TryGet) + " instead.")]
        public static bool TryLoadOrCreate<T>([NotNullWhen(true)] out T? scriptableSettings, ScriptableSettings.FindHandler? findCallback = null)
            where T : ScriptableSettings
        {
            return ScriptableSettings.TryGet(out scriptableSettings);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(nameof(ScriptableSettingsUtility) + "." + nameof(GetPrefsKey) + " shouldn't be used anymore, use " + nameof(ApplicationUtility) + "." + nameof(ApplicationUtility.GetPrefsKey) + " instead.")]
        public static string GetPrefsKey(Type type)
        {
            return ApplicationUtility.GetPrefsKey(type);
        }

        private static void HandleEditorApplicationFocusChanged(bool isFocused)
        {
            foreach (KeyValuePair<Type, ScriptableSettings.Instance> pair in ScriptableSettings.Map)
            {
                if (pair.Value.Current.TryGetValid(out ScriptableSettings? value))
                {
                    value.Reload();
                }
            }
        }
    }
}
