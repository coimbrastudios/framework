using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Coimbra.SourceGenerators.Systems
{
    [Generator]
    public sealed class ApplicationSystemGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            StringBuilder sourceBuilder = new StringBuilder(@"// This file is auto-generated!

using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Coimbra.Systems
{
    public partial class ApplicationSystem
    {
        protected void Awake()
        {
            ExecutePlayerLoopEvents();
        }
    
        private async UniTask ExecutePlayerLoopEvents()
        {
            while (gameObject != null)
            {");

            const int last = (int)PlayerLoopTiming.PostTimeUpdate;

            for (int i = (int)PlayerLoopTiming.PreInitialization; i <= last; i++)
            {
                sourceBuilder.Append($@"
                await Yield({i});

                Invoke(new {(PlayerLoopTiming)i}Event(Time.deltaTime));
");
            }

            sourceBuilder.Append(@"            }
        }
    }
}
");

            context.AddSource("ApplicationSystem.generated", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context) { }
    }
}
