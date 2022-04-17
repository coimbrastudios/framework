#pragma warning disable 0161
using System;

namespace Coimbra.Roslyn
{
    public readonly struct PragmaWarningDisableScope : IDisposable
    {
        private readonly SourceBuilder _sourceBuilder;

        private readonly string _value;

        public PragmaWarningDisableScope(SourceBuilder sourceBuilder, string value)
        {
            _value = value;
            _sourceBuilder = sourceBuilder;

            if (string.IsNullOrWhiteSpace(_value))
            {
                return;
            }

            _sourceBuilder.AddLine($"#pragma warning disable {_value}", true);
            _sourceBuilder.SkipLine();
        }

        public void Dispose()
        {
            if (string.IsNullOrWhiteSpace(_value))
            {
                return;
            }

            _sourceBuilder.SkipLine();
            _sourceBuilder.AddLine($"#pragma warning restore {_value}", true);
        }
    }
}
