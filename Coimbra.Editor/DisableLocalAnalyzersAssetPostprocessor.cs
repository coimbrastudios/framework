using UnityEditor;
using System.Text.RegularExpressions;

namespace Coimbra.Editor
{
    /// <a href="https://github.com/microsoft/Microsoft.Unity.Analyzers#handling-duplicate-diagnostics">Copyright (c) Microsoft Corporation.</a>
    internal class DisableLocalAnalyzersPostProcessor : AssetPostprocessor
    {
        public static string OnGeneratedCSProject(string path, string content)
        {
            if (ScriptableSettings.TryGetOrFind(out CoimbraEditorProjectSettings settings) && settings.DisableLocalAnalyzers)
            {
                return Regex.Replace(content, "(\\<Analyzer)\\s+(Include=\".*Microsoft\\.Unity\\.Analyzers\\.dll\")", "$1 Condition=\"false\" $2");
            }

            return content;
        }
    }
}
