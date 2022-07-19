using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Coimbra.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SharedManagedPoolUsageAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CoimbraDiagnostics.SharedManagedPoolRequiresPartialKeyword,
                                                                                                           CoimbraDiagnostics.SharedManagedPoolDoesntSupportNestedTypes,
                                                                                                           CoimbraDiagnostics.SharedManagedPoolDoesntSupportGenericTypes,
                                                                                                           CoimbraDiagnostics.SharedManagedPoolRequiresToUseCreateShared,
                                                                                                           CoimbraDiagnostics.DoNotUseCreateSharedOutsideFromSharedManagedPool);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeSharedManagedPoolUsage, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeManagedPoolCreation, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeCreateSharedInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeCreateSharedInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocation
             || context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol
             || !methodSymbol.ContainingType.IsGenericType
             || !methodSymbol.ContainingType.Is(CoimbraTypes.ManagedPoolClass))
            {
                return;
            }

            SimpleNameSyntax methodNameSyntax = invocation.GetMethodNameSyntax();

            if (methodNameSyntax == null || methodNameSyntax.ToString() != "CreateShared")
            {
                return;
            }

            for (SyntaxNode current = invocation.Parent; current is not NamespaceDeclarationSyntax && current != null; current = current.Parent)
            {
                if (current is TypeDeclarationSyntax typeDeclaration && context.SemanticModel.GetDeclaredSymbol(typeDeclaration) is { } typeSymbol && typeSymbol.HasAttribute(CoimbraTypes.SharedManagedPoolAttribute, out _, false))
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.DoNotUseCreateSharedOutsideFromSharedManagedPool, invocation.GetLocation(), methodSymbol.ContainingType));
        }

        private void AnalyzeManagedPoolCreation(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ObjectCreationExpressionSyntax objectCreation)
            {
                return;
            }

            TypeInfo typeInfo = context.SemanticModel.GetTypeInfo(objectCreation);

            if (typeInfo.Type == null || !typeInfo.Type.Is(CoimbraTypes.ManagedPoolClass))
            {
                return;
            }

            for (SyntaxNode current = objectCreation.Parent; current is not NamespaceDeclarationSyntax && current != null; current = current.Parent)
            {
                if (current is not TypeDeclarationSyntax typeDeclaration || context.SemanticModel.GetDeclaredSymbol(typeDeclaration) is not { } typeSymbol || !typeSymbol.HasAttribute(CoimbraTypes.SharedManagedPoolAttribute, out _, false))
                {
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.SharedManagedPoolRequiresToUseCreateShared, objectCreation.GetLocation(), objectCreation.Type));

                return;
            }
        }

        private static void AnalyzeSharedManagedPoolUsage(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not TypeDeclarationSyntax typeDeclaration
             || context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol typeSymbol
             || !typeSymbol.HasAttribute(CoimbraTypes.SharedManagedPoolAttribute, out _, false))
            {
                return;
            }

            if (typeDeclaration.Parent is TypeDeclarationSyntax outerDeclaration)
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.SharedManagedPoolDoesntSupportNestedTypes, typeDeclaration.Identifier.GetLocation(), typeDeclaration.GetTypeName(), outerDeclaration.GetTypeName()));
            }

            if (!typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.SharedManagedPoolRequiresPartialKeyword, typeDeclaration.Identifier.GetLocation(), typeDeclaration.GetTypeName()));
            }

            if (typeDeclaration.TypeParameterList != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.SharedManagedPoolDoesntSupportGenericTypes, typeDeclaration.Identifier.GetLocation(), typeDeclaration.GetTypeName()));
            }
        }
    }
}
