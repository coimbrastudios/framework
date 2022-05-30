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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.SharedManagedPoolRequiresPartialKeyword,
                                                                                                           Diagnostics.SharedManagedPoolDoesntSupportNestedTypes,
                                                                                                           Diagnostics.SharedManagedPoolDoesntSupportGenericTypes);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeSharedManagedPoolUsage, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
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
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.SharedManagedPoolDoesntSupportNestedTypes, typeDeclaration.Identifier.GetLocation(), typeDeclaration.GetTypeName(), outerDeclaration.GetTypeName()));
            }

            if (!typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.SharedManagedPoolRequiresPartialKeyword, typeDeclaration.Identifier.GetLocation(), typeDeclaration.GetTypeName()));
            }

            if (typeDeclaration.TypeParameterList != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.SharedManagedPoolDoesntSupportGenericTypes, typeDeclaration.Identifier.GetLocation(), typeDeclaration.GetTypeName()));
            }
        }
    }
}
