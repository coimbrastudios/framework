using System.Collections.Generic;
using System.Text;

namespace Coimbra.Roslyn
{
    public class SourceBuilder
    {
        public readonly SourceIndent Indent = new();

        private readonly StringBuilder _stringBuilder = new();

        private readonly HashSet<string> _usingSet = new();

        public void Initialize()
        {
            Indent.Amount = 0;
            _stringBuilder.Clear();
            _stringBuilder.AppendLine("// This file is auto-generated!");
            _stringBuilder.AppendLine();
            _usingSet.Clear();
        }

        public void AddLine(string lineContent, bool skipIndent = false)
        {
            _stringBuilder.AppendLine(skipIndent ? $"{lineContent}" : $"{Indent}{lineContent}");
        }

        public void AddUsing(string value)
        {
            if (_usingSet.Add(value))
            {
                AddLine($"using {value};");
            }
        }

        public LineScope BeginLine(bool skipIndent = false)
        {
            if (!skipIndent)
            {
                _stringBuilder.Append(Indent);
            }

            return new LineScope(_stringBuilder);
        }

        public UsingScope BeginUsing()
        {
            return new UsingScope(_stringBuilder, _usingSet);
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
