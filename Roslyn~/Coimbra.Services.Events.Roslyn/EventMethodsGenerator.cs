using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Coimbra.Services.Events.Roslyn
{
    [Generator]
    public sealed class EventMethodsGenerator : ISourceGenerator
    {
        private const string AtMethodService = "eventService";

        private const string AtMethodSuffixFormat = "At(IEventService{0} eventService";

        private const string SharedMethodService = "ServiceLocator.Shared.Get<IEventService>()";

        private const string SharedMethodSuffix = "Shared(";

        private const string TryBodySuffix = " ?? default";

        private const string TryMethodPrefix = "Try";

        private const string TrySymbol = "?";

        public void Execute(GeneratorExecutionContext context)
        {
            SourceBuilder sourceBuilder = new();

            foreach (TypeDeclarationSyntax typeDeclarationSyntax in EnumerateTypes(context))
            {
                sourceBuilder.Initialize();
                sourceBuilder.AddLine("#nullable enable");
                sourceBuilder.SkipLine();

                using (new PragmaWarningDisableScope(sourceBuilder, "0109"))
                {
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
                            AddInstanceMethods(sourceBuilder, typeName);
                        }

                        sourceBuilder.SkipLine();
                        sourceBuilder.AddLine($"internal static class {typeName}GeneratedExtensions");

                        using (new BracesScope(sourceBuilder))
                        {
                            AddExtensionMethods(sourceBuilder, typeName);
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

        private static void AddExtensionMethods(SourceBuilder sourceBuilder, string typeName)
        {
            void addInvokeFunctions(string methodPrefix, string specialSymbol, string bodySuffix)
            {
                const string generatedPrefix = "internal static bool";
                const string originalMethod = "Invoke";

                void addFunction(string generatedMethod, string targetService)
                {
                    AddMethodBoilerplate(sourceBuilder, originalMethod, "object");
                    sourceBuilder.AddLine($"{generatedPrefix} {generatedMethod}object sender)");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {targetService}{specialSymbol}.{originalMethod}<{typeName}>(sender, ref e){bodySuffix};");
                    }
                }

                addFunction($"{methodPrefix}{originalMethod}At(this {typeName} e, IEventService{specialSymbol} eventService, ", AtMethodService);

                if (string.IsNullOrWhiteSpace(specialSymbol))
                {
                    return;
                }

                sourceBuilder.SkipLine();
                addFunction($"{methodPrefix}{originalMethod}{SharedMethodSuffix}this {typeName} e, ", SharedMethodService);
            }

            addInvokeFunctions(string.Empty, string.Empty, string.Empty);
            addInvokeFunctions(TryMethodPrefix, TrySymbol, TryBodySuffix);
        }

        private static void AddInstanceMethods(SourceBuilder sourceBuilder, string typeName)
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            string validateComment(string value)
            {
                return value.Replace('<', '{').Replace('>', '}').Replace(typeName, "T");
            }

            void addInternalMethods(string methodPrefix, string specialSymbol, string bodySuffix)
            {
                void addInvokeFunctions()
                {
                    const string generatedPrefix = "internal new static bool";
                    const string originalMethod = "Invoke";

                    void addFunction0(string generatedMethod, string targetService)
                    {
                        AddMethodBoilerplate(sourceBuilder, originalMethod, "object");
                        sourceBuilder.AddLine($"{generatedPrefix} {methodPrefix}{generatedMethod}object sender)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {targetService}{specialSymbol}.{originalMethod}<{typeName}>(sender){bodySuffix};");
                        }
                    }

                    void addFunction1(string generatedMethod, string targetService)
                    {
                        AddMethodBoilerplate(sourceBuilder, originalMethod, "object");
                        sourceBuilder.AddLine($"{generatedPrefix} {methodPrefix}{generatedMethod}object sender, ref {typeName} data)");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {targetService}{specialSymbol}.{originalMethod}<{typeName}>(sender, ref data){bodySuffix};");
                        }
                    }

                    addFunction0($"{originalMethod}{string.Format(AtMethodSuffixFormat, specialSymbol)}, ", AtMethodService);
                    sourceBuilder.SkipLine();
                    addFunction1($"{originalMethod}{string.Format(AtMethodSuffixFormat, specialSymbol)}, ", AtMethodService);

                    if (string.IsNullOrWhiteSpace(specialSymbol))
                    {
                        return;
                    }

                    sourceBuilder.SkipLine();
                    addFunction0($"{originalMethod}{SharedMethodSuffix}", SharedMethodService);
                    sourceBuilder.SkipLine();
                    addFunction1($"{originalMethod}{SharedMethodSuffix}", SharedMethodService);
                }

                void addFunctions0(string generatedPrefix, string originalMethod)
                {
                    void addFunction(string generatedMethod, string targetService)
                    {
                        AddMethodBoilerplate(sourceBuilder, originalMethod, string.Empty);
                        sourceBuilder.AddLine($"{generatedPrefix} {methodPrefix}{generatedMethod})");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {targetService}{specialSymbol}.{originalMethod}<{typeName}>(){bodySuffix};");
                        }
                    }

                    addFunction($"{originalMethod}{string.Format(AtMethodSuffixFormat, specialSymbol)}", AtMethodService);

                    if (string.IsNullOrWhiteSpace(specialSymbol))
                    {
                        return;
                    }

                    sourceBuilder.SkipLine();
                    addFunction($"{originalMethod}{SharedMethodSuffix}", SharedMethodService);
                }

                addInvokeFunctions();
                sourceBuilder.SkipLine();
                addFunctions0("internal new static bool", "RemoveAllListeners");
            }

            void addPublicMethods(string methodPrefix, string specialSymbol, string bodySuffix)
            {
                void addActions1(string generatedPrefix, string originalMethod, string paramType, string paramName, string defaultValue = null)
                {
                    void addAction(string generatedMethod, string targetService)
                    {
                        AddMethodBoilerplate(sourceBuilder, originalMethod, validateComment(paramType));
                        sourceBuilder.AddLine($"{generatedPrefix} {methodPrefix}{generatedMethod}{paramType} {paramName}{(defaultValue != null ? $" = {defaultValue}" : string.Empty)})");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"{targetService}{specialSymbol}.{originalMethod}<{typeName}>({paramName});");
                        }
                    }

                    addAction($"{originalMethod}{string.Format(AtMethodSuffixFormat, specialSymbol)}, ", AtMethodService);

                    if (string.IsNullOrWhiteSpace(specialSymbol))
                    {
                        return;
                    }

                    sourceBuilder.SkipLine();
                    addAction($"{originalMethod}{SharedMethodSuffix}", SharedMethodService);
                }

                void addFunctions0(string generatedPrefix, string originalMethod)
                {
                    void addFunction(string generatedMethod, string targetService)
                    {
                        AddMethodBoilerplate(sourceBuilder, originalMethod, string.Empty);
                        sourceBuilder.AddLine($"{generatedPrefix} {methodPrefix}{generatedMethod})");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {targetService}{specialSymbol}.{originalMethod}<{typeName}>(){bodySuffix};");
                        }
                    }

                    addFunction($"{originalMethod}{string.Format(AtMethodSuffixFormat, specialSymbol)}", AtMethodService);

                    if (string.IsNullOrWhiteSpace(specialSymbol))
                    {
                        return;
                    }

                    sourceBuilder.SkipLine();
                    addFunction($"{originalMethod}{SharedMethodSuffix}", SharedMethodService);
                }

                void addFunctions1(string generatedPrefix, string originalMethod, string paramType, string paramName)
                {
                    void addFunction(string generatedMethod, string targetService)
                    {
                        AddMethodBoilerplate(sourceBuilder, originalMethod, validateComment(paramType));
                        sourceBuilder.AddLine($"{generatedPrefix} {methodPrefix}{generatedMethod}{paramType} {paramName})");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {targetService}{specialSymbol}.{originalMethod}<{typeName}>({paramName}){bodySuffix};");
                        }
                    }

                    addFunction($"{originalMethod}{string.Format(AtMethodSuffixFormat, specialSymbol)}, ", AtMethodService);

                    if (string.IsNullOrWhiteSpace(specialSymbol))
                    {
                        return;
                    }

                    sourceBuilder.SkipLine();
                    addFunction($"{originalMethod}{SharedMethodSuffix}", SharedMethodService);
                }

                void addFunctions2(string generatedPrefix, string originalMethod, string paramType1, string paramName1, string paramType2, string paramName2)
                {
                    void addFunction(string generatedMethod, string targetService)
                    {
                        AddMethodBoilerplate(sourceBuilder, originalMethod, $"{validateComment(paramType1)}, {validateComment(paramType2)}");
                        sourceBuilder.AddLine($"{generatedPrefix} {methodPrefix}{generatedMethod}{paramType1} {paramName1}, {paramType2} {paramName2})");

                        using (new BracesScope(sourceBuilder))
                        {
                            sourceBuilder.AddLine($"return {targetService}{specialSymbol}.{originalMethod}<{typeName}>({paramName1}, {paramName2}){bodySuffix};");
                        }
                    }

                    addFunction($"{originalMethod}{string.Format(AtMethodSuffixFormat, specialSymbol)}, ", AtMethodService);

                    if (string.IsNullOrWhiteSpace(specialSymbol))
                    {
                        return;
                    }

                    sourceBuilder.SkipLine();
                    addFunction($"{originalMethod}{SharedMethodSuffix}", SharedMethodService);
                }

                addFunctions1("public new static EventHandle", "AddListener", $"Event<{typeName}>.Handler", "eventCallback");
                sourceBuilder.SkipLine();
                addFunctions2("public new static bool", "AddListener", $"Event<{typeName}>.Handler", "eventCallback", "List<EventHandle>", "appendList");
                sourceBuilder.SkipLine();
                addActions1("public new static void", "AddRelevancyListener", "IEventService.EventRelevancyChangedHandler", "relevancyChangedHandler");
                sourceBuilder.SkipLine();
                addFunctions0("public new static int", "GetListenerCount");
                sourceBuilder.SkipLine();
                addFunctions0("public new static bool", "IsInvoking");
                sourceBuilder.SkipLine();
                addActions1("public new static void", "RemoveRelevancyListener", "IEventService.EventRelevancyChangedHandler", "relevancyChangedHandler");
            }

            addPublicMethods(string.Empty, string.Empty, string.Empty);
            sourceBuilder.SkipLine();
            addPublicMethods(TryMethodPrefix, TrySymbol, TryBodySuffix);
            sourceBuilder.SkipLine();
            addInternalMethods(string.Empty, string.Empty, string.Empty);
            sourceBuilder.SkipLine();
            addInternalMethods(TryMethodPrefix, TrySymbol, TryBodySuffix);
        }

        private static void AddMethodBoilerplate(SourceBuilder sourceBuilder, string originalMethod, string originalParameters)
        {
            sourceBuilder.AddLine("/// <summary>");
            sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{originalMethod}{{T}}({originalParameters})\"/>");
            sourceBuilder.AddLine("/// </summary>");
            sourceBuilder.AddLine("[CompilerGenerated]");
            sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraServicesEventsTypes.Namespace}.Roslyn.{nameof(EventMethodsGenerator)}\", \"1.0.0.0\")]");
            sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        }

        private static IEnumerable<TypeDeclarationSyntax> EnumerateTypes(GeneratorExecutionContext context)
        {
            EventSyntaxReceiver syntaxReceiver = (EventSyntaxReceiver)context.SyntaxReceiver;

            foreach (TypeDeclarationSyntax syntaxNode in syntaxReceiver!.Types)
            {
                if (context.Compilation.GetSemanticModel(syntaxNode.SyntaxTree).GetDeclaredSymbol(syntaxNode) is { } typeSymbol
                 && typeSymbol.ImplementsInterface(CoimbraServicesEventsTypes.EventInterface, CoimbraServicesEventsTypes.Namespace))
                {
                    yield return syntaxNode;
                }
            }
        }
    }
}
