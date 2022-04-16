using Microsoft.CodeAnalysis;
using System;
using System.IO;

namespace Coimbra.SourceGenerators
{
    public static class Logger
    {
        private static string ProjectPath => Environment.CurrentDirectory;

        public static void Log(this GeneratorExecutionContext context, string id, string title, string message, Location location = null, string description = "")
        {
            context.Log(DiagnosticSeverity.Info, id, title, message, location, description);
        }

        public static void LogError(this GeneratorExecutionContext context, string id, string title, string message, Location location = null, string description = "")
        {
            context.Log(DiagnosticSeverity.Error, id, title, message, location, description);
        }

        public static void LogWarning(this GeneratorExecutionContext context, string id, string title, string message, Location location = null, string description = "")
        {
            context.Log(DiagnosticSeverity.Warning, id, title, message, location, description);
        }

        public static void Write(string message)
        {
            try
            {
                using StreamWriter streamWriter = File.AppendText(GetTempGeneratedPathToFile("Coimbra.SourceGenerators.log"));
                streamWriter.WriteLine(message);
            }
            catch (IOException)
            {
                // ignored
            }
        }

        public static void Write(Exception e)
        {
            Write($"[{nameof(Exception)}] {e.GetType()} - {e.Message}{Environment.NewLine}{e.StackTrace}");
        }

        private static string GetTempGeneratedPathToFile(string fileNameWithExtension)
        {
            string directory = Path.Combine(ProjectPath, "Temp");
            Directory.CreateDirectory(directory);

            return Path.Combine(directory, fileNameWithExtension);
        }

        private static void Log(this GeneratorExecutionContext context, DiagnosticSeverity diagnosticSeverity, string id, string title, string message, Location location, string description = "")
        {
            DiagnosticDescriptor rule = new DiagnosticDescriptor(id, title, message, "Coimbra.SourceGenerators", diagnosticSeverity, true, description);
            context.ReportDiagnostic(Diagnostic.Create(rule, location));
            Write($"[{diagnosticSeverity}] {id} - {title}{Environment.NewLine}{message}");
        }
    }
}
