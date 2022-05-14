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
                            AddMethods(sourceBuilder, typeName);
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

        private static void AddMethodBoilerplate(SourceBuilder sourceBuilder, string originalMethod, string originalParameters)
        {
            sourceBuilder.AddLine("/// <summary>");
            sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{originalMethod}{{T}}({originalParameters})\"/>");
            sourceBuilder.AddLine("/// </summary>");
            sourceBuilder.AddLine("[CompilerGenerated]");
            sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraServicesEventsTypes.Namespace}.Roslyn.{nameof(EventMethodsGenerator)}\", \"1.0.0.0\")]");
            sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        }

        private static void AddMethods(SourceBuilder sourceBuilder, string typeName)
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            string validateComment(string value)
            {
                return value.Replace('<', '{').Replace('>', '}').Replace(typeName, "T");
            }

            const string atMethodReturn = "";
            const string atMethodService = "eventService";
            const string atMethodSuffix = "At(IEventService eventService";
            const string sharedMethodReturn = " ?? default";
            const string sharedMethodService = "ServiceLocator.Shared.Get<IEventService>()?";
            const string sharedMethodSuffix = "Shared(";

            void addActions1(string generatedPrefix, string originalMethod, string paramType, string paramName, string defaultValue = null)
            {
                void addAction(string generatedMethod, string targetService)
                {
                    AddMethodBoilerplate(sourceBuilder, originalMethod, validateComment(paramType));
                    sourceBuilder.AddLine($"{generatedPrefix} {generatedMethod}{paramType} {paramName}{(defaultValue != null ? $" = {defaultValue}" : string.Empty)})");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"{targetService}.{originalMethod}<{typeName}>({paramName});");
                    }
                }

                addAction($"{originalMethod}{atMethodSuffix}, ", atMethodService);
                sourceBuilder.SkipLine();
                addAction($"{originalMethod}{sharedMethodSuffix}", sharedMethodService);
            }

            void addFunctions0(string generatedPrefix, string originalMethod)
            {
                void addFunction(string generatedMethod, string targetService, string bodySuffix)
                {
                    AddMethodBoilerplate(sourceBuilder, originalMethod, string.Empty);
                    sourceBuilder.AddLine($"{generatedPrefix} {generatedMethod})");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {targetService}.{originalMethod}<{typeName}>(){bodySuffix};");
                    }
                }

                addFunction($"{originalMethod}{atMethodSuffix}", atMethodService, atMethodReturn);
                sourceBuilder.SkipLine();
                addFunction($"{originalMethod}{sharedMethodSuffix}", sharedMethodService, sharedMethodReturn);
            }

            void addFunctions1(string generatedPrefix, string originalMethod, string paramType, string paramName)
            {
                void addFunction(string generatedMethod, string targetService, string bodySuffix)
                {
                    AddMethodBoilerplate(sourceBuilder, originalMethod, validateComment(paramType));
                    sourceBuilder.AddLine($"{generatedPrefix} {generatedMethod}{paramType} {paramName})");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {targetService}.{originalMethod}<{typeName}>({paramName}){bodySuffix};");
                    }
                }

                addFunction($"{originalMethod}{atMethodSuffix}, ", atMethodService, atMethodReturn);
                sourceBuilder.SkipLine();
                addFunction($"{originalMethod}{sharedMethodSuffix}", sharedMethodService, sharedMethodReturn);
            }

            void addFunctions2(string generatedPrefix, string originalMethod, string paramType1, string paramName1, string paramType2, string paramName2)
            {
                void addFunction(string generatedMethod, string targetService, string bodySuffix)
                {
                    AddMethodBoilerplate(sourceBuilder, originalMethod, $"{validateComment(paramType1)}, {validateComment(paramType2)}");
                    sourceBuilder.AddLine($"{generatedPrefix} {generatedMethod}{paramType1} {paramName1}, {paramType2} {paramName2})");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {targetService}.{originalMethod}<{typeName}>({paramName1}, {paramName2}){bodySuffix};");
                    }
                }

                addFunction($"{originalMethod}{atMethodSuffix}, ", atMethodService, atMethodReturn);
                sourceBuilder.SkipLine();
                addFunction($"{originalMethod}{sharedMethodSuffix}", sharedMethodService, sharedMethodReturn);
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
            sourceBuilder.SkipLine();

            void addStaticInvokeFunctions()
            {
                const string generatedPrefix = "internal new static bool";
                const string originalMethod = "Invoke";

                void addFunction0(string generatedMethod, string targetService, string bodySuffix)
                {
                    AddMethodBoilerplate(sourceBuilder, originalMethod, "object");
                    sourceBuilder.AddLine($"{generatedPrefix} {generatedMethod}object sender)");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {targetService}.{originalMethod}<{typeName}>(sender){bodySuffix};");
                    }
                }

                void addFunction1(string generatedMethod, string targetService, string bodySuffix)
                {
                    AddMethodBoilerplate(sourceBuilder, originalMethod, "object");
                    sourceBuilder.AddLine($"{generatedPrefix} {generatedMethod}object sender, ref {typeName} data)");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {targetService}.{originalMethod}<{typeName}>(sender, ref data){bodySuffix};");
                    }
                }

                addFunction0($"{originalMethod}{atMethodSuffix}, ", atMethodService, atMethodReturn);
                sourceBuilder.SkipLine();
                addFunction1($"{originalMethod}{atMethodSuffix}, ", atMethodService, atMethodReturn);
                sourceBuilder.SkipLine();
                addFunction0($"{originalMethod}{sharedMethodSuffix}", sharedMethodService, sharedMethodReturn);
                sourceBuilder.SkipLine();
                addFunction1($"{originalMethod}{sharedMethodSuffix}", sharedMethodService, sharedMethodReturn);
            }

            addStaticInvokeFunctions();
            sourceBuilder.SkipLine();
            addFunctions0("internal new static bool", "RemoveAllListeners");
            sourceBuilder.SkipLine();

            void addTryInvokeFunctions()
            {
                const string generatedPrefix = "internal new bool";
                const string originalMethod = "Invoke";

                void addFunction(string generatedMethod, string targetService)
                {
                    AddMethodBoilerplate(sourceBuilder, originalMethod, "object");
                    sourceBuilder.AddLine($"{generatedPrefix} {generatedMethod}object sender)");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {targetService}.{originalMethod}<{typeName}>(sender, this) ?? false;");
                    }
                }

                addFunction($"Try{originalMethod}At(IEventService? eventService, ", $"{atMethodService}?");
                sourceBuilder.SkipLine();
                addFunction($"Try{originalMethod}{sharedMethodSuffix}", sharedMethodService);
            }

            addTryInvokeFunctions();
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
