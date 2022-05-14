using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Coimbra.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ManagedPoolUsageAnalyzer : DiagnosticAnalyzer
    {
        private static readonly HashSet<string> SpecificTypes = new()
        {
            "Dictionary",
            "HashSet",
            "List",
            "Queue",
            "Stack",
            "StringBuilder",
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.SpecifiedTypeShouldBeUsedWithAnotherSharedPool);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeServiceLocatorUsage, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeServiceLocatorUsage(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocationExpressionSyntax)
            {
                return;
            }

            if (context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is not IMethodSymbol methodSymbol
             || methodSymbol.ContainingType.IsGenericType
             || !methodSymbol.ContainingType.Is(CoimbraTypes.ManagedPoolClass, CoimbraTypes.Namespace)
             || !methodSymbol.IsGenericMethod)
            {
                return;
            }

            SimpleNameSyntax methodNameSyntax = invocationExpressionSyntax.GetMethodNameSyntax();

            if (methodNameSyntax == null)
            {
                return;
            }

            ITypeSymbol typeSymbol = methodSymbol.TypeArguments.First();

            if (SpecificTypes.Contains(typeSymbol.Name))
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.SpecifiedTypeShouldBeUsedWithAnotherSharedPool, methodNameSyntax.GetLocation(), typeSymbol.Name));
            }
        }
    }
}
