
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEditor;
using UnityEditor.SettingsManagement;
#if !UNITY_2021_2_OR_NEWER
using JetBrains.Annotations;
#endif

namespace Coimbra.Editor
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    internal static class CoimbraSettingsProviderRegister
    {
        internal const string EditorUserSettingsName = "EditorPrefs";

        internal const string ProjectSettingsName = "ProjectSettings";

        internal const string ProjectUserSettingsName = "UserSettings";

        private static Settings _settings;

        [NotNull]
        internal static Settings Settings => _settings ??= new Settings(new ISettingsRepository[]
        {
            new PackageSettingsRepository(CoimbraUtility.PackageName, ProjectSettingsName),
            new ProjectUserSettings(CoimbraUtility.PackageName, ProjectUserSettingsName),
            new UserSettingsRepository(),
        });

        [SettingsProvider]
        private static SettingsProvider CreateEditorUserSettingsProvider()
        {
            return new UserSettingsProvider(CoimbraUtility.EditorUserPreferencesPath, Settings, Array.Empty<Assembly>(), SettingsScope.User);
        }

        [SettingsProvider]
        private static SettingsProvider CreateProjectSettingsProvider()
        {
            return new UserSettingsProvider(CoimbraUtility.ProjectSettingsPath, Settings, new[]
            {
                typeof(CoimbraSettingsProviderRegister).Assembly,
            }, SettingsScope.Project);
        }

        [SettingsProvider]
        private static SettingsProvider CreateProjectUserSettingsProvider()
        {
            return new UserSettingsProvider(CoimbraUtility.ProjectUserPreferencesPath, Settings, Array.Empty<Assembly>(), SettingsScope.User);
        }
    }
}
