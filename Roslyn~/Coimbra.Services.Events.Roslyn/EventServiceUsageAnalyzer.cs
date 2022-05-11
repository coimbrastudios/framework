using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Coimbra.Services.Events.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventServiceUsageAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.EventServiceGenericAPIsSholdNotBeUsed);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeEventServiceUsage, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeEventServiceUsage(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocationExpressionSyntax)
            {
                return;
            }

            if (context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is not IMethodSymbol methodSymbol
             || !methodSymbol.ContainingType.IsOrImplementsInterface(CoimbraServicesEventsTypes.EventServiceInterface, CoimbraServicesEventsTypes.Namespace)
             || !methodSymbol.IsGenericMethod
             || methodSymbol.DeclaredAccessibility != Accessibility.Public)
            {
                return;
            }

            SimpleNameSyntax methodNameSyntax = invocationExpressionSyntax.GetMethodNameSyntax();

            if (methodNameSyntax == null)
            {
                return;
            }

            ITypeSymbol typeSymbol = methodSymbol.TypeArguments.First();
            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.EventServiceGenericAPIsSholdNotBeUsed, methodNameSyntax.GetLocation(), typeSymbol.Name, methodNameSyntax.Identifier.Text));
        }
    }
}
