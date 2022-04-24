using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Coimbra.Services.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ServiceDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.ConcreteServiceShouldOnlyImplementOneService,
                                                                                                           Diagnostics.ConcreteServiceShouldNotImplementAbstractService);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeServiceDeclaration, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeServiceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax
             || classDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)
             || context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            int abstractServiceCount = 0;
            int concreteServiceCount = 0;

            foreach (INamedTypeSymbol interfaceSymbol in typeSymbol.Interfaces)
            {
                if (!interfaceSymbol.IsOrImplementsInterface(CoimbraServicesTypes.ServiceInterface, CoimbraServicesTypes.Namespace))
                {
                    continue;
                }

                if (interfaceSymbol.HasAttribute(CoimbraServicesTypes.AbstractServiceAttribute, CoimbraServicesTypes.Namespace))
                {
                    abstractServiceCount++;
                }
                else
                {
                    concreteServiceCount++;
                }
            }

            if (abstractServiceCount > 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ConcreteServiceShouldNotImplementAbstractService, classDeclarationSyntax.BaseList!.GetLocation(), classDeclarationSyntax.GetTypeName()));
            }

            if (concreteServiceCount > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ConcreteServiceShouldOnlyImplementOneService, classDeclarationSyntax.BaseList!.GetLocation(), classDeclarationSyntax.GetTypeName()));
            }
        }
    }
}
