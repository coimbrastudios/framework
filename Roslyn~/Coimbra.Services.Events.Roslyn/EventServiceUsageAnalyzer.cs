using Coimbra.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Coimbra.Services.Events.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EventServiceUsageAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CoimbraServicesEventsDiagnostics.EventServiceGenericMethodsShouldNotBeUsedDirectly);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeEventServiceUsage, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeEventServiceUsage(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocation)
            {
                return;
            }

            if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol { DeclaredAccessibility: Accessibility.Public, IsGenericMethod: true } method
             || !method.ContainingType.Is(CoimbraServicesEventsTypes.EventServiceInterface))
            {
                return;
            }

            SimpleNameSyntax methodName = invocation.GetMethodNameSyntax();

            if (methodName == null)
            {
                return;
            }

            ITypeSymbol methodTypeParameter = method.TypeArguments.First();

            static bool predicate(TypeDeclarationSyntax typeDeclaration)
            {
                return typeDeclaration is ClassDeclarationSyntax or StructDeclarationSyntax;
            }

            if (invocation.FirstAncestorOrSelf<TypeDeclarationSyntax>(predicate) is not { } callerTypeDeclaration
             || context.SemanticModel.GetDeclaredSymbol(callerTypeDeclaration) is not { } callerType
             || !IsEventServiceUsageAllowedRecursive(callerType, methodTypeParameter))
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraServicesEventsDiagnostics.EventServiceGenericMethodsShouldNotBeUsedDirectly, invocation.GetLocation(), methodTypeParameter.Name, methodName.Identifier.Text));
            }
        }

        private static bool IsEventServiceUsageAllowedRecursive(ITypeSymbol callerType, ITypeSymbol methodTypeParameter)
        {
            if (methodTypeParameter is ITypeParameterSymbol genericType)
            {
                return IsGenericMethodAllowed(callerType, genericType);
            }

            if (methodTypeParameter.HasAttribute(CoimbraServicesEventsTypes.AllowEventServiceUsageForAttribute, out AttributeData attributeData, true)
             && attributeData.ConstructorArguments.Length > 0
             && attributeData.ConstructorArguments[0].Value is INamedTypeSymbol allowedType)
            {
                if (attributeData.ConstructorArguments.Length == 1
                 || attributeData.ConstructorArguments[1].Value is bool and false)
                {
                    if (callerType.IsAssignableTo(TypeString.From(allowedType)))
                    {
                        return true;
                    }
                }
                else if (callerType.Is(TypeString.From(allowedType)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsGenericMethodAllowed(ITypeSymbol callerType, ITypeParameterSymbol genericType)
        {
            foreach (ITypeSymbol constraintType in genericType.ConstraintTypes)
            {
                if (IsEventServiceUsageAllowedRecursive(callerType, constraintType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
