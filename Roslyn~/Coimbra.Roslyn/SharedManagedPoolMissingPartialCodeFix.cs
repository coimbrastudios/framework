using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Immutable;

namespace Coimbra.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public sealed class SharedManagedPoolMissingPartialCodeFix : AddPartialKeywordCodeFix
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CoimbraDiagnostics.SharedManagedPoolRequiresPartialKeyword.Id);
    }
}
