using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Coimbra.Roslyn
{
    public static class InvocationExpressionSyntaxUtility
    {
        public static SimpleNameSyntax GetMethodNameSyntax(this InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.Expression switch
            {
                MemberAccessExpressionSyntax memberAccess => memberAccess.Name,
                SimpleNameSyntax simpleName => simpleName,
                MemberBindingExpressionSyntax memberBinding => memberBinding.Name,
                _ => null,
            };
        }
    }
}
