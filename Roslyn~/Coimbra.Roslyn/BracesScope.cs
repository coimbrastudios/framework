using System;

namespace Coimbra.Roslyn
{
    public readonly struct BracesScope : IDisposable
    {
        private readonly string _end;

        private readonly SourceBuilder _sourceBuilder;

        public BracesScope(SourceBuilder sourceBuilder, string end = "")
        {
            _end = end;
            _sourceBuilder = sourceBuilder;
            _sourceBuilder.AddLine("{");
            _sourceBuilder.Indent.Amount++;
        }

        public void Dispose()
        {
            _sourceBuilder.Indent.Amount--;
            _sourceBuilder.AddLine($"}}{_end}");
        }
    }
}
