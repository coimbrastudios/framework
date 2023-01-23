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
        private static readonly string AggressiveInlineAttribute = $"[{SystemTypes.MethodImplAttribute.FullName}({SystemTypes.MethodImplOptionsEnum.FullName}.AggressiveInlining)]";

        private static readonly string GeneratedCodeAttribute = $"[{SystemTypes.GeneratedCodeAttribute.FullName}(\"{CoimbraTypes.Namespace}.Roslyn.{nameof(SharedManagedPoolGenerator)}\", \"1.0.0.0\")]";

        private static readonly string NotNullAttribute = $"[{JetBrainsTypes.NotNullAttribute.FullName}]";

        private static readonly SymbolDisplayFormat QualifiedNameOnlyFormat = new(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, SymbolDisplayGenericsOptions.IncludeTypeParameters);

        public void Execute(GeneratorExecutionContext context)
        {
            SourceBuilder sourceBuilder = new();

            foreach ((SemanticModel semanticModel, TypeDeclarationSyntax typeDeclaration, AttributeData attributeData) in EnumerateTypes(context))
            {
                string valueField = attributeData.ConstructorArguments[0].Value as string;
                string instanceArguments = string.Empty;
                string instanceConstraints = string.Empty;
                GenericNameSyntax valueTypeSyntax = null;

                if (attributeData.ConstructorArguments.Length > 1 && !attributeData.ConstructorArguments[1].IsNull)
                {
                    string instanceName = attributeData.ConstructorArguments[1].Value as string;

                    foreach (SyntaxNode childNode in typeDeclaration.ChildNodes())
                    {
                        if (childNode is not TypeDeclarationSyntax nestedTypeDeclaration
                         || nestedTypeDeclaration.GetTypeName() != instanceName)
                        {
                            continue;
                        }

                        instanceArguments = nestedTypeDeclaration.TypeParameterList?.ToString() ?? string.Empty;
                        instanceConstraints = nestedTypeDeclaration.ConstraintClauses.ToString();
                        valueTypeSyntax = GetValueTypeSyntax(nestedTypeDeclaration);
                        valueField = $"{instanceName}{instanceArguments}.{valueField}";

                        break;
                    }
                }
                else
                {
                    valueTypeSyntax = GetValueTypeSyntax(typeDeclaration);
                }

                if (valueTypeSyntax == null)
                {
                    continue;
                }

                TypeInfo valueTypeInfo = semanticModel.GetTypeInfo(valueTypeSyntax.TypeArgumentList.Arguments[0]);
                ITypeSymbol valueTypeSymbol = valueTypeInfo.Type ?? valueTypeInfo.ConvertedType;
                string valueType = valueTypeSymbol?.ToDisplayString(QualifiedNameOnlyFormat) ?? valueTypeSyntax.TypeArgumentList.Arguments.ToString();
                sourceBuilder.Initialize();

                using (new NamespaceScope(sourceBuilder, typeDeclaration.GetNamespace()))
                {
                    string type = typeDeclaration is ClassDeclarationSyntax ? "class" : "struct";
                    sourceBuilder.AddLine($"{typeDeclaration.Modifiers.ToString()} {type} {typeDeclaration.GetTypeName()}");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"/// <inheritdoc cref=\"{CoimbraTypes.ManagedPoolClass.FullName}{{T}}.AvailableCount\"/>");
                        sourceBuilder.AddLine(GeneratedCodeAttribute);
                        sourceBuilder.AddLine(AggressiveInlineAttribute);
                        sourceBuilder.AddLine($"public static int GetAvailableCount{instanceArguments}()");

                        if (!string.IsNullOrWhiteSpace(instanceConstraints))
                        {
                            sourceBuilder.AddLine($"    {instanceConstraints}");
                        }

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {valueField}.AvailableCount;");
                        }

                        sourceBuilder.SkipLine();
                        sourceBuilder.AddLine($"/// <inheritdoc cref=\"{CoimbraTypes.ManagedPoolClass.FullName}{{T}}.MaxCapacity\"/>");
                        sourceBuilder.AddLine(GeneratedCodeAttribute);
                        sourceBuilder.AddLine(AggressiveInlineAttribute);
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
                        sourceBuilder.AddLine($"/// <inheritdoc cref=\"{CoimbraTypes.ManagedPoolClass.FullName}{{T}}.PreloadCount\"/>");
                        sourceBuilder.AddLine(GeneratedCodeAttribute);
                        sourceBuilder.AddLine(AggressiveInlineAttribute);
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
                        sourceBuilder.AddLine($"/// <inheritdoc cref=\"{CoimbraTypes.ManagedPoolClass.FullName}{{T}}.Initialize\"/>");
                        sourceBuilder.AddLine(GeneratedCodeAttribute);
                        sourceBuilder.AddLine(AggressiveInlineAttribute);
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
                        sourceBuilder.AddLine($"/// <inheritdoc cref=\"{CoimbraTypes.ManagedPoolClass.FullName}{{T}}.Pop()\"/>");
                        sourceBuilder.AddLine(GeneratedCodeAttribute);
                        sourceBuilder.AddLine(AggressiveInlineAttribute);
                        sourceBuilder.AddLine(NotNullAttribute);
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
                        sourceBuilder.AddLine($"/// <inheritdoc cref=\"{CoimbraTypes.ManagedPoolClass.FullName}{{T}}.Pop()\"/>");
                        sourceBuilder.AddLine(GeneratedCodeAttribute);
                        sourceBuilder.AddLine(AggressiveInlineAttribute);
                        sourceBuilder.AddLine($"public static {CoimbraTypes.ManagedPoolClass.FullName}<{valueType}>.Instance Pop{instanceArguments}({NotNullAttribute} out {valueType} instance)");

                        if (!string.IsNullOrWhiteSpace(instanceConstraints))
                        {
                            sourceBuilder.AddLine($"    {instanceConstraints}");
                        }

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {valueField}.Pop(out instance);");
                        }

                        sourceBuilder.SkipLine();
                        sourceBuilder.AddLine($"/// <inheritdoc cref=\"{CoimbraTypes.ManagedPoolClass.FullName}{{T}}.Push\"/>");
                        sourceBuilder.AddLine(GeneratedCodeAttribute);
                        sourceBuilder.AddLine(AggressiveInlineAttribute);
                        sourceBuilder.AddLine($"public static void Push{instanceArguments}({NotNullAttribute} in {valueType} instance)");

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

                context.AddSource(typeDeclaration.GetTypeName(), SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SharedManagedPoolSyntaxReceiver());
        }

        private static IEnumerable<(SemanticModel, TypeDeclarationSyntax, AttributeData)> EnumerateTypes(GeneratorExecutionContext context)
        {
            SharedManagedPoolSyntaxReceiver syntaxReceiver = (SharedManagedPoolSyntaxReceiver)context.SyntaxReceiver;

            foreach (TypeDeclarationSyntax syntaxNode in syntaxReceiver!.Types)
            {
                SemanticModel semanticModel = context.Compilation.GetSemanticModel(syntaxNode.SyntaxTree);

                if (semanticModel.GetDeclaredSymbol(syntaxNode) is { } typeSymbol
                 && typeSymbol.HasAttribute(CoimbraTypes.SharedManagedPoolAttribute, out AttributeData attributeData, false))
                {
                    yield return (semanticModel, syntaxNode, attributeData);
                }
            }
        }

        private static GenericNameSyntax GetValueTypeSyntax(SyntaxNode typeDeclaration)
        {
            foreach (SyntaxNode nestedClassDeclarationSyntaxChild in typeDeclaration.ChildNodes())
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
