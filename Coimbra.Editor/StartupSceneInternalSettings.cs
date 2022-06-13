using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor
{
    [Preferences(null, false)]
    internal sealed class StartupSceneInternalSettings : ScriptableSettings
    {
        [field: SerializeField]
        public bool HasSavedStartupScene { get; set; }

        [field: SerializeField]
        public SceneAsset SavedStartupScene { get; set; }
    }
}
