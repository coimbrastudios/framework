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
    public sealed class EventsGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            Console.WriteLine($"{nameof(EventsGenerator)} executing on assembly {context.Compilation.AssemblyName}");

            try
            {
                EventsSyntaxReceiver syntaxReceiver = (EventsSyntaxReceiver)context.SyntaxReceiver;

                if (syntaxReceiver == null)
                {
                    return;
                }

                SourceBuilder sourceBuilder = new SourceBuilder();

                string[] usings =
                {
                    "Coimbra.Services",
                    "Coimbra.Services.Events",
                    "System.Collections.Generic",
                    "System.Runtime.CompilerServices"
                };

                foreach (ClassDeclarationSyntax node in syntaxReceiver.Classes)
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

                foreach (StructDeclarationSyntax node in syntaxReceiver.Structs)
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
            context.RegisterForSyntaxNotifications(() => new EventsSyntaxReceiver());
        }

        private void AddContent(SourceBuilder sourceBuilder, string typeName)
        {
            const string atMethod = "At(IEventService eventService";
            const string atService = "eventService";
            const string sharedMethod = "Shared(";
            const string sharedService = "ServiceLocator.Shared.Get<IEventService>()";

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

            addMethods1("static EventHandle", "AddListener", $"EventData<{typeName}>.Handler", "eventCallback");
            sourceBuilder.SkipLine();
            addMethods2("static bool", "AddListener", $"EventData<{typeName}>.Handler", "eventCallback", "List<EventHandle>", "appendList");
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
            addMethods2("bool", "Invoke", "object", "sender", "EventKey", "key", "null");
            sourceBuilder.SkipLine();
            sourceBuilder.AddLine("/// <summary>");
            sourceBuilder.AddLine("/// Returns an <see cref=\"EventData{T}\"/> for this event.");
            sourceBuilder.AddLine("/// </summary>");
            sourceBuilder.AddLine($"public new EventData<{typeName}> ToData(object sender)");

            using (new BracesScope(sourceBuilder))
            {
                sourceBuilder.AddLine($"return new EventData<{typeName}>(sender, this);");
            }
        }
    }
}
