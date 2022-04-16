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

        public void Initialize()
        {
            Indent.Amount = 0;
            _stringBuilder.Clear();
            _stringBuilder.AppendLine("// This file is auto-generated!");
            _stringBuilder.AppendLine();
        }

        public void AddLine(string lineContent, bool skipIndent = false)
        {
            _stringBuilder.AppendLine(skipIndent ? $"{lineContent}" : $"{Indent}{lineContent}");
        }

        public void AddUsing(string value)
        {
            AddLine($"using {value};");
        }

        public LineScope BeginLine(bool skipIndent = false)
        {
            if (!skipIndent)
            {
                _stringBuilder.Append(Indent);
            }

            return new LineScope(_stringBuilder);
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
