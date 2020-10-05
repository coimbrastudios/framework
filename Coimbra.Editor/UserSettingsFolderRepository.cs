using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.SettingsManagement
{
    [Serializable]
    internal sealed class UserSettingsFolderRepository : ISettingsRepository
    {
        [SerializeField] private string _name;
        [SerializeField] private string _path;
        [SerializeField] private SettingsDictionary _dictionary = new SettingsDictionary();

        private const bool PrettyPrintJson = true;
        private const string SettingsDirectory = "UserSettings/Packages";
        private bool _initialized;
        private string _cachedJson;

        internal UserSettingsFolderRepository(string package, string name)
        {
            _name = name;
            _path = GetSettingsPath(package, name);
            _initialized = false;

            AssemblyReloadEvents.beforeAssemblyReload += Save;
            EditorApplication.quitting += Save;
        }

        public string name => _name;

        public string path => _path;

        public SettingsScope scope => SettingsScope.User;

        public static string GetSettingsPath(string packageName, string name = "Settings")
        {
            return $"{SettingsDirectory}/{packageName}/{name}.json";
        }

        public bool ContainsKey<T>(string key)
        {
            Initialize();

            return _dictionary.ContainsKey<T>(key);
        }

        public T Get<T>(string key, T fallback = default)
        {
            Initialize();

            return _dictionary.Get(key, fallback);
        }

        public void Remove<T>(string key)
        {
            Initialize();
            _dictionary.Remove<T>(key);
        }

        public void Save()
        {
            Initialize();

            if (!File.Exists(path))
            {
                string directory = Path.GetDirectoryName(path);
                Assert.IsFalse(string.IsNullOrWhiteSpace(directory));
                Directory.CreateDirectory(directory);
            }

            string newSettingsJson = EditorJsonUtility.ToJson(this, PrettyPrintJson);
            bool areJsonsEqual = newSettingsJson == _cachedJson;

            if (!AssetDatabase.IsOpenForEdit(path) && areJsonsEqual == false)
            {
                if (!AssetDatabase.MakeEditable(path))
                {
                    Debug.LogWarning($"Could not save package settings to {path}");

                    return;
                }
            }

            try
            {
                if (!areJsonsEqual)
                {
                    File.WriteAllText(path, newSettingsJson);
                    _cachedJson = newSettingsJson;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Debug.LogWarning($"Could not save package settings to {path}");
            }
        }

        public void Set<T>(string key, T value)
        {
            Initialize();
            _dictionary.Set(key, value);
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            if (File.Exists(path))
            {
                _dictionary = null;
                _cachedJson = File.ReadAllText(path);
                EditorJsonUtility.FromJsonOverwrite(_cachedJson, this);

                if (_dictionary == null)
                {
                    _dictionary = new SettingsDictionary();
                }
            }
        }
    }
}
