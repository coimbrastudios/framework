using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Coimbra.Editor
{
    internal sealed class CoimbraUserSetting<T> : UserSetting<T>
    {
        internal CoimbraUserSetting(string key, T value, SettingsScope scope)
            : base(CoimbraSettingsProviderRegister.Settings, "Settings", key, value, scope) { }
    }
}
