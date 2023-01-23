using Coimbra.Roslyn;
using Coimbra.Services.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Coimbra.Services.Events.Roslyn
{
    [Generator]
    public sealed class EventMethodsGenerator : ISourceGenerator
    {
        private static readonly SymbolDisplayFormat QualifiedNameOnlyFormat = new(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, SymbolDisplayGenericsOptions.IncludeTypeParameters);

        public void Execute(GeneratorExecutionContext context)
        {
            SourceBuilder sourceBuilder = new();
            EventSyntaxReceiver syntaxReceiver = (EventSyntaxReceiver)context.SyntaxContextReceiver;

            foreach (TypeDeclarationSyntax typeDeclarationSyntax in syntaxReceiver!.Types)
            {
                sourceBuilder.Initialize();
                sourceBuilder.AddLine("#nullable enable");
                sourceBuilder.SkipLine();

                using (new NamespaceScope(sourceBuilder, typeDeclarationSyntax.GetNamespace()))
                {
                    SemanticModel semanticModel = context.Compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree);
                    TypeInfo typeInfo = semanticModel.GetTypeInfo(typeDeclarationSyntax);
                    ITypeSymbol typeSymbol = typeInfo.Type ?? typeInfo.ConvertedType;
                    string typeName = typeSymbol?.ToDisplayString(QualifiedNameOnlyFormat) ?? (typeDeclarationSyntax.TypeParameterList != null ? $"{typeDeclarationSyntax.GetTypeName()}{typeDeclarationSyntax.TypeParameterList}" : typeDeclarationSyntax.GetTypeName());
                    bool isStruct = false;

                    using (LineScope lineScope = sourceBuilder.BeginLine())
                    {
                        lineScope.AddContent(typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword) ? "public" : "internal");

                        switch (typeDeclarationSyntax)
                        {
                            case ClassDeclarationSyntax _:
                            {
                                if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.SealedKeyword))
                                {
                                    lineScope.AddContent(" sealed");
                                }

                                lineScope.AddContent(" partial class ");

                                break;
                            }

                            case StructDeclarationSyntax _:
                            {
                                isStruct = true;

                                if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                                {
                                    lineScope.AddContent(" readonly");
                                }

                                lineScope.AddContent(" partial struct ");

                                break;
                            }

                            default:
                            {
                                continue;
                            }
                        }

                        lineScope.AddContent(typeName);
                    }

                    using (new BracesScope(sourceBuilder))
                    {
                        AddMethodBoilerplate(sourceBuilder, "AddListener");
                        sourceBuilder.AddLine($"public static {CoimbraServicesEventsTypes.EventHandleStruct.FullName} AddListener(in {CoimbraServicesEventsTypes.EventContextHandlerStruct.FullName}<{typeName}> eventHandler)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{CoimbraServicesEventsTypes.EventServiceInterface.FullName}>().AddListener<{typeName}>(in eventHandler);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "AddRelevancyListener");
                        sourceBuilder.AddLine($"public static void AddRelevancyListener(in {CoimbraServicesEventsTypes.EventRelevancyChangedHandlerDelegate.FullName} relevancyChangedHandler)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"{CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{CoimbraServicesEventsTypes.EventServiceInterface.FullName}>().AddRelevancyListener<{typeName}>(in relevancyChangedHandler);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "GetListenerCount");
                        sourceBuilder.AddLine("public static int GetListenerCount()");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{CoimbraServicesEventsTypes.EventServiceInterface.FullName}>().GetListenerCount<{typeName}>();");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "GetListeners");
                        sourceBuilder.AddLine($"public static int GetListeners({SystemTypes.ListClass.FullName}<{CoimbraTypes.DelegateListenerStruct.FullName}> listeners)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{CoimbraServicesEventsTypes.EventServiceInterface.FullName}>().GetListeners<{typeName}>(listeners);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "GetRelevancyListeners");
                        sourceBuilder.AddLine($"public static int GetRelevancyListeners({SystemTypes.ListClass.FullName}<{CoimbraTypes.DelegateListenerStruct.FullName}> listeners)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{CoimbraServicesEventsTypes.EventServiceInterface.FullName}>().GetRelevancyListeners<{typeName}>(listeners);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "IsInvoking");
                        sourceBuilder.AddLine("public static bool IsInvoking()");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{CoimbraServicesEventsTypes.EventServiceInterface.FullName}>().IsInvoking<{typeName}>();");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "RemoveRelevancyListener");
                        sourceBuilder.AddLine($"public static void RemoveRelevancyListener(in {CoimbraServicesEventsTypes.EventRelevancyChangedHandlerDelegate.FullName} relevancyChangedHandler)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"{CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{CoimbraServicesEventsTypes.EventServiceInterface.FullName}>().RemoveRelevancyListener<{typeName}>(in relevancyChangedHandler);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "RemoveAllListeners");
                        sourceBuilder.AddLine("internal static bool RemoveAllListeners()");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{CoimbraServicesEventsTypes.EventServiceInterface.FullName}>().RemoveAllListeners<{typeName}>();");
                        }
                    }

                    sourceBuilder.SkipLine();
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// Generated utility methods for <see cref=\"{typeName}\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine($"[{SystemTypes.GeneratedCodeAttribute.FullName}(\"{CoimbraServicesEventsTypes.Namespace}.Roslyn.{nameof(EventMethodsGenerator)}\", \"1.0.0.0\")]");
                    sourceBuilder.AddLine($"internal static class Generated{typeName}Utility");

                    using (new BracesScope(sourceBuilder))
                    {
                        AddMethodBoilerplate(sourceBuilder, "Invoke");
                        sourceBuilder.AddLine($"internal static bool Invoke({(isStruct ? "in this" : "this")} {typeName} e, object sender)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{CoimbraServicesEventsTypes.EventServiceInterface.FullName}>().Invoke<{typeName}>(sender, in e);");
                        }
                    }
                }

                context.AddSource(typeDeclarationSyntax.GetTypeName(), SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new EventSyntaxReceiver());
        }

        private static void AddMethodBoilerplate(SourceBuilder sourceBuilder, string originalMethod)
        {
            sourceBuilder.AddLine("/// <summary>");
            sourceBuilder.AddLine($"/// <inheritdoc cref=\"{CoimbraServicesEventsTypes.EventServiceInterface.FullName}.{originalMethod}{{T}}\"/>");
            sourceBuilder.AddLine("/// </summary>");
            sourceBuilder.AddLine($"[{SystemTypes.GeneratedCodeAttribute.FullName}(\"{CoimbraServicesEventsTypes.Namespace}.Roslyn.{nameof(EventMethodsGenerator)}\", \"1.0.0.0\")]");
            sourceBuilder.AddLine($"[{SystemTypes.MethodImplAttribute.FullName}({SystemTypes.MethodImplOptionsEnum.FullName}.AggressiveInlining)]");
        }
    }
}
