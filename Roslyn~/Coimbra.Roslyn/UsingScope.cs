using System;
using System.Collections.Generic;
using System.Text;

namespace Coimbra.Roslyn
{
    public readonly struct UsingScope : IDisposable
    {
        private readonly StringBuilder _stringBuilder;

        private readonly HashSet<string> _set;

        private readonly List<string> _list;

        public UsingScope(StringBuilder stringBuilder, HashSet<string> set)
        {
            _stringBuilder = stringBuilder;
            _set = set;
            _list = new List<string>();
        }

        public void AddContent(string content)
        {
            if (_set.Add(content))
            {
                _list.Add(content);
            }
        }

        public void Dispose()
        {
            _list.Sort(StringComparer.InvariantCulture);

            foreach (string value in _list)
            {
                _stringBuilder.AppendLine($"using {value};");
            }
        }
    }
}
