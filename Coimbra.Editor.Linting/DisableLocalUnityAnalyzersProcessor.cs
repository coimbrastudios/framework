using System.Text.RegularExpressions;
using UnityEditor;

namespace Coimbra.Editor.Linting
{
    /// <a href="https://github.com/microsoft/Microsoft.Unity.Analyzers#handling-duplicate-diagnostics">Copyright (c) Microsoft Corporation.</a>
    internal sealed class DisableLocalUnityAnalyzersProcessor : AssetPostprocessor
    {
        private static readonly Regex Regex = new("(\\<Analyzer)\\s+(Include=\".*Microsoft\\.Unity\\.Analyzers\\.dll\")");

        private static string OnGeneratedCSProject(string path, string content)
        {
            if (ScriptableSettings.TryGetOrFind(out LintingSettings settings) && settings.DisableLocalUnityAnalyzers)
            {
                return Regex.Replace(content, "$1 Condition=\"false\" $2");
            }

            return content;
        }
    }
}
