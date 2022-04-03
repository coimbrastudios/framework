using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Coimbra.Editor
{
    internal sealed class EditorUserSetting<T> : UserSetting<T>
    {
        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
        internal EditorUserSetting(string key, T value)
            : base(FrameworkSettingsProvider.Settings, FrameworkSettingsProvider.EditorUserSettingsName, key, value, SettingsScope.User) { }
    }
}
