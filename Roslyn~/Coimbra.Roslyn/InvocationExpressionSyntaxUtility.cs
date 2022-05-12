using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Coimbra.Roslyn
{
    public static class InvocationExpressionSyntaxUtility
    {
        public static SimpleNameSyntax GetMethodNameSyntax(this InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.Expression switch
            {
                MemberAccessExpressionSyntax memberAccessExpressionSyntax => memberAccessExpressionSyntax.Name,
                SimpleNameSyntax simpleNameSyntax => simpleNameSyntax,
                MemberBindingExpressionSyntax memberBindingExpressionSyntax => memberBindingExpressionSyntax.Name,
                _ => null,
            };
        }
    }
}
