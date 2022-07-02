#if COIMBRA_ROSLYN_SOURCE
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Coimbra.Editor
{
    internal static class RoslynUtility
    {
        private static readonly string PackageDirectory = Path.Combine(Environment.CurrentDirectory, "Packages", "com.coimbrastudios");

        [MenuItem(CoimbraUtility.ToolsMenuPath + "Update Roslyn DLLs (Debug) %&#G")]
        internal static void UpdateRoslynDLLsForDebug()
        {
            UpdateRoslynDLLs("debug");
        }

        [MenuItem(CoimbraUtility.ToolsMenuPath + "Update Roslyn DLLs (Release) %&G")]
        internal static void UpdateRoslynDLLsForRelease()
        {
            UpdateRoslynDLLs("release");
        }

        private static void UpdateRoslynDLLs(string type)
        {
            CoimbraEditorUtility.ClearConsoleWindow();

            bool isDirty = false;
            string[] directories = Directory.GetDirectories(PackageDirectory, "Coimbra*", SearchOption.TopDirectoryOnly);

            foreach (string directory in directories)
            {
                string[] destinationPaths = Directory.GetFiles(directory, "Coimbra.*.dll", SearchOption.AllDirectories);

                foreach (string destinationPath in destinationPaths)
                {
                    string sourcePath = Path.Combine(PackageDirectory, "Roslyn~", Path.GetFileNameWithoutExtension(destinationPath), "bin", type, "netstandard2.0", Path.GetFileName(destinationPath));
                    FileInfo source = new(sourcePath);

                    if (!source.Exists)
                    {
                        Debug.Log($"Missing \"{sourcePath}\"");

                        continue;
                    }

                    if (source.LastWriteTimeUtc.Ticks <= new FileInfo(destinationPath).LastWriteTimeUtc.Ticks)
                    {
                        Debug.Log($"Skipping \"{destinationPath}\"");

                        continue;
                    }

                    isDirty = true;
                    File.Copy(sourcePath, destinationPath, true);
                    Debug.Log($"Updated \"{destinationPath}\"");
                }
            }

            if (!isDirty)
            {
                return;
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            CompilationPipeline.RequestScriptCompilation();
            EditorUtility.RequestScriptReload();
        }
    }
}
#endif
