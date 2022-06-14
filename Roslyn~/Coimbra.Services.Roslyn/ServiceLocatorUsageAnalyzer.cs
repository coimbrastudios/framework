using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Coimbra.Services.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ServiceLocatorUsageAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.ServiceLocatorRequiresInterface,
                                                                                                           Diagnostics.ServiceLocatorRequiresNonAbstractInterface);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeServiceLocatorClassUsage, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeServiceLocatorClassUsage(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocationExpressionSyntax)
            {
                return;
            }

            if (context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is not IMethodSymbol methodSymbol
             || !methodSymbol.ContainingType.Is(CoimbraServicesTypes.ServiceLocatorClass)
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

            if (typeSymbol.TypeKind is not TypeKind.Interface)
            {
                if (typeSymbol.TypeKind is not TypeKind.TypeParameter)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ServiceLocatorRequiresInterface, methodNameSyntax.GetLocation(), methodNameSyntax.Identifier.Text, typeSymbol.Name));
                }
            }
            else if (typeSymbol.HasAttribute(CoimbraServicesTypes.AbstractServiceAttribute, out _, false))
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ServiceLocatorRequiresNonAbstractInterface, methodNameSyntax.GetLocation(), methodNameSyntax.Identifier.Text, typeSymbol.Name));
            }
        }
    }
}
