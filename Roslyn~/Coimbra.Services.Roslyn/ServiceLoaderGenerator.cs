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

        private static readonly string GeneratedCodeAttribute = $"[{SystemTypes.GeneratedCodeAttribute.FullName}(\"{CoimbraServicesTypes.Namespace}.Roslyn.{nameof(ServiceLoaderGenerator)}\", \"1.0.0.0\")]";

        private static readonly SymbolDisplayFormat QualifiedNameOnlyFormat = new(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, SymbolDisplayGenericsOptions.IncludeTypeParameters);

        public void Execute(GeneratorExecutionContext context)
        {
            SourceBuilder sourceBuilder = new();

            foreach (TypeData typeData in EnumerateTypes(context))
            {
                string qualifiedInterfaceName = typeData.InterfaceSymbol.ToDisplayString(QualifiedNameOnlyFormat);
                TypeString source = new($"{typeData.ClassSymbol.Name}Loader", typeData.ClassSymbol.ContainingNamespace?.ToString() ?? string.Empty);
                sourceBuilder.Initialize();

                using (new NamespaceScope(sourceBuilder, source.Namespace))
                {
                    sourceBuilder.AddLine(GeneratedCodeAttribute);
                    sourceBuilder.AddLine($"internal static class {source.Name}");

                    using (new BracesScope(sourceBuilder))
                    {
                        if (!typeData.DisableDefaultFactory)
                        {
                            sourceBuilder.AddLine(GeneratedCodeAttribute);
                            sourceBuilder.AddLine($"[{UnityEngineTypes.RuntimeInitializeOnLoadMethodAttribute.FullName}({UnityEngineTypes.RuntimeInitializeLoadTypeEnum.FullName}.SubsystemRegistration)]");
                            sourceBuilder.AddLine("private static void HandleSubsystemRegistration()");

                            using (new BracesScope(sourceBuilder))
                            {
                                sourceBuilder.AddLine($"if (!{CoimbraServicesTypes.ServiceLocatorClass.FullName}.HasFactory<{qualifiedInterfaceName}>())");

                                using (new BracesScope(sourceBuilder))
                                {
                                    TypeString factory = typeData.IsActor ? CoimbraServicesTypes.DefaultServiceActorFactoryClass : CoimbraServicesTypes.DefaultServiceFactoryClass;
                                    sourceBuilder.AddLine($"{CoimbraServicesTypes.ServiceLocatorClass.FullName}.SetFactory<{qualifiedInterfaceName}>({factory.FullName}<{typeData.ClassSymbol.ToDisplayString(QualifiedNameOnlyFormat)}>.Instance);");
                                }
                            }

                            if (typeData.PreloadService)
                            {
                                sourceBuilder.SkipLine();
                            }
                        }

                        if (typeData.PreloadService)
                        {
                            sourceBuilder.AddLine(GeneratedCodeAttribute);
                            sourceBuilder.AddLine($"[{UnityEngineTypes.RuntimeInitializeOnLoadMethodAttribute.FullName}({UnityEngineTypes.RuntimeInitializeLoadTypeEnum.FullName}.BeforeSceneLoad)]");
                            sourceBuilder.AddLine("private static void HandleBeforeSceneLoad()");

                            using (new BracesScope(sourceBuilder))
                            {
                                if (typeData.InterfaceSymbol.HasAttribute(CoimbraServicesTypes.RequiredServiceAttribute, out _, false))
                                {
                                    sourceBuilder.AddLine($"{CoimbraServicesTypes.ServiceLocatorClass.FullName}.GetChecked<{qualifiedInterfaceName}>();");
                                }
                                else
                                {
                                    sourceBuilder.AddLine($"{CoimbraServicesTypes.ServiceLocatorClass.FullName}.Get<{qualifiedInterfaceName}>();");
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
