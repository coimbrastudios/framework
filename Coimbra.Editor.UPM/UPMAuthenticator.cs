using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using Tommy;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor.UPM
{
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath, true, FileDirectory = null)]
    internal sealed class UPMAuthenticator : ScriptableSettings
    {
        [Serializable]
        private struct Entry
        {
#pragma warning disable CS0649
            [UsedImplicitly]
            public string Address;

            [UsedImplicitly]
            public SerializableDictionary<string, string> Values;
#pragma warning restore CS0649
        }

        private const string BackupFormat = FileName + ".{0:yyyyMMddHHmmss}";

        private const string FileName = ".upmconfig.toml";

        [SerializeField]
        [FormerlySerializedAsBackingFieldOf("Entries")]
        private List<Entry> _entries = new();

        private IReadOnlyList<Entry> Entries => _entries;

        [InitializeOnLoadMethod]
        internal static void Update()
        {
            try
            {
                if (!TryGetOrFind(out UPMAuthenticator authenticator))
                {
                    return;
                }

                string folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                if (!Directory.Exists(folder))
                {
                    Debug.LogError($"Couldn't find \"{folder}\"!");

                    return;
                }

                string file = Path.Combine(folder, FileName);

                if (!File.Exists(file))
                {
                    File.Create(file).Close();
                }

                TomlTable table = GetTable(file);
                string previous = table.ToString();

                for (int i = 0; i < authenticator.Entries.Count; i++)
                {
                    Entry entry = authenticator.Entries[i];
                    string address = $"npmAuth.\"{entry.Address}\"";
                    table[address]["alwaysAuth"] = true;

                    foreach (KeyValuePair<string, string> value in entry.Values)
                    {
                        table[address][value.Key] = value.Value;
                    }
                }

                if (previous == table.ToString())
                {
                    return;
                }

                string backup = Path.Combine(folder, string.Format(BackupFormat, DateTime.UtcNow));
                File.Copy(file, backup);

                using (StreamWriter writer = File.CreateText(file))
                {
                    table.WriteTo(writer);
                    writer.Flush();
                }

                Debug.Log($"Updated \"{file}\" with backup at \"{backup}\"");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <inheritdoc/>
        protected override void OnValidate()
        {
            Preload = false;
            base.OnValidate();
        }

        private static TomlTable GetTable(in string file)
        {
            using StreamReader reader = File.OpenText(file);

            try
            {
                return TOML.Parse(reader);
            }
            catch (TomlParseException ex)
            {
                foreach (TomlSyntaxException syntaxEx in ex.SyntaxErrors)
                {
                    Debug.LogWarning($"Error on {syntaxEx.Column}:{syntaxEx.Line}: {syntaxEx.Message}");
                }

                return ex.ParsedTable;
            }
        }
    }
}
