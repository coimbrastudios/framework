using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Coimbra.Services.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AbstractServiceUsageAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.AbstractServiceShouldBeUsedWithServiceInterfaces);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeAbstractServiceUsage, SyntaxKind.InterfaceDeclaration);
        }

        private static void AnalyzeAbstractServiceUsage(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InterfaceDeclarationSyntax interfaceDeclarationSyntax
             || context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol typeSymbol
             || !typeSymbol.HasAttribute(CoimbraServicesTypes.AbstractServiceAttribute, CoimbraServicesTypes.Namespace, out _))
            {
                return;
            }

            if (!typeSymbol.IsOrImplementsInterface(CoimbraServicesTypes.ServiceInterface, CoimbraServicesTypes.Namespace))
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.AbstractServiceShouldBeUsedWithServiceInterfaces, interfaceDeclarationSyntax.Identifier.GetLocation(), interfaceDeclarationSyntax.GetTypeName()));
            }
        }
    }
}
