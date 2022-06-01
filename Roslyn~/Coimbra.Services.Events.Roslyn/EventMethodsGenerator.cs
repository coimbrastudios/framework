using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace Coimbra.Services.Events.Roslyn
{
    [Generator]
    public sealed class EventMethodsGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            SourceBuilder sourceBuilder = new();

            foreach (TypeDeclarationSyntax typeDeclarationSyntax in EnumerateTypes(context))
            {
                sourceBuilder.Initialize();
                sourceBuilder.AddLine("#nullable enable");
                sourceBuilder.SkipLine();

                sourceBuilder.AddUsing("Coimbra.Services");
                sourceBuilder.AddUsing("Coimbra.Services.Events");
                sourceBuilder.AddUsing("System.CodeDom.Compiler");
                sourceBuilder.AddUsing("System.Collections.Generic");
                sourceBuilder.AddUsing("System.Runtime.CompilerServices");
                sourceBuilder.SkipLine();

                using (new NamespaceScope(sourceBuilder, typeDeclarationSyntax.GetNamespace()))
                {
                    string typeName = typeDeclarationSyntax.TypeParameterList != null ? $"{typeDeclarationSyntax.GetTypeName()}{typeDeclarationSyntax.TypeParameterList}" : typeDeclarationSyntax.GetTypeName();

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
                        AddTypeMethods(sourceBuilder, typeName);
                    }

                    sourceBuilder.SkipLine();
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// Generated utility methods for <see cref=\"{typeName}\"/>.");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraServicesEventsTypes.Namespace}.Roslyn.{nameof(EventMethodsGenerator)}\", \"1.0.0.0\")]");
                    sourceBuilder.AddLine($"internal static class Generated{typeName}Utility");

                    using (new BracesScope(sourceBuilder))
                    {
                        AddUtilityMethods(sourceBuilder, typeName);
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

        private static void AddMethodBoilerplate(SourceBuilder sourceBuilder, string originalMethod, string originalParameters)
        {
            sourceBuilder.AddLine("/// <summary>");
            sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{originalMethod}{{T}}({originalParameters})\"/>");
            sourceBuilder.AddLine("/// </summary>");
            sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraServicesEventsTypes.Namespace}.Roslyn.{nameof(EventMethodsGenerator)}\", \"1.0.0.0\")]");
            sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        }

        private static void AddTypeMethods(SourceBuilder sourceBuilder, string typeName)
        {
            AddMethodBoilerplate(sourceBuilder, "AddListener");
            sourceBuilder.AddLine($"public static EventHandle AddListener(IEventService eventService, in Event<{typeName}>.Handler eventHandler)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return eventService.AddListener<{typeName}>(in eventHandler);");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "AddListener");
            sourceBuilder.AddLine($"public static EventHandle AddListener(ServiceLocator serviceLocator, in Event<{typeName}>.Handler eventHandler)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return serviceLocator.Get<IEventService>()?.AddListener<{typeName}>(in eventHandler) ?? default;");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "AddRelevancyListener");
            sourceBuilder.AddLine("public static void AddRelevancyListener(IEventService eventService, in IEventService.EventRelevancyChangedHandler relevancyChangedHandler)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"eventService.AddRelevancyListener<{typeName}>(in relevancyChangedHandler);");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "AddRelevancyListener");
            sourceBuilder.AddLine("public static void AddRelevancyListener(ServiceLocator serviceLocator, in IEventService.EventRelevancyChangedHandler relevancyChangedHandler)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"serviceLocator.Get<IEventService>()?.AddRelevancyListener<{typeName}>(in relevancyChangedHandler);");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "GetListenerCount");
            sourceBuilder.AddLine("public static int GetListenerCount(IEventService eventService)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return eventService.GetListenerCount<{typeName}>();");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "GetListenerCount");
            sourceBuilder.AddLine("public static int GetListenerCount(ServiceLocator serviceLocator)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return serviceLocator.Get<IEventService>()?.GetListenerCount<{typeName}>() ?? 0;");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "IsInvoking");
            sourceBuilder.AddLine("public static bool IsInvoking(IEventService eventService)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return eventService.IsInvoking<{typeName}>();");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "IsInvoking");
            sourceBuilder.AddLine("public static bool IsInvoking(ServiceLocator serviceLocator)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return serviceLocator.Get<IEventService>()?.IsInvoking<{typeName}>() ?? false;");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "RemoveRelevancyListener");
            sourceBuilder.AddLine("public static void RemoveRelevancyListener(IEventService eventService, in IEventService.EventRelevancyChangedHandler relevancyChangedHandler)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"eventService.RemoveRelevancyListener<{typeName}>(in relevancyChangedHandler);");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "RemoveRelevancyListener");
            sourceBuilder.AddLine("public static void RemoveRelevancyListener(ServiceLocator serviceLocator, in IEventService.EventRelevancyChangedHandler relevancyChangedHandler)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"serviceLocator.Get<IEventService>()?.RemoveRelevancyListener<{typeName}>(in relevancyChangedHandler);");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "Invoke", "object");
            sourceBuilder.AddLine("internal static bool Invoke(IEventService eventService, object sender)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return eventService.Invoke<{typeName}>(sender);");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "Invoke", "object");
            sourceBuilder.AddLine("internal static bool Invoke(ServiceLocator serviceLocator, object sender)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return serviceLocator.Get<IEventService>()?.Invoke<{typeName}>(sender) ?? false;");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "RemoveAllListeners");
            sourceBuilder.AddLine("internal static bool RemoveAllListeners(IEventService eventService)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return eventService.RemoveAllListeners<{typeName}>();");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "RemoveAllListeners");
            sourceBuilder.AddLine("internal static bool RemoveAllListeners(ServiceLocator serviceLocator)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return serviceLocator.Get<IEventService>()?.RemoveAllListeners<{typeName}>() ?? false;");
            }
        }

        private static void AddUtilityMethods(SourceBuilder sourceBuilder, string typeName)
        {
            AddMethodBoilerplate(sourceBuilder, "Invoke", "object");
            sourceBuilder.AddLine($"internal static bool Invoke(this {typeName} e, IEventService eventService, object sender)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return eventService.Invoke<{typeName}>(sender, in e);");
            }

            sourceBuilder.SkipLine();
            AddMethodBoilerplate(sourceBuilder, "Invoke", "object");
            sourceBuilder.AddLine($"internal static bool Invoke(this {typeName} e, ServiceLocator serviceLocator, object sender)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return serviceLocator.Get<IEventService>()?.Invoke<{typeName}>(sender, in e) ?? false;");
            }
        }

        private static IEnumerable<TypeDeclarationSyntax> EnumerateTypes(GeneratorExecutionContext context)
        {
            EventSyntaxReceiver syntaxReceiver = (EventSyntaxReceiver)context.SyntaxReceiver;

            foreach (TypeDeclarationSyntax syntaxNode in syntaxReceiver!.Types)
            {
                if (context.Compilation.GetSemanticModel(syntaxNode.SyntaxTree).GetDeclaredSymbol(syntaxNode) is { } typeSymbol
                 && typeSymbol.ImplementsInterface(CoimbraServicesEventsTypes.EventInterface))
                {
                    yield return syntaxNode;
                }
            }
        }
    }
}
