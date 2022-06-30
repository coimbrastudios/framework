using Coimbra.Roslyn;
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
        public void Execute(GeneratorExecutionContext context)
        {
            SourceBuilder sourceBuilder = new();
            EventSyntaxReceiver syntaxReceiver = (EventSyntaxReceiver)context.SyntaxContextReceiver;

            foreach (TypeDeclarationSyntax typeDeclarationSyntax in syntaxReceiver!.Types)
            {
                sourceBuilder.Initialize();
                sourceBuilder.AddLine("#nullable enable");
                sourceBuilder.SkipLine();

                sourceBuilder.AddUsing("Coimbra");
                sourceBuilder.AddUsing("Coimbra.Services");
                sourceBuilder.AddUsing("Coimbra.Services.Events");
                sourceBuilder.AddUsing("System.CodeDom.Compiler");
                sourceBuilder.AddUsing("System.Collections.Generic");
                sourceBuilder.AddUsing("System.Runtime.CompilerServices");
                sourceBuilder.SkipLine();

                using (new NamespaceScope(sourceBuilder, typeDeclarationSyntax.GetNamespace()))
                {
                    string typeName = typeDeclarationSyntax.TypeParameterList != null ? $"{typeDeclarationSyntax.GetTypeName()}{typeDeclarationSyntax.TypeParameterList}" : typeDeclarationSyntax.GetTypeName();
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
                        sourceBuilder.AddLine($"public static EventHandle AddListener(in EventContextHandler<{typeName}> eventHandler)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return ServiceLocator.GetChecked<IEventService>().AddListener<{typeName}>(in eventHandler);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "AddRelevancyListener");
                        sourceBuilder.AddLine("public static void AddRelevancyListener(in EventRelevancyChangedHandler relevancyChangedHandler)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"ServiceLocator.GetChecked<IEventService>().AddRelevancyListener<{typeName}>(in relevancyChangedHandler);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "GetListenerCount");
                        sourceBuilder.AddLine("public static int GetListenerCount()");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return ServiceLocator.GetChecked<IEventService>().GetListenerCount<{typeName}>();");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "GetListeners");
                        sourceBuilder.AddLine("public static int GetListeners(List<DelegateListener> listeners)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return ServiceLocator.GetChecked<IEventService>().GetListeners<{typeName}>(listeners);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "GetRelevancyListeners");
                        sourceBuilder.AddLine("public static int GetRelevancyListeners(List<DelegateListener> listeners)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return ServiceLocator.GetChecked<IEventService>().GetRelevancyListeners<{typeName}>(listeners);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "IsInvoking");
                        sourceBuilder.AddLine("public static bool IsInvoking()");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return ServiceLocator.GetChecked<IEventService>().IsInvoking<{typeName}>();");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "RemoveRelevancyListener");
                        sourceBuilder.AddLine("public static void RemoveRelevancyListener(in EventRelevancyChangedHandler relevancyChangedHandler)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"ServiceLocator.GetChecked<IEventService>().RemoveRelevancyListener<{typeName}>(in relevancyChangedHandler);");
                        }

                        sourceBuilder.SkipLine();
                        AddMethodBoilerplate(sourceBuilder, "RemoveAllListeners");
                        sourceBuilder.AddLine("internal static bool RemoveAllListeners()");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return ServiceLocator.GetChecked<IEventService>().RemoveAllListeners<{typeName}>();");
                        }
                    }

                    sourceBuilder.SkipLine();
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// Generated utility methods for <see cref=\"{typeName}\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraServicesEventsTypes.Namespace}.Roslyn.{nameof(EventMethodsGenerator)}\", \"1.0.0.0\")]");
                    sourceBuilder.AddLine($"internal static class Generated{typeName}Utility");

                    using (new BracesScope(sourceBuilder))
                    {
                        AddMethodBoilerplate(sourceBuilder, "Invoke");
                        sourceBuilder.AddLine($"internal static bool Invoke({(isStruct ? "in this" : "this")} {typeName} e, object sender)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return ServiceLocator.GetChecked<IEventService>().Invoke<{typeName}>(sender, in e);");
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
            sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{originalMethod}{{T}}\"/>");
            sourceBuilder.AddLine("/// </summary>");
            sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraServicesEventsTypes.Namespace}.Roslyn.{nameof(EventMethodsGenerator)}\", \"1.0.0.0\")]");
            sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        }
    }
}
