using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Coimbra.Services.Events.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EventDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.ConcreteEventShouldBePartial, Diagnostics.ConcreteEventShouldNotBeNested);

        public override void Initialize(AnalysisContext context)
        {
            static bool predicate(INamedTypeSymbol x)
            {
                return x.Name == "IEvent" && x.ContainingNamespace.ToString() == "Coimbra.Services.Events";
            }

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(x => AnalyzeObjectCreation<ClassDeclarationSyntax>(x, predicate), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(x => AnalyzeObjectCreation<StructDeclarationSyntax>(x, predicate), SyntaxKind.StructDeclaration);
        }

        private static void AnalyzeObjectCreation<T>(SyntaxNodeAnalysisContext context, Func<INamedTypeSymbol, bool> interfacePredicate)
            where T : TypeDeclarationSyntax
        {
            if (!(context.Node is T typeDeclarationSyntax)
             || typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)
             || !(context.SemanticModel.GetDeclaredSymbol(context.Node) is INamedTypeSymbol typeSymbol)
             || typeSymbol.AllInterfaces.FirstOrDefault(interfacePredicate) == null)
            {
                return;
            }

            if (typeDeclarationSyntax.Parent is TypeDeclarationSyntax parentTypeNode)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ConcreteEventShouldNotBeNested, typeDeclarationSyntax.GetLocation(), typeDeclarationSyntax.GetTypeName(), parentTypeNode.GetTypeName()));
            }

            if (!typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ConcreteEventShouldBePartial, typeDeclarationSyntax.GetLocation(), typeDeclarationSyntax.GetTypeName()));
            }
        }
    }
}
