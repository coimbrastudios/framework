using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
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

            foreach ((ClassDeclarationSyntax ClassDeclaration, ITypeSymbol ClassType, bool IgnoreProtected) type in EnumerateTypes(context))
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
                        bool isFirst = true;

                        foreach (IMethodSymbol constructor in constructors)
                        {
                            if (isFirst)
                            {
                                isFirst = false;
                            }
                            else
                            {
                                sourceBuilder.SkipLine();
                            }

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
                                lineScope.AddContent($"{access} {type.ClassDeclaration.GetTypeName()}(");

                                if (constructor.Parameters.Length == 0)
                                {
                                    lineScope.AddContent(") { }");
                                }
                                else
                                {
                                    lineScope.AddContent($"{parameters[0].Type} {parameters[0].Name}");

                                    for (int i = 1; i < parameters.Length; i++)
                                    {
                                        lineScope.AddContent($", {parameters[i].Type} {parameters[i].Name}");
                                    }

                                    lineScope.AddContent(")");
                                }
                            }

                            if (constructor.Parameters.Length <= 0)
                            {
                                continue;
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
                    }
                }

                context.AddSource(type.ClassDeclaration.GetTypeName(), SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new CopyBaseConstructorsSyntaxReceiver());
        }

        private static IEnumerable<(ClassDeclarationSyntax ClassDeclaration, ITypeSymbol ClassType, bool IgnoreProtected)> EnumerateTypes(GeneratorExecutionContext context)
        {
            CopyBaseConstructorsSyntaxReceiver syntaxReceiver = (CopyBaseConstructorsSyntaxReceiver)context.SyntaxReceiver;

            foreach (ClassDeclarationSyntax classDeclaration in syntaxReceiver!.Types)
            {
                SemanticModel semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);

                if (semanticModel.GetDeclaredSymbol(classDeclaration) is not ITypeSymbol { BaseType: not null } classType
                 || classType.BaseType.InstanceConstructors.Length == 0
                 || !classType.HasAttribute(CoimbraTypes.CopyBaseConstructorsAttribute, out AttributeData attribute, false))
                {
                    continue;
                }

                bool ignoreProtected = attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is bool and true;

                yield return (classDeclaration, classType, ignoreProtected);
            }
        }
    }
}
