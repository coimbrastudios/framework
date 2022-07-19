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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CoimbraServicesEventsDiagnostics.ConcreteEventShouldBePartial,
                                                                                                           CoimbraServicesEventsDiagnostics.ConcreteEventShouldNotBeNested,
                                                                                                           CoimbraServicesEventsDiagnostics.ClassEventShouldBeEitherAbstractOrSealed);

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
             || !typeSymbol.ImplementsInterface(CoimbraServicesEventsTypes.EventInterface))
            {
                return;
            }

            if (typeDeclarationSyntax.Parent is TypeDeclarationSyntax parentTypeNode)
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraServicesEventsDiagnostics.ConcreteEventShouldNotBeNested, typeDeclarationSyntax.Identifier.GetLocation(), typeDeclarationSyntax.GetTypeName(), parentTypeNode.GetTypeName()));
            }

            if (!typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraServicesEventsDiagnostics.ConcreteEventShouldBePartial, typeDeclarationSyntax.Identifier.GetLocation(), typeDeclarationSyntax.GetTypeName()));
            }

            if (typeDeclarationSyntax is ClassDeclarationSyntax && !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.SealedKeyword) && !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraServicesEventsDiagnostics.ClassEventShouldBeEitherAbstractOrSealed, typeDeclarationSyntax.Identifier.GetLocation(), typeDeclarationSyntax.GetTypeName()));
            }
        }
    }
}
