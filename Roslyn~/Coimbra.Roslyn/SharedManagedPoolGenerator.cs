using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace Coimbra.Roslyn
{
    [Generator]
    public sealed class SharedManagedPoolGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            SourceBuilder sourceBuilder = new();

            foreach ((SemanticModel semanticModel, ClassDeclarationSyntax classDeclarationSyntax, AttributeData attributeData) in EnumerateTypes(context))
            {
#if DEBUG
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Launch();
                }
#endif
                string instanceArguments = string.Empty;
                string instanceConstraints = string.Empty;
                string valueField = attributeData.ConstructorArguments[0].Value as string;
                string valueType = string.Empty;
                GenericNameSyntax valueTypeSyntax = null;

                if (attributeData.ConstructorArguments.Length > 1 && !attributeData.ConstructorArguments[1].IsNull)
                {
                    string instanceName = attributeData.ConstructorArguments[1].Value as string;

                    foreach (SyntaxNode classDeclarationSyntaxChild in classDeclarationSyntax.ChildNodes())
                    {
                        if (classDeclarationSyntaxChild is not ClassDeclarationSyntax nestedClassDeclarationSyntax
                         || !nestedClassDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword)
                         || nestedClassDeclarationSyntax.GetTypeName() != instanceName)
                        {
                            continue;
                        }

                        instanceArguments = nestedClassDeclarationSyntax.TypeParameterList?.ToString() ?? string.Empty;
                        instanceConstraints = nestedClassDeclarationSyntax.ConstraintClauses.ToString();
                        valueTypeSyntax = GetValueTypeSyntax(nestedClassDeclarationSyntax);
                        valueField = $"{instanceName}{instanceArguments}.{valueField}";

                        break;
                    }
                }
                else
                {
                    valueTypeSyntax = GetValueTypeSyntax(classDeclarationSyntax);
                }

                sourceBuilder.Initialize();
                sourceBuilder.AddUsing("Coimbra");
                sourceBuilder.AddUsing("JetBrains.Annotations");
                sourceBuilder.AddUsing("System.Runtime.CompilerServices");

                if (valueTypeSyntax != null)
                {
                    TypeInfo valueTypeInfo = semanticModel.GetTypeInfo(valueTypeSyntax.TypeArgumentList.Arguments[0]);
                    ITypeSymbol valueTypeSymbol = valueTypeInfo.Type ?? valueTypeInfo.ConvertedType;
                    sourceBuilder.AddUsing(valueTypeSymbol!.ContainingNamespace.ToString());
                    valueType = valueTypeSyntax.TypeArgumentList.Arguments.ToString();
                }

                sourceBuilder.SkipLine();

                using (new NamespaceScope(sourceBuilder, classDeclarationSyntax.GetNamespace()))
                {
                    string access = classDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword) ? "public" : "internal";
                    sourceBuilder.AddLine($"{access} static partial class {classDeclarationSyntax.GetTypeName()}");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine("/// <inheritdoc cref=\"ManagedPool{T}.MaxCapacity\"/>");
                        sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                        sourceBuilder.AddLine($"public static int GetMaxCapacity{instanceArguments}()");

                        if (!string.IsNullOrWhiteSpace(instanceConstraints))
                        {
                            sourceBuilder.AddLine($"    {instanceConstraints}");
                        }

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {valueField}.MaxCapacity;");
                        }

                        sourceBuilder.SkipLine();
                        sourceBuilder.AddLine("/// <inheritdoc cref=\"ManagedPool{T}.PreloadCount\"/>");
                        sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                        sourceBuilder.AddLine($"public static int GetPreloadCount{instanceArguments}()");

                        if (!string.IsNullOrWhiteSpace(instanceConstraints))
                        {
                            sourceBuilder.AddLine($"    {instanceConstraints}");
                        }

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {valueField}.PreloadCount;");
                        }

                        sourceBuilder.SkipLine();
                        sourceBuilder.AddLine("/// <inheritdoc cref=\"ManagedPool{T}.Initialize\"/>");
                        sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                        sourceBuilder.AddLine($"public static void Initialize{instanceArguments}(int? preloadCount = null, int? maxCapacity = null)");

                        if (!string.IsNullOrWhiteSpace(instanceConstraints))
                        {
                            sourceBuilder.AddLine($"    {instanceConstraints}");
                        }

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"{valueField}.Initialize(preloadCount, maxCapacity);");
                        }

                        sourceBuilder.SkipLine();
                        sourceBuilder.AddLine("/// <inheritdoc cref=\"ManagedPool{T}.Pop()\"/>");
                        sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                        sourceBuilder.AddLine("[NotNull]");
                        sourceBuilder.AddLine($"public static {valueType} Pop{instanceArguments}()");

                        if (!string.IsNullOrWhiteSpace(instanceConstraints))
                        {
                            sourceBuilder.AddLine($"    {instanceConstraints}");
                        }

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {valueField}.Pop();");
                        }

                        sourceBuilder.SkipLine();
                        sourceBuilder.AddLine("/// <inheritdoc cref=\"ManagedPool{T}.Pop()\"/>");
                        sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                        sourceBuilder.AddLine($"public static ManagedPool<{valueType}>.Instance Pop{instanceArguments}([NotNull] out {valueType} instance)");

                        if (!string.IsNullOrWhiteSpace(instanceConstraints))
                        {
                            sourceBuilder.AddLine($"    {instanceConstraints}");
                        }

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {valueField}.Pop(out instance);");
                        }

                        sourceBuilder.SkipLine();
                        sourceBuilder.AddLine("/// <inheritdoc cref=\"ManagedPool{T}.Push\"/>");
                        sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                        sourceBuilder.AddLine($"public static void Push{instanceArguments}([NotNull] in {valueType} instance)");

                        if (!string.IsNullOrWhiteSpace(instanceConstraints))
                        {
                            sourceBuilder.AddLine($"    {instanceConstraints}");
                        }

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"{valueField}.Push(in instance);");
                        }
                    }
                }

                context.AddSource(classDeclarationSyntax.GetTypeName(), SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SharedManagedPoolSyntaxReceiver());
        }

        private static IEnumerable<(SemanticModel, ClassDeclarationSyntax, AttributeData)> EnumerateTypes(GeneratorExecutionContext context)
        {
            SharedManagedPoolSyntaxReceiver syntaxReceiver = (SharedManagedPoolSyntaxReceiver)context.SyntaxReceiver;

            foreach (ClassDeclarationSyntax syntaxNode in syntaxReceiver!.Types)
            {
                SemanticModel semanticModel = context.Compilation.GetSemanticModel(syntaxNode.SyntaxTree);

                if (semanticModel.GetDeclaredSymbol(syntaxNode) is { } typeSymbol
                 && typeSymbol.HasAttribute(CoimbraTypes.SharedManagedPoolAttribute, out AttributeData attributeData))
                {
                    yield return (semanticModel, syntaxNode, attributeData);
                }
            }
        }

        private static GenericNameSyntax GetValueTypeSyntax(SyntaxNode classDeclarationSyntax)
        {
            foreach (SyntaxNode nestedClassDeclarationSyntaxChild in classDeclarationSyntax.ChildNodes())
            {
                if (nestedClassDeclarationSyntaxChild is FieldDeclarationSyntax fieldDeclarationSyntax
                 && fieldDeclarationSyntax.Declaration.Type is GenericNameSyntax genericNameSyntax
                 && genericNameSyntax.TypeArgumentList.Arguments.Count == 1
                 && genericNameSyntax.Identifier.ToString() == CoimbraTypes.ManagedPoolClass.Name)
                {
                    return genericNameSyntax;
                }
            }

            return null;
        }
    }
}
