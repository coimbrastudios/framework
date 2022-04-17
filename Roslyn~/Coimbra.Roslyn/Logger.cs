using System;
using System.IO;

namespace Coimbra.Roslyn
{
    public static class Logger
    {
        private static string ProjectPath => Environment.CurrentDirectory;

        public static void Write(string message)
        {
            try
            {
                using StreamWriter streamWriter = File.AppendText(GetTempGeneratedPathToFile("Coimbra.Roslyn.log"));
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
    }
}
