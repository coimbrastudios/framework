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
            EventContextReceiver contextReceiver = (EventContextReceiver)context.SyntaxContextReceiver;
            SourceBuilder sourceBuilder = new();

            foreach (TypeDeclarationSyntax typeDeclarationSyntax in contextReceiver!.Types)
            {
                sourceBuilder.Initialize();

                using (new PragmaWarningDisableScope(sourceBuilder, "0109"))
                {
                    sourceBuilder.AddUsing("Coimbra.Services");
                    sourceBuilder.AddUsing("Coimbra.Services.Events");
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
            context.RegisterForSyntaxNotifications(() => new EventContextReceiver());
        }

        private static void AddMethods(SourceBuilder sourceBuilder, string typeName)
        {
            const string atMethod = "At(IEventService eventService";
            const string atService = "eventService";
            const string sharedMethod = "Shared(";
            const string sharedService = "ServiceLocator.Shared.Get<IEventService>()";

            void addInvokeMethods()
            {
                const string prefix = "bool";
                const string name = "Invoke";

                void addMethod(string method, string service)
                {
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{name}{{T}}(object, EventKey)\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    sourceBuilder.AddLine($"public new {prefix} {method}object sender, EventKey key = null)");

                    using (new BracesScope(sourceBuilder))
                    {
                        sourceBuilder.AddLine($"return {service}.{name}<{typeName}>(sender, this, key);");
                    }
                }

                addMethod($"{name}{atMethod}, ", atService);
                sourceBuilder.SkipLine();
                addMethod($"{name}{sharedMethod}", sharedService);
            }

            void addMethods0(string prefix, string name)
            {
                void addMethod(string method, string service)
                {
                    sourceBuilder.AddLine("/// <summary>");
                    sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{name}{{T}}\"/>");
                    sourceBuilder.AddLine("/// </summary>");
                    sourceBuilder.AddLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                    sourceBuilder.AddLine($"public new {prefix} {method})");

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

            string validateComment(string value)
            {
                return value.Replace('<', '{').Replace('>', '}').Replace(typeName, "T");
            }

            addMethods1("static EventHandle", "AddListener", $"Event<{typeName}>.Handler", "eventCallback");
            sourceBuilder.SkipLine();
            addMethods2("static bool", "AddListener", $"Event<{typeName}>.Handler", "eventCallback", "List<EventHandle>", "appendList");
            sourceBuilder.SkipLine();
            addMethods1("static bool", "CompareEventKey", "EventKey", "eventKey");
            sourceBuilder.SkipLine();
            addMethods0("static bool", "HasAnyListeners");
            sourceBuilder.SkipLine();
            addMethods0("static bool", "IsInvoking");
            sourceBuilder.SkipLine();
            addMethods1("static bool", "RemoveAllListeners", "EventKey", "eventKey", "null");
            sourceBuilder.SkipLine();
            addMethods1("static bool", "ResetEventKey", "EventKey", "eventKey");
            sourceBuilder.SkipLine();
            addMethods1("static bool", "SetEventKey", "EventKey", "eventKey");
            sourceBuilder.SkipLine();
            addInvokeMethods();
        }
    }
}
