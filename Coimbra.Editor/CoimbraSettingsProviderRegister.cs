#nullable enable

using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Coimbra.Editor
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    internal static class CoimbraSettingsProviderRegister
    {
        private static Settings? _settings;

        internal static Settings Settings => _settings ??= new Settings(new ISettingsRepository[]
        {
            new ProjectUserSettings(CoimbraUtility.PackageName),
            new UserSettingsRepository(),
        });

        [SettingsProvider]
        private static SettingsProvider CreateProjectSettingsProvider()
        {
            return new UserSettingsProvider(CoimbraUtility.ProjectSettingsPath, Settings, new[]
            {
                typeof(CoimbraSettingsProviderRegister).Assembly,
            }, SettingsScope.Project);
        }
    }
}
