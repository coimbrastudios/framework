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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CoimbraDiagnostics.ScriptableSettingsHasConflictingAttributes,
                                                                                                           CoimbraDiagnostics.ScriptableSettingsFileDirectoryIsInvalid,
                                                                                                           CoimbraDiagnostics.ScriptableSettingsShouldNotBeAbstract,
                                                                                                           CoimbraDiagnostics.ScriptableSettingsShouldNotBeGeneric);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeServiceDeclaration, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeServiceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ClassDeclarationSyntax classDeclaration
             || context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol classType
             || !classType.InheritsFrom(CoimbraTypes.ScriptableSettingsClass))
            {
                return;
            }

            bool hasPreferencesAttribute = classType.HasAttribute(CoimbraTypes.PreferencesAttribute, out AttributeData preferencesAttribute, false);
            bool hasProjectSettingsAttribute = classType.HasAttribute(CoimbraTypes.ProjectSettingsAttribute, out AttributeData projectSettingsAttribute, false);

            if (!hasPreferencesAttribute && !hasProjectSettingsAttribute)
            {
                return;
            }

            if (classDeclaration.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                if (hasPreferencesAttribute)
                {
                    context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.ScriptableSettingsShouldNotBeAbstract,
                                                               classDeclaration.BaseList!.GetLocation(),
                                                               CoimbraTypes.PreferencesAttribute.Name,
                                                               classDeclaration.GetTypeName()));
                }

                if (hasProjectSettingsAttribute)
                {
                    context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.ScriptableSettingsShouldNotBeAbstract,
                                                               classDeclaration.BaseList!.GetLocation(),
                                                               CoimbraTypes.ProjectSettingsAttribute.Name,
                                                               classDeclaration.GetTypeName()));
                }

                return;
            }

            if (classType.IsGenericType)
            {
                if (hasPreferencesAttribute)
                {
                    context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.ScriptableSettingsShouldNotBeGeneric,
                                                               classDeclaration.BaseList!.GetLocation(),
                                                               CoimbraTypes.PreferencesAttribute.Name,
                                                               classDeclaration.GetTypeName()));
                }

                if (hasProjectSettingsAttribute)
                {
                    context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.ScriptableSettingsShouldNotBeGeneric,
                                                               classDeclaration.BaseList!.GetLocation(),
                                                               CoimbraTypes.ProjectSettingsAttribute.Name,
                                                               classDeclaration.GetTypeName()));
                }

                return;
            }

            if (hasPreferencesAttribute)
            {
                if (hasProjectSettingsAttribute)
                {
                    context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.ScriptableSettingsHasConflictingAttributes, classDeclaration.BaseList!.GetLocation(), classDeclaration.GetTypeName()));
                }
                else if (HasInvalidFileDirectory(preferencesAttribute, out string shouldNotMessage))
                {
                    context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.ScriptableSettingsFileDirectoryIsInvalid,
                                                               classDeclaration.BaseList!.GetLocation(),
                                                               CoimbraTypes.PreferencesAttribute.Name,
                                                               classDeclaration.GetTypeName(),
                                                               shouldNotMessage));
                }
            }
            else if (HasInvalidFileDirectory(projectSettingsAttribute, out string shouldNotMessage))
            {
                context.ReportDiagnostic(Diagnostic.Create(CoimbraDiagnostics.ScriptableSettingsFileDirectoryIsInvalid,
                                                           classDeclaration.BaseList!.GetLocation(),
                                                           CoimbraTypes.ProjectSettingsAttribute.Name,
                                                           classDeclaration.GetTypeName(),
                                                           shouldNotMessage));
            }
        }

        private static bool HasInvalidFileDirectory(AttributeData attribute, out string shouldNot)
        {
            if (attribute.NamedArguments.ToImmutableDictionary().TryGetValue("FileDirectory", out TypedConstant value) && value.Value is string s)
            {
                s = s.Replace("\\", "/");

                if (s == "Assets" || s.StartsWith("Assets/"))
                {
                    shouldNot = "be inside the Assets folder";

                    return true;
                }

                if (s == ".." || s.StartsWith("../") || s.EndsWith("/..") || s.Contains("/../"))
                {
                    shouldNot = "contains \"..\" in the path";

                    return true;
                }
            }

            shouldNot = null;

            return false;
        }
    }
}
