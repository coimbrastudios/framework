using Coimbra.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace Coimbra.Services.SourceGenerators
{
    [Generator]
    public sealed class EventGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            Console.WriteLine($"{nameof(EventGenerator)} executing on assembly {context.Compilation.AssemblyName}");

            try
            {
                EventSyntaxReceiver eventSyntaxReceiver = (EventSyntaxReceiver)context.SyntaxReceiver;

                if (eventSyntaxReceiver == null)
                {
                    return;
                }

                SourceBuilder sourceBuilder = new SourceBuilder();
                string[] usings =
                {
                    "Coimbra.Services",
                    "System.Collections.Generic"
                };

                foreach (ClassDeclarationSyntax node in eventSyntaxReceiver.Classes)
                {
                    sourceBuilder.Initialize(usings);

                    using (new NamespaceScope(sourceBuilder, node.GetNamespace()))
                    {
                        string prefix = node.Modifiers.Any(SyntaxKind.PublicKeyword) ? "public" : "internal";
                        prefix += node.Modifiers.Any(SyntaxKind.SealedKeyword) ? " sealed" : string.Empty;
                        sourceBuilder.AddLine($"{prefix} partial class {node.GetTypeName()}");

                        using (new BracesScope(sourceBuilder))
                        {
                            AddContent(sourceBuilder, node.GetTypeName());
                        }
                    }

                    Console.WriteLine($"Finished generating {node.GetTypeName()}");
                    context.AddSource(node.GetTypeName(), SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                }

                foreach (StructDeclarationSyntax node in eventSyntaxReceiver.Structs)
                {
                    sourceBuilder.Initialize(usings);

                    using (new NamespaceScope(sourceBuilder, node.GetNamespace()))
                    {
                        string prefix = node.Modifiers.Any(SyntaxKind.PublicKeyword) ? "public" : "internal";
                        prefix += node.Modifiers.Any(SyntaxKind.ReadOnlyKeyword) ? " readonly" : string.Empty;
                        sourceBuilder.AddLine($"{prefix} partial struct {node.GetTypeName()}");

                        using (new BracesScope(sourceBuilder))
                        {
                            AddContent(sourceBuilder, node.GetTypeName());
                        }
                    }

                    Console.WriteLine($"Finished generating {node.GetTypeName()}");
                    context.AddSource(node.GetTypeName(), SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new EventSyntaxReceiver());
        }

        private void AddContent(SourceBuilder sourceBuilder, string typeName)
        {
            void addMethod0(string prefix, string name)
            {
                sourceBuilder.AddLine("/// <summary>");
                sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{name}{{T}}\"/>");
                sourceBuilder.AddLine("/// </summary>");
                sourceBuilder.AddLine($"public {prefix} {name}(IEventService eventService = null)");

                using (new BracesScope(sourceBuilder))
                {
                    checkService();
                    sourceBuilder.AddLine($"return eventService.{name}<{typeName}>();");
                }
            }

            void addMethod1(string prefix, string name, string paramType, string paramName, string defaultValue = null)
            {
                sourceBuilder.AddLine("/// <summary>");
                sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{name}{{T}}({validateComment(paramType)})\"/>");
                sourceBuilder.AddLine("/// </summary>");
                sourceBuilder.AddLine($"public {prefix} {name}({paramType} {paramName}{(defaultValue != null ? $" = {defaultValue}" : string.Empty)}, IEventService eventService = null)");

                using (new BracesScope(sourceBuilder))
                {
                    checkService();
                    sourceBuilder.AddLine($"return eventService.{name}<{typeName}>({paramName});");
                }
            }

            void addMethod2(string prefix, string name, string paramType1, string paramName1, string paramType2, string paramName2, string defaultValue = null)
            {
                sourceBuilder.AddLine("/// <summary>");
                sourceBuilder.AddLine($"/// <inheritdoc cref=\"IEventService.{name}{{T}}({validateComment(paramType1)}, {validateComment(paramType2)})\"/>");
                sourceBuilder.AddLine("/// </summary>");
                sourceBuilder.AddLine($"public {prefix} {name}({paramType1} {paramName1}, {paramType2} {paramName2}{(defaultValue != null ? $" = {defaultValue}" : string.Empty)}, IEventService eventService = null)");

                using (new BracesScope(sourceBuilder))
                {
                    checkService();
                    sourceBuilder.AddLine($"return eventService.{name}<{typeName}>({paramName1}, {paramName2});");
                }
            }

            void checkService()
            {
                sourceBuilder.AddLine("eventService ??= ServiceLocator.Shared.Get<IEventService>();");
                sourceBuilder.SkipLine();
            }

            string validateComment(string value)
            {
                return value.Replace('<', '{').Replace('>', '}').Replace(typeName, "T");
            }

            addMethod1("static EventHandle", "AddListener", $"EventData<{typeName}>.Handler", "eventCallback");
            sourceBuilder.SkipLine();
            addMethod2("static bool", "AddListener", $"EventData<{typeName}>.Handler", "eventCallback", "List<EventHandle>", "appendList");
            sourceBuilder.SkipLine();
            addMethod1("static bool", "CompareEventKey", "EventKey", "eventKey");
            sourceBuilder.SkipLine();
            addMethod0("static bool", "HasAnyListeners");
            sourceBuilder.SkipLine();
            addMethod0("static bool", "IsInvoking");
            sourceBuilder.SkipLine();
            addMethod1("static bool", "RemoveAllListeners", "EventKey", "eventKey", "null");
            sourceBuilder.SkipLine();
            addMethod1("static bool", "ResetEventKey", "EventKey", "eventKey");
            sourceBuilder.SkipLine();
            addMethod1("static bool", "SetEventKey", "EventKey", "eventKey");
            sourceBuilder.SkipLine();
            addMethod2("bool", "Invoke", "object", "sender", "EventKey", "key", "null");
        }
    }
}
