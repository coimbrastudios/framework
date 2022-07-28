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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CoimbraServicesDiagnostics.ServiceLocatorRequiresInterface,
                                                                                                           CoimbraServicesDiagnostics.ServiceLocatorRequiresNonAbstractInterface,
                                                                                                           CoimbraServicesDiagnostics.MissingRequiredServiceAttribute,
                                                                                                           CoimbraServicesDiagnostics.SetBeingUsedInNonDynamicService,
                                                                                                           CoimbraServicesDiagnostics.UseGetCheckedWithRequiredService);

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
                    context.ReportDiagnostic(Diagnostic.Create(CoimbraServicesDiagnostics.ServiceLocatorRequiresInterface, methodNameSyntax.GetLocation(), methodNameSyntax.Identifier.Text, typeSymbol.Name));

                    return;
                }
            }
            else if (typeSymbol.HasAttribute(CoimbraServicesTypes.AbstractServiceAttribute, out _, false))
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraServicesDiagnostics.ServiceLocatorRequiresNonAbstractInterface, methodNameSyntax.GetLocation(), methodNameSyntax.Identifier.Text, typeSymbol.Name));

                return;
            }

            AnalyzeTypeArgument(context, typeSymbol, methodNameSyntax);
        }

        private static void AnalyzeTypeArgument(SyntaxNodeAnalysisContext context, ITypeSymbol typeSymbol, SimpleNameSyntax methodNameSyntax)
        {
            switch (methodNameSyntax.Identifier.Text)
            {
                case "Get":
                case "TryGet":
                {
                    if (typeSymbol.HasAttribute(CoimbraServicesTypes.RequiredServiceAttribute, out _, false))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(CoimbraServicesDiagnostics.UseGetCheckedWithRequiredService, methodNameSyntax.GetLocation(), typeSymbol.Name));
                    }

                    break;
                }

                case "GetChecked":
                {
                    if (!typeSymbol.HasAttribute(CoimbraServicesTypes.RequiredServiceAttribute, out _, false))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(CoimbraServicesDiagnostics.MissingRequiredServiceAttribute, methodNameSyntax.GetLocation(), typeSymbol.Name));
                    }

                    break;
                }

                case "Set":
                {
                    if (!typeSymbol.HasAttribute(CoimbraServicesTypes.DynamicServiceAttribute, out _, false))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(CoimbraServicesDiagnostics.SetBeingUsedInNonDynamicService, methodNameSyntax.GetLocation(), typeSymbol.Name));
                    }

                    break;
                }
            }
        }
    }
}
