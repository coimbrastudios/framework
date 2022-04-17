using System;
using System.Text;

namespace Coimbra.Roslyn
{
    public readonly struct LineScope : IDisposable
    {
        private readonly StringBuilder _stringBuilder;

        public LineScope(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }

        public void AddContent(string content)
        {
            _stringBuilder.Append(content);
        }

        public void Dispose()
        {
            _stringBuilder.AppendLine();
        }
    }
}
