using System;

namespace Coimbra.Roslyn
{
    public readonly struct NamespaceScope : IDisposable
    {
        private readonly BracesScope? _bracesScope;

        public NamespaceScope(SourceBuilder sourceBuilder, string namespaceValue)
        {
            if (string.IsNullOrWhiteSpace(namespaceValue))
            {
                _bracesScope = null;
            }
            else
            {
                sourceBuilder.AddLine($"namespace {namespaceValue}");
                _bracesScope = new BracesScope(sourceBuilder);
            }
        }

        public void Dispose()
        {
            _bracesScope?.Dispose();
        }
    }
}
