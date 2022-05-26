using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Coimbra.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ScriptableSettingsDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostics.ScriptableSettingsHasConflictingAttributes,
                                                                                                           Diagnostics.ScriptableSettingsFileDirectoryIsInvalid,
                                                                                                           Diagnostics.ScriptableSettingsShouldNotBeAbstract,
                                                                                                           Diagnostics.ScriptableSettingsShouldNotBeGeneric);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeServiceDeclaration, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeServiceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax
             || context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol typeSymbol
             || !typeSymbol.InheritsFrom(CoimbraTypes.ScriptableSettingsClass))
            {
                return;
            }

            bool isPreferences = typeSymbol.HasAttribute(CoimbraTypes.PreferencesAttribute, out AttributeData preferencesData);
            bool isProjectSettings = typeSymbol.HasAttribute(CoimbraTypes.ProjectSettingsAttribute, out AttributeData projectSettingsData);

            if (!isPreferences && !isProjectSettings)
            {
                return;
            }

            if (classDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                if (isPreferences)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ScriptableSettingsShouldNotBeAbstract,
                                                               classDeclarationSyntax.BaseList!.GetLocation(),
                                                               CoimbraTypes.PreferencesAttribute.Name,
                                                               classDeclarationSyntax.GetTypeName()));
                }

                if (isProjectSettings)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ScriptableSettingsShouldNotBeAbstract,
                                                               classDeclarationSyntax.BaseList!.GetLocation(),
                                                               CoimbraTypes.ProjectSettingsAttribute.Name,
                                                               classDeclarationSyntax.GetTypeName()));
                }

                return;
            }

            if (typeSymbol.IsGenericType)
            {
                if (isPreferences)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ScriptableSettingsShouldNotBeGeneric,
                                                               classDeclarationSyntax.BaseList!.GetLocation(),
                                                               CoimbraTypes.PreferencesAttribute.Name,
                                                               classDeclarationSyntax.GetTypeName()));
                }

                if (isProjectSettings)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ScriptableSettingsShouldNotBeGeneric,
                                                               classDeclarationSyntax.BaseList!.GetLocation(),
                                                               CoimbraTypes.ProjectSettingsAttribute.Name,
                                                               classDeclarationSyntax.GetTypeName()));
                }

                return;
            }

            if (isPreferences)
            {
                if (isProjectSettings)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ScriptableSettingsHasConflictingAttributes, classDeclarationSyntax.BaseList!.GetLocation(), classDeclarationSyntax.GetTypeName()));
                }
                else if (!IsAttributeDataValid(preferencesData, out string reason))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ScriptableSettingsFileDirectoryIsInvalid,
                                                               classDeclarationSyntax.BaseList!.GetLocation(),
                                                               CoimbraTypes.PreferencesAttribute.Name,
                                                               classDeclarationSyntax.GetTypeName(),
                                                               reason));
                }
            }
            else if (!IsAttributeDataValid(projectSettingsData, out string reason))
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ScriptableSettingsFileDirectoryIsInvalid,
                                                           classDeclarationSyntax.BaseList!.GetLocation(),
                                                           CoimbraTypes.ProjectSettingsAttribute.Name,
                                                           classDeclarationSyntax.GetTypeName(),
                                                           reason));
            }
        }

        private static bool IsAttributeDataValid(AttributeData attributeData, out string reason)
        {
            if (attributeData.NamedArguments.ToImmutableDictionary().TryGetValue("FileDirectory", out TypedConstant value) && value.Value is string s)
            {
                s = s.Replace("\\", "/");

                if (s == "Assets" || s.StartsWith("Assets/"))
                {
                    reason = "be inside the Assets folder";

                    return false;
                }

                if (s == ".." || s.StartsWith("../") || s.EndsWith("/..") || s.Contains("/../"))
                {
                    reason = "contains \"..\" in the path";

                    return false;
                }
            }

            reason = null;

            return true;
        }
    }
}
