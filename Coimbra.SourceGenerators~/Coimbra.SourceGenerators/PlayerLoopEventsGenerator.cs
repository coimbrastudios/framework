using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace Coimbra.SourceGenerators
{
    [Generator]
    public sealed class PlayerLoopEventsGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            StringBuilder sourceBuilder = new StringBuilder(@"// This file is auto-generated!

using UnityEngine.Scripting;

namespace Coimbra
{");

            foreach (string value in Enum.GetNames(typeof(PlayerLoopTiming)))
            {
                sourceBuilder.Append($@"
    [Preserve]
    public readonly struct {value}Event
    {{
        public readonly float DeltaTime;

        public {value}Event(float deltaTime)
        {{
            DeltaTime = deltaTime;
        }}
    }}
");
            }

            sourceBuilder.Append(@"}
");

            context.AddSource("PlayerLoopEvents.generated", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context) { }
    }
}
