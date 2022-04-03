using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Coimbra.Editor
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    internal static class FrameworkSettingsProvider
    {
        internal const string EditorUserSettingsName = "EditorPrefs";
        internal const string ProjectSettingsName = "ProjectSettings";
        internal const string ProjectUserSettingsName = "UserSettings";

        private static UnityEditor.SettingsManagement.Settings _settings;

        [NotNull]
        internal static UnityEditor.SettingsManagement.Settings Settings => _settings ??= new UnityEditor.SettingsManagement.Settings(new ISettingsRepository[]
        {
            new PackageSettingsRepository(FrameworkUtility.PackageName, ProjectSettingsName),
            new ProjectUserSettings(FrameworkUtility.PackageName, ProjectUserSettingsName),
            new UserSettingsRepository(),
        });

        // [SettingsProvider]
        private static SettingsProvider CreateEditorUserSettingsProvider()
        {
            return new UserSettingsProvider(FrameworkUtility.EditorUserPreferencesPath, Settings, new[]
            {
                typeof(FrameworkSettingsProvider).Assembly,
            }, SettingsScope.User);
        }

        [SettingsProvider]
        private static SettingsProvider CreateProjectSettingsProvider()
        {
            return new UserSettingsProvider(FrameworkUtility.ProjectSettingsPath, Settings, new[]
            {
                typeof(FrameworkSettingsProvider).Assembly,
            }, SettingsScope.Project);
        }

        // [SettingsProvider]
        private static SettingsProvider CreateProjectUserSettingsProvider()
        {
            return new UserSettingsProvider(FrameworkUtility.ProjectUserPreferencesPath, Settings, new[]
            {
                typeof(FrameworkSettingsProvider).Assembly,
            }, SettingsScope.User);
        }
    }
}
