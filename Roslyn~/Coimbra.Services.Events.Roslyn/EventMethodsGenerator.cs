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
            static IEnumerable<TypeDeclarationSyntax> getTypes(GeneratorExecutionContext context)
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

            SourceBuilder sourceBuilder = new();

            foreach (TypeDeclarationSyntax typeDeclarationSyntax in getTypes(context))
            {
                sourceBuilder.Initialize();

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
                        string typeName;

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

                            typeName = typeDeclarationSyntax.TypeParameterList != null ? $"{typeDeclarationSyntax.GetTypeName()}{typeDeclarationSyntax.TypeParameterList}" : typeDeclarationSyntax.GetTypeName();
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

        private static void AddMethods(SourceBuilder sourceBuilder, string typeName)
        {
            const string atMethod = "At(IEventService eventService";
            const string atService = "eventService";
            const string sharedMethod = "Shared(";
            const string sharedService = "ServiceLocator.Shared.Get<IEventService>()";
            string generatedCodeAttribute = $"[GeneratedCode(\"{CoimbraServicesEventsTypes.Namespace}.Roslyn.{nameof(EventMethodsGenerator)}\", \"1.0.0.0\")]";

            void addActions1(string prefix, string name, string paramType, string paramName, string defaultValue = null)
            {
                void addFunction(string method, string service)
                {
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{name}{{T}}({validateComment(paramType)})\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine("[CompilerGenerated]");
                    sourceBuilder.AddLine(generatedCodeAttribute);
                    sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    sourceBuilder.AddLine($"public new {prefix} {method}{paramType} {paramName}{(defaultValue != null ? $" = {defaultValue}" : string.Empty)})");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"{service}.{name}<{typeName}>({paramName});");
                    }
                }

                addFunction($"{name}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addFunction($"{name}{sharedMethod}", sharedService);
            }

            void addInvokeFunctions()
            {
                const string returnType = "bool";
                const string serviceMethod = "Invoke";

                void addFunction(string method, string service)
                {
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{serviceMethod}{{T}}(object)\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine("[CompilerGenerated]");
                    sourceBuilder.AddLine(generatedCodeAttribute);
                    sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    sourceBuilder.AddLine($"internal new {returnType} {method}object sender)");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {service}?.{serviceMethod}<{typeName}>(sender, this) ?? false;");
                    }
                }

                addFunction($"Try{serviceMethod}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addFunction($"Try{serviceMethod}{sharedMethod}", sharedService);
            }

            void addFunctions0(string prefix, string name, string access)
            {
                void addFunction(string method, string service)
                {
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{name}{{T}}\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine("[CompilerGenerated]");
                    sourceBuilder.AddLine(generatedCodeAttribute);
                    sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    sourceBuilder.AddLine($"{access} new {prefix} {method})");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {service}.{name}<{typeName}>();");
                    }
                }

                addFunction($"{name}{atMethod}", atService);
                sourceBuilder.SkipLine();
                addFunction($"{name}{sharedMethod}", sharedService);
            }

            void addFunctions1(string prefix, string name, string paramType, string paramName, string defaultValue = null)
            {
                void addFunction(string method, string service)
                {
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{name}{{T}}({validateComment(paramType)})\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine("[CompilerGenerated]");
                    sourceBuilder.AddLine(generatedCodeAttribute);
                    sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    sourceBuilder.AddLine($"public new {prefix} {method}{paramType} {paramName}{(defaultValue != null ? $" = {defaultValue}" : string.Empty)})");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {service}.{name}<{typeName}>({paramName});");
                    }
                }

                addFunction($"{name}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addFunction($"{name}{sharedMethod}", sharedService);
            }

            void addFunctions2(string prefix, string name, string paramType1, string paramName1, string paramType2, string paramName2, string defaultValue = null)
            {
                void addFunction(string method, string service)
                {
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{name}{{T}}({validateComment(paramType1)}, {validateComment(paramType2)})\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine("[CompilerGenerated]");
                    sourceBuilder.AddLine(generatedCodeAttribute);
                    sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    sourceBuilder.AddLine($"public new {prefix} {method}{paramType1} {paramName1}, {paramType2} {paramName2}{(defaultValue != null ? $" = {defaultValue}" : string.Empty)})");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {service}.{name}<{typeName}>({paramName1}, {paramName2});");
                    }
                }

                addFunction($"{name}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addFunction($"{name}{sharedMethod}", sharedService);
            }

            void addStaticInvokeFunctions()
            {
                const string returnType = "static bool";
                const string serviceMethod = "Invoke";

                void addFunction0(string method, string service)
                {
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{serviceMethod}{{T}}(object)\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine("[CompilerGenerated]");
                    sourceBuilder.AddLine(generatedCodeAttribute);
                    sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    sourceBuilder.AddLine($"internal new {returnType} {method}object sender)");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {service}.{serviceMethod}<{typeName}>(sender);");
                    }
                }

                void addFunction1(string method, string service)
                {
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{serviceMethod}{{T}}(object)\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine("[CompilerGenerated]");
                    sourceBuilder.AddLine(generatedCodeAttribute);
                    sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    sourceBuilder.AddLine($"internal new {returnType} {method}object sender, ref {typeName} data)");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {service}.{serviceMethod}<{typeName}>(sender, ref data);");
                    }
                }

                addFunction0($"{serviceMethod}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addFunction1($"{serviceMethod}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addFunction0($"{serviceMethod}{sharedMethod}", sharedService);
                sourceBuilder.SkipLine();
                addFunction1($"{serviceMethod}{sharedMethod}", sharedService);
            }

            string validateComment(string value)
            {
                return value.Replace('<', '{').Replace('>', '}').Replace(typeName, "T");
            }

            addFunctions1("static EventHandle", "AddListener", $"Event<{typeName}>.Handler", "eventCallback");
            sourceBuilder.SkipLine();
            addFunctions2("static bool", "AddListener", $"Event<{typeName}>.Handler", "eventCallback", "List<EventHandle>", "appendList");
            sourceBuilder.SkipLine();
            addActions1("static void", "AddRelevancyListener", "IEventService.EventRelevancyChangedHandler", "relevancyChangedHandler");
            sourceBuilder.SkipLine();
            addFunctions0("static int", "GetListenerCount", "public");
            sourceBuilder.SkipLine();
            addFunctions0("static bool", "IsInvoking", "public");
            sourceBuilder.SkipLine();
            addActions1("static void", "RemoveRelevancyListener", "IEventService.EventRelevancyChangedHandler", "relevancyChangedHandler");
            sourceBuilder.SkipLine();
            addStaticInvokeFunctions();
            sourceBuilder.SkipLine();
            addFunctions0("static bool", "RemoveAllListeners", "internal");
            sourceBuilder.SkipLine();
            addInvokeFunctions();
        }
    }
}
