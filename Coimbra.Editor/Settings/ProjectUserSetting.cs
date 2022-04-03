using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Coimbra.Editor
{
    internal sealed class ProjectUserSetting<T> : UserSetting<T>
    {
        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
        internal ProjectUserSetting(string key, T value)
            : base(FrameworkSettingsProvider.Settings, FrameworkSettingsProvider.ProjectUserSettingsName, key, value, SettingsScope.User) { }
    }
}
