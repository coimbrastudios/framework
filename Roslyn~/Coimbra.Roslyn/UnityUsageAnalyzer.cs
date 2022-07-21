using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Coimbra.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UnityUsageAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CoimbraDiagnostics.ObjectDestroyShouldNotBeUsed);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeUnityUsage, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeUnityUsage(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocationExpressionSyntax
             || context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is not IMethodSymbol methodSymbol
             || !methodSymbol.ContainingType.Is(UnityEngineTypes.ObjectClass)
             || methodSymbol.Name != "Destroy"
             || invocationExpressionSyntax.ArgumentList.Arguments.Count == 0)
            {
                return;
            }

            TypeInfo typeInfo = context.SemanticModel.GetTypeInfo(invocationExpressionSyntax.ArgumentList.Arguments[0].Expression);

            if (typeInfo.Type.Is(UnityEngineTypes.ObjectClass) || typeInfo.Type.Is(UnityEngineTypes.GameObjectClass) || typeInfo.Type.IsAssignableTo(CoimbraTypes.ActorClass))
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.ObjectDestroyShouldNotBeUsed, invocationExpressionSyntax.GetLocation(), invocationExpressionSyntax.ArgumentList.Arguments[0]));
            }
        }
    }
}
