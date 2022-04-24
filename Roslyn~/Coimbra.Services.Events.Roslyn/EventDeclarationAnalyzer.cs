using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Coimbra.Services.Events.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EventDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.ConcreteEventShouldBePartial,
                                                                                                           Diagnostics.ConcreteEventShouldNotBeNested,
                                                                                                           Diagnostics.ConcreteEventParameterlessCtorShouldBePublic);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeEventDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
        }

        private static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not TypeDeclarationSyntax typeDeclarationSyntax
             || typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)
             || context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol typeSymbol
             || !typeSymbol.ImplementsInterface("IEvent", "Coimbra.Services.Events", true))
            {
                return;
            }

            bool reportError = true;

            if (typeDeclarationSyntax.Parent is TypeDeclarationSyntax parentTypeNode)
            {
                reportError = false;
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ConcreteEventShouldNotBeNested, typeDeclarationSyntax.Identifier.GetLocation(), typeDeclarationSyntax.GetTypeName(), parentTypeNode.GetTypeName()));
            }

            if (!typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                reportError = false;
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ConcreteEventShouldBePartial, typeDeclarationSyntax.Identifier.GetLocation(), typeDeclarationSyntax.GetTypeName()));
            }

            if (reportError && typeDeclarationSyntax.HasParameterlessConstructor(out bool isPublic) && !isPublic)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ConcreteEventParameterlessCtorShouldBePublic, typeDeclarationSyntax.Identifier.GetLocation(), typeDeclarationSyntax.GetTypeName()));
            }
        }
    }
}
