using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace Coimbra.Services.Roslyn
{
    [Generator]
    public sealed class ServiceLoaderGenerator : ISourceGenerator
    {
        private struct TypeData
        {
            public ITypeSymbol ClassSymbol;

            public INamedTypeSymbol InterfaceSymbol;

            public bool DisableDefaultFactory;

            public bool IsActor;

            public bool PreloadService;
        }

        public void Execute(GeneratorExecutionContext context)
        {
            SourceBuilder sourceBuilder = new();

            foreach (TypeData typeData in EnumerateTypes(context))
            {
                TypeString source = new($"{typeData.ClassSymbol.Name}Loader", typeData.ClassSymbol.ContainingNamespace.ToString());
                sourceBuilder.Initialize();

                using (UsingScope usingScope = sourceBuilder.BeginUsing())
                {
                    usingScope.AddContent("Coimbra.Services");
                    usingScope.AddContent("System.CodeDom.Compiler");
                    usingScope.AddContent("UnityEngine");
                    usingScope.AddContent(typeData.InterfaceSymbol.ContainingNamespace.ToString());
                }

                sourceBuilder.SkipLine();

                using (new NamespaceScope(sourceBuilder, source.Namespace))
                {
                    sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraServicesTypes.Namespace}.Roslyn.{nameof(ServiceLoaderGenerator)}\", \"1.0.0.0\")]");
                    sourceBuilder.AddLine($"internal static class {source.Name}");

                    using (new BracesScope(sourceBuilder))
                    {
                        if (!typeData.DisableDefaultFactory)
                        {
                            sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraServicesTypes.Namespace}.Roslyn.{nameof(ServiceLoaderGenerator)}\", \"1.0.0.0\")]");
                            sourceBuilder.AddLine("[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]");
                            sourceBuilder.AddLine("private static void HandleSubsystemRegistration()");

                            using (new BracesScope(sourceBuilder))
                            {
                                sourceBuilder.AddLine($"if (!ServiceLocator.HasFactory<{typeData.InterfaceSymbol.Name}>())");

                                using (new BracesScope(sourceBuilder))
                                {
                                    TypeString factory = typeData.IsActor ? CoimbraServicesTypes.DefaultServiceActorFactoryClass : CoimbraServicesTypes.DefaultServiceFactoryClass;
                                    sourceBuilder.AddLine($"ServiceLocator.SetFactory<{typeData.InterfaceSymbol.Name}>({factory.Name}<{typeData.ClassSymbol.Name}>.Instance);");
                                }
                            }

                            if (typeData.PreloadService)
                            {
                                sourceBuilder.SkipLine();
                            }
                        }

                        if (typeData.PreloadService)
                        {
                            sourceBuilder.AddLine($"[GeneratedCode(\"{CoimbraServicesTypes.Namespace}.Roslyn.{nameof(ServiceLoaderGenerator)}\", \"1.0.0.0\")]");
                            sourceBuilder.AddLine("[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]");
                            sourceBuilder.AddLine("private static void HandleBeforeSceneLoad()");

                            using (new BracesScope(sourceBuilder))
                            {
                                if (typeData.InterfaceSymbol.HasAttribute(CoimbraServicesTypes.RequiredServiceAttribute, out _, false))
                                {
                                    sourceBuilder.AddLine($"ServiceLocator.GetChecked<{typeData.InterfaceSymbol.Name}>();");
                                }
                                else
                                {
                                    sourceBuilder.AddLine($"ServiceLocator.Get<{typeData.InterfaceSymbol.Name}>();");
                                }
                            }
                        }
                    }
                }

                context.AddSource(source.FullName, SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ServiceLoaderSyntaxReceiver());
        }

        private static IEnumerable<TypeData> EnumerateTypes(GeneratorExecutionContext context)
        {
            ServiceLoaderSyntaxReceiver syntaxReceiver = (ServiceLoaderSyntaxReceiver)context.SyntaxReceiver;

            foreach (ClassDeclarationSyntax syntaxNode in syntaxReceiver!.Types)
            {
                SemanticModel semanticModel = context.Compilation.GetSemanticModel(syntaxNode.SyntaxTree);

                if (semanticModel.GetDeclaredSymbol(syntaxNode) is not ITypeSymbol typeSymbol
                 || !IsValidService(typeSymbol, out INamedTypeSymbol interfaceSymbol))
                {
                    continue;
                }

                bool disableDefaultFactory = typeSymbol.HasAttribute(CoimbraServicesTypes.DisableDefaultFactoryAttribute, out _, false);
                bool preloadService = typeSymbol.HasAttribute(CoimbraServicesTypes.PreloadServiceAttribute, out _, false);

                if (disableDefaultFactory && !preloadService)
                {
                    continue;
                }

                if (typeSymbol.InheritsFrom(CoimbraTypes.ActorClass))
                {
                    yield return new TypeData
                    {
                        ClassSymbol = typeSymbol,
                        InterfaceSymbol = interfaceSymbol,
                        DisableDefaultFactory = disableDefaultFactory,
                        IsActor = true,
                        PreloadService = preloadService,
                    };
                }
                else if (!typeSymbol.InheritsFrom(UnityEngineTypes.ComponentClass) && syntaxNode.HasParameterlessConstructor(out bool isPublic) && isPublic)
                {
                    yield return new TypeData
                    {
                        ClassSymbol = typeSymbol,
                        InterfaceSymbol = interfaceSymbol,
                        DisableDefaultFactory = disableDefaultFactory,
                        IsActor = false,
                        PreloadService = preloadService,
                    };
                }
            }
        }

        private static bool IsValidService(ITypeSymbol typeSymbol, out INamedTypeSymbol concreteInterfaceSymbol)
        {
            concreteInterfaceSymbol = null;

            foreach (INamedTypeSymbol interfaceSymbol in typeSymbol.Interfaces)
            {
                if (!interfaceSymbol.IsOrImplementsInterface(CoimbraServicesTypes.ServiceInterface))
                {
                    continue;
                }

                if (interfaceSymbol.HasAttribute(CoimbraServicesTypes.AbstractServiceAttribute, out _, false))
                {
                    concreteInterfaceSymbol = null;

                    return false;
                }

                if (concreteInterfaceSymbol != null)
                {
                    return false;
                }

                concreteInterfaceSymbol = interfaceSymbol;
            }

            return concreteInterfaceSymbol != null;
        }
    }
}
