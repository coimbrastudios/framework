using System.Text;

namespace Coimbra.SourceGenerators
{
    public class SourceBuilder
    {
        public readonly SourceIndent Indent;

        private readonly StringBuilder _stringBuilder;

        public SourceBuilder()
        {
            Indent = new SourceIndent();
            _stringBuilder = new StringBuilder();
        }

        public void Initialize(params string[] usings)
        {
            Indent.Amount = 0;
            _stringBuilder.Clear();
            _stringBuilder.AppendLine("// This file is auto-generated!");
            _stringBuilder.AppendLine();

            if (usings == null || usings.Length == 0)
            {
                return;
            }

            foreach (string s in usings)
            {
                _stringBuilder.AppendLine($"using {s};");
            }

            _stringBuilder.AppendLine();
        }

        public void AddLine(string lineContent)
        {
            _stringBuilder.AppendLine($"{Indent}{lineContent}");
        }

        public void SkipLine()
        {
            _stringBuilder.AppendLine();
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
