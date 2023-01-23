using System.Text;

namespace Coimbra.Roslyn
{
    public class SourceBuilder
    {
        public readonly SourceIndent Indent = new();

        private readonly StringBuilder _stringBuilder = new();

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
