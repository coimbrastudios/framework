using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Coimbra.Editor
{
    internal static class AssetsAssemblyCreator
    {
        private const string AssemblyDefinitionExtension = "asmdef";

        private const string AssemblyDefinitionReferenceExtension = "asmref";

        private const string AssetsAssemblyDefinitionName = "Assembly-CSharp-Assets";

        private const string AssetsEditorAssemblyDefinitionName = "Assembly-CSharp-Assets-Editor";

        private const string DefaultScriptFile = "AssemblyInfo.cs";

        private const string GuidFormat = "GUID:{0}";

        internal static void CreateAssetsAssemblies()
        {
            using (ListPool.Pop(out List<string> ignoredFolders))
            using (HashSetPool.Pop(out HashSet<string> runtimeGuids))
            using (HashSetPool.Pop(out HashSet<string> editorGuids))
            {
                string runtimePath = null;
                string editorPath = null;
                AssemblyDefinition runtimeAssembly = null;
                AssemblyDefinition editorAssembly = null;

                using (HashSetPool.Pop(out HashSet<string> runtimeNames))
                using (HashSetPool.Pop(out HashSet<string> editorNames))
                {
                    foreach (Assembly assembly in CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies))
                    {
                        runtimeNames.Add(assembly.name);
                    }

                    foreach (Assembly assembly in CompilationPipeline.GetAssemblies(AssembliesType.Editor))
                    {
                        editorNames.Add(assembly.name);
                    }

                    foreach (string guid in AssetDatabase.FindAssets($"t:{AssemblyDefinitionExtension}"))
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

                        if (asset == null)
                        {
                            continue;
                        }

                        string text = asset.text;
                        AssemblyDefinition assembly = JsonUtility.FromJson<AssemblyDefinition>(text);

                        if (!assembly.autoReferenced && text.Contains("autoReferenced"))
                        {
                            continue;
                        }

                        if (assembly.name == AssetsAssemblyDefinitionName)
                        {
                            runtimeAssembly = assembly;
                            runtimePath = path;

                            continue;
                        }

                        if (assembly.name == AssetsEditorAssemblyDefinitionName)
                        {
                            editorAssembly = assembly;
                            editorPath = path;

                            continue;
                        }

                        if (runtimeNames.Contains(assembly.name))
                        {
                            string assemblyGuid = string.Format(GuidFormat, guid);
                            editorGuids.Add(assemblyGuid);
                            runtimeGuids.Add(assemblyGuid);
                            ignoredFolders.Add(Path.GetDirectoryName(path)!.Replace('\\', '/'));
                        }
                        else if (editorNames.Contains(assembly.name))
                        {
                            editorGuids.Add(string.Format(GuidFormat, guid));
                            ignoredFolders.Add(Path.GetDirectoryName(path)!.Replace('\\', '/'));
                        }
                    }
                }

                CreateAssetsAssemblyDefinition(ref runtimePath, runtimeAssembly, runtimeGuids);
                CreateAssetsEditorAssemblyDefinition(ref editorPath, editorAssembly, editorGuids);
                ignoredFolders.Add(Path.GetDirectoryName(editorPath)!.Replace('\\', '/'));
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceSynchronousImport);
                CreateAssetsEditorAssemblyReferences(editorPath, ignoredFolders);
            }
        }

        private static void CreateAssetsAssemblyDefinition(ref string assemblyPath, AssemblyDefinition assemblyDefinition, ISet<string> autoReferencedGuids)
        {
            if (assemblyPath == null)
            {
                const string defaultAssemblyFolder = "Assets";
                const string defaultAssemblyPath = defaultAssemblyFolder + "/" + AssetsAssemblyDefinitionName + "." + AssemblyDefinitionExtension;
                const string defaultScriptPath = defaultAssemblyFolder + "/" + DefaultScriptFile;
                assemblyPath = defaultAssemblyPath;
                assemblyDefinition = new AssemblyDefinition(AssetsAssemblyDefinitionName, autoReferencedGuids.ToArray(), false);
                File.WriteAllText(defaultScriptPath, string.Empty);
            }
            else
            {
                autoReferencedGuids.UnionWith(assemblyDefinition.references);

                assemblyDefinition.references = autoReferencedGuids.ToArray();
            }

            File.WriteAllText(assemblyPath, JsonUtility.ToJson(assemblyDefinition, true));
        }

        private static void CreateAssetsEditorAssemblyDefinition(ref string assemblyPath, AssemblyDefinition assemblyDefinition, ISet<string> autoReferencedGuids)
        {
            if (assemblyPath == null)
            {
                const string defaultAssemblyFolder = "Assets/Editor";

                if (!Directory.Exists(defaultAssemblyFolder))
                {
                    Directory.CreateDirectory(defaultAssemblyFolder);
                }

                const string defaultAssemblyPath = defaultAssemblyFolder + "/" + AssetsEditorAssemblyDefinitionName + "." + AssemblyDefinitionExtension;
                const string defaultScriptPath = defaultAssemblyFolder + "/" + DefaultScriptFile;
                assemblyPath = defaultAssemblyPath;
                assemblyDefinition = new AssemblyDefinition(AssetsEditorAssemblyDefinitionName, autoReferencedGuids.ToArray(), true);
                File.WriteAllText(defaultScriptPath, string.Empty);
            }
            else
            {
                autoReferencedGuids.UnionWith(assemblyDefinition.references);

                assemblyDefinition.references = autoReferencedGuids.ToArray();
            }

            File.WriteAllText(assemblyPath, JsonUtility.ToJson(assemblyDefinition, true));
        }

        private static void CreateAssetsEditorAssemblyReferences(in string definitionPath, IReadOnlyList<string> ignoredFolders)
        {
            static bool isValid(string path, IReadOnlyList<string> ignoredFolders)
            {
                for (int i = 0; i < ignoredFolders.Count; i++)
                {
                    if (path.StartsWith(ignoredFolders[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            const string fileName = AssetsEditorAssemblyDefinitionName + "." + AssemblyDefinitionReferenceExtension;
            const string definitionSearch = "*." + AssemblyDefinitionExtension;
            const string definitionReferenceSearch = "*." + AssemblyDefinitionReferenceExtension;
            string fileContent = JsonUtility.ToJson(new AssemblyDefinitionReference(string.Format(GuidFormat, AssetDatabase.AssetPathToGUID(definitionPath))), true);

            string[] searchInFolders =
            {
                "Assets"
            };

            foreach (string guid in AssetDatabase.FindAssets("t:folder Editor", searchInFolders))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (Path.GetFileName(path) == "Editor"
                 && isValid(path, ignoredFolders)
                 && Directory.GetFiles(path, definitionSearch, SearchOption.TopDirectoryOnly).Length == 0
                 && Directory.GetFiles(path, definitionReferenceSearch, SearchOption.TopDirectoryOnly).Length == 0
                 && Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Length > 0)
                {
                    File.WriteAllText($"{path}/{fileName}", fileContent);
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceSynchronousImport);
        }
    }
}
