using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Coimbra.Roslyn
{
    [Generator]
    public sealed class CopyBaseConstructorsGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            SourceBuilder sourceBuilder = new();
            CopyBaseConstructorsSyntaxReceiver syntaxReceiver = (CopyBaseConstructorsSyntaxReceiver)context.SyntaxContextReceiver;

            foreach (CopyBaseConstructorsTypeInfo type in syntaxReceiver!.Types)
            {
                ImmutableArray<IMethodSymbol> constructors;

                if (type.IgnoreProtected)
                {
                    constructors = type.ClassType.BaseType!.InstanceConstructors.Where(x => x.DeclaredAccessibility is Accessibility.Public).ToImmutableArray();
                }
                else
                {
                    constructors = type.ClassType.BaseType!.InstanceConstructors.Where(x => x.DeclaredAccessibility is Accessibility.Public or Accessibility.Protected).ToImmutableArray();
                }

                sourceBuilder.Initialize();
                sourceBuilder.AddUsing("System.CodeDom.Compiler");
                sourceBuilder.SkipLine();

                using (new NamespaceScope(sourceBuilder, type.ClassDeclaration.GetNamespace()))
                {
                    string types = type.ClassDeclaration.TypeParameterList?.ToString() ?? string.Empty;
                    sourceBuilder.AddLine($"{type.ClassDeclaration.Modifiers.ToString()} class {type.ClassDeclaration.GetTypeName()}{types}");

                    using (new BracesScope(sourceBuilder))
                    {
                        AddConstructor(sourceBuilder, constructors[0], type.ClassDeclaration);

                        for (int i = 1; i < constructors.Length; i++)
                        {
                            sourceBuilder.SkipLine();
                            AddConstructor(sourceBuilder, constructors[i], type.ClassDeclaration);
                        }
                    }
                }

                context.AddSource(type.ClassDeclaration.GetTypeName(), SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new CopyBaseConstructorsSyntaxReceiver());
        }

        private static void AddConstructor(SourceBuilder sourceBuilder, IMethodSymbol constructor, ClassDeclarationSyntax classDeclaration)
        {
            ImmutableArray<IParameterSymbol> parameters = constructor.Parameters;
            string comment = constructor.GetDocumentationCommentXml();

            if (!string.IsNullOrWhiteSpace(comment))
            {
                sourceBuilder.AddLine(comment);
            }

            sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraTypes.Namespace}.Roslyn.{nameof(CopyBaseConstructorsGenerator)}\", \"1.0.0.0\")]");

            using (LineScope lineScope = sourceBuilder.BeginLine())
            {
                string access = constructor.DeclaredAccessibility is Accessibility.Public ? "public" : "protected";
                lineScope.AddContent($"{access} {classDeclaration.GetTypeName()}(");

                if (constructor.Parameters.Length == 0)
                {
                    lineScope.AddContent(") { }");
                }
                else
                {
                    AddParameter(lineScope, parameters[0]);

                    for (int i = 1; i < parameters.Length; i++)
                    {
                        lineScope.AddContent(", ");
                        AddParameter(lineScope, parameters[i]);
                    }

                    lineScope.AddContent(")");
                }
            }

            if (constructor.Parameters.Length <= 0)
            {
                return;
            }

            using (LineScope lineScope = sourceBuilder.BeginLine())
            {
                lineScope.AddContent("    : base(");
                lineScope.AddContent($"{parameters[0].Name}");

                for (int i = 1; i < parameters.Length; i++)
                {
                    lineScope.AddContent($", {parameters[i].Name}");
                }

                lineScope.AddContent(") { }");
            }
        }

        private static void AddParameter(LineScope lineScope, IParameterSymbol parameter)
        {
            if (parameter.IsParams)
            {
                lineScope.AddContent("params ");
            }

            lineScope.AddContent(parameter.Type.ToString());
            lineScope.AddContent(" ");
            lineScope.AddContent(parameter.Name);
        }
    }
}
