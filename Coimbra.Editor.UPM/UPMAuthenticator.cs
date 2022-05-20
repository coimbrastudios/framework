using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using Tommy;
using UnityEditor;
using UnityEngine;

namespace Coimbra.Editor.UPM
{
    [ProjectSettings(CoimbraUtility.ProjectSettingsPath, true, FileDirectory = CoimbraUtility.ProjectSettingsFilePath)]
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

        [field: SerializeField]
        [UsedImplicitly]
        private List<Entry> Entries { get; set; } = new List<Entry>();

        private bool IsInitialized { get; set; }

        protected override void OnValidate()
        {
            base.OnValidate();

            IsInitialized = false;
        }

        [InitializeOnLoadMethod]
        internal static void Update()
        {
            try
            {
                UPMAuthenticator authenticator = GetOrFind<UPMAuthenticator>();

                if (authenticator == null)
                {
                    EditorApplication.delayCall += Update;

                    return;
                }

                if (authenticator.IsInitialized)
                {
                    return;
                }

                authenticator.IsInitialized = true;

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

                TomlTable table;

                using (StreamReader reader = File.OpenText(file))
                {
                    try
                    {
                        table = TOML.Parse(reader);
                    }
                    catch (TomlParseException ex)
                    {
                        table = ex.ParsedTable;

                        foreach (TomlSyntaxException syntaxEx in ex.SyntaxErrors)
                        {
                            Console.WriteLine($"Error on {syntaxEx.Column}:{syntaxEx.Line}: {syntaxEx.Message}");
                        }
                    }
                }

                string previous = table.ToString();

                foreach (Entry entry in authenticator.Entries)
                {
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
    }
}
