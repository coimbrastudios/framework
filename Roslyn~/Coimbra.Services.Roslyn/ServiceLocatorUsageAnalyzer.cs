using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Coimbra.Services.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ServiceLocatorUsageAnalyzer : DiagnosticAnalyzer
    {
        private static readonly char[] SplitChars =
        {
            '.'
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.ServiceLocatorRequiresInterface,
                                                                                                           Diagnostics.ServiceLocatorRequiresNonAbstractInterface,
                                                                                                           Diagnostics.OwningLocatorShouldBeUsedInsteadOfShared,
                                                                                                           Diagnostics.OwningLocatorSetterIsForInternalUseOnly);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeServiceLocatorClassUsage, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeServiceLocatorSetterUsage, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeServiceLocatorSharedUsage, SyntaxKind.SimpleMemberAccessExpression);
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

        private static void AnalyzeServiceLocatorSetterUsage(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not AssignmentExpressionSyntax assignmentExpressionSyntax
             || assignmentExpressionSyntax.Left.ToString().Split(SplitChars, StringSplitOptions.RemoveEmptyEntries).Last() != "OwningLocator"
             || context.SemanticModel.GetSymbolInfo(assignmentExpressionSyntax.Left).Symbol is not IPropertySymbol propertySymbol
             || !propertySymbol.ContainingType.IsOrImplementsInterface(CoimbraServicesTypes.ServiceInterface))
            {
                return;
            }

            TypeDeclarationSyntax typeDeclarationSyntax = assignmentExpressionSyntax.FirstAncestorOrSelf<TypeDeclarationSyntax>(_ => true);

            if (typeDeclarationSyntax != null
             && context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is { } typeSymbol
             && typeSymbol.Is(CoimbraServicesTypes.ServiceLocatorClass))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.OwningLocatorSetterIsForInternalUseOnly, assignmentExpressionSyntax.GetLocation()));
        }

        private static void AnalyzeServiceLocatorSharedUsage(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not MemberAccessExpressionSyntax memberAccessExpression
             || memberAccessExpression.Name.Identifier.Text != "Shared"
             || context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol is not IFieldSymbol fieldSymbol
             || !fieldSymbol.ContainingType.Is(CoimbraServicesTypes.ServiceLocatorClass))
            {
                return;
            }

            foreach (SyntaxNode ancestor in memberAccessExpression.Ancestors())
            {
                switch (ancestor)
                {
                    case LocalFunctionStatementSyntax localFunctionStatementSyntax when localFunctionStatementSyntax.Modifiers.Any(SyntaxKind.StaticKeyword):
                    case MemberDeclarationSyntax memberDeclarationSyntax when memberDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword):
                        return;

                    case TypeDeclarationSyntax typeDeclarationSyntax:
                    {
                        if (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword)
                         || context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is not { } typeSymbol
                         || !typeSymbol.IsOrImplementsInterface(CoimbraServicesTypes.ServiceInterface))
                        {
                            return;
                        }

                        context.ReportDiagnostic(Diagnostic.Create(Diagnostics.OwningLocatorShouldBeUsedInsteadOfShared, memberAccessExpression.GetLocation()));

                        break;
                    }
                }
            }
        }
    }
}
