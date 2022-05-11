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

                                    lineScope.AddContent(" partial class");

                                    break;
                                }

                                case StructDeclarationSyntax _:
                                {
                                    if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.SealedKeyword))
                                    {
                                        lineScope.AddContent(" readonly");
                                    }

                                    lineScope.AddContent(" partial struct");

                                    break;
                                }

                                default:
                                {
                                    continue;
                                }
                            }

                            lineScope.AddContent($" {typeDeclarationSyntax.GetTypeName()}");
                        }

                        using (new BracesScope(sourceBuilder))
                        {
                            string typeName = typeDeclarationSyntax.GetTypeName();
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

            void addInvokeMethods()
            {
                const string returnType = "bool";
                const string serviceMethod = "Invoke";

                void addMethod(string method, string service)
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

                addMethod($"Try{serviceMethod}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addMethod($"Try{serviceMethod}{sharedMethod}", sharedService);
            }

            void addMethods0(string prefix, string name, string access)
            {
                void addMethod(string method, string service)
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

                addMethod($"{name}{atMethod}", atService);
                sourceBuilder.SkipLine();
                addMethod($"{name}{sharedMethod}", sharedService);
            }

            void addMethods1(string prefix, string name, string paramType, string paramName, string defaultValue = null)
            {
                void addMethod(string method, string service)
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

                addMethod($"{name}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addMethod($"{name}{sharedMethod}", sharedService);
            }

            void addMethods2(string prefix, string name, string paramType1, string paramName1, string paramType2, string paramName2, string defaultValue = null)
            {
                void addMethod(string method, string service)
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

                addMethod($"{name}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addMethod($"{name}{sharedMethod}", sharedService);
            }

            void addStaticInvokeMethods()
            {
                const string returnType = "static bool";
                const string serviceMethod = "Invoke";

                void addMethod0(string method, string service)
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

                void addMethod1(string method, string service)
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

                addMethod0($"{serviceMethod}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addMethod1($"{serviceMethod}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addMethod0($"{serviceMethod}{sharedMethod}", sharedService);
                sourceBuilder.SkipLine();
                addMethod1($"{serviceMethod}{sharedMethod}", sharedService);
            }

            string validateComment(string value)
            {
                return value.Replace('<', '{').Replace('>', '}').Replace(typeName, "T");
            }

            addMethods1("static EventHandle", "AddListener", $"Event<{typeName}>.Handler", "eventCallback");
            sourceBuilder.SkipLine();
            addMethods2("static bool", "AddListener", $"Event<{typeName}>.Handler", "eventCallback", "List<EventHandle>", "appendList");
            sourceBuilder.SkipLine();
            addMethods0("static int", "GetListenerCount", "public");
            sourceBuilder.SkipLine();
            addMethods0("static bool", "IsInvoking", "public");
            sourceBuilder.SkipLine();
            addStaticInvokeMethods();
            sourceBuilder.SkipLine();
            addMethods0("static bool", "RemoveAllListeners", "internal");
            sourceBuilder.SkipLine();
            addInvokeMethods();
        }
    }
}
