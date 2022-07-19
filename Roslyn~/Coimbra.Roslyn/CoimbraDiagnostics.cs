using Microsoft.CodeAnalysis;
using System.ComponentModel;

#pragma warning disable RS2008

namespace Coimbra.Roslyn
{
    public static class CoimbraDiagnostics
    {
        private const string Category = CoimbraTypes.Namespace;

        public static readonly DiagnosticDescriptor SharedManagedPoolDoesntSupportGenericTypes = new("COIMBRA" + "0001",
                                                                                                     "Type with SharedManagedPoolAttribute should not be generic.",
                                                                                                     "Remove SharedManagedPoolAttribute from {0} or make it non-generic.",
                                                                                                     Category,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true);

        public static readonly DiagnosticDescriptor ScriptableSettingsHasConflictingAttributes = new("COIMBRA" + "0002",
                                                                                                     "ProjectSettingsAttribute and PreferencesAttribute should not be used together.",
                                                                                                     "Remove either ProjectSettingsAttribute or PreferencesAttribute from {0}",
                                                                                                     Category,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true);

        public static readonly DiagnosticDescriptor ScriptableSettingsFileDirectoryIsInvalid = new("COIMBRA" + "0003",
                                                                                                   "ScriptableSettings has an invalided FileDirectory.",
                                                                                                   "{0}.FileDirectory for {1} should not {2}.",
                                                                                                   Category,
                                                                                                   DiagnosticSeverity.Error,
                                                                                                   true);

        public static readonly DiagnosticDescriptor ScriptableSettingsShouldNotBeAbstract = new("COIMBRA" + "0004",
                                                                                                "ScriptableSettings attributes are not supported on abstract types.",
                                                                                                "{0} should be removed from abstract type {1}.",
                                                                                                Category,
                                                                                                DiagnosticSeverity.Error,
                                                                                                true);

        public static readonly DiagnosticDescriptor ScriptableSettingsShouldNotBeGeneric = new("COIMBRA" + "0005",
                                                                                               "ScriptableSettings attributes are not supported on generic types.",
                                                                                               "{0} should be removed from generic type {1}.",
                                                                                               Category,
                                                                                               DiagnosticSeverity.Error,
                                                                                               true);

        public static readonly DiagnosticDescriptor SharedManagedPoolRequiresToUseCreateShared = new("COIMBRA" + "0006",
                                                                                                     "Use CreateShared method for any SharedManagedPool.",
                                                                                                     "Use {0}.CreateShared instead of new {0}.",
                                                                                                     Category,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true);

        public static readonly DiagnosticDescriptor DoNotUseCreateSharedOutsideFromSharedManagedPool = new("COIMBRA" + "0007",
                                                                                                           "Use constructor when not a SharedManagedPool.",
                                                                                                           "Use new {0} instead of {0}.CreateShared.",
                                                                                                           Category,
                                                                                                           DiagnosticSeverity.Error,
                                                                                                           true);

        public static readonly DiagnosticDescriptor ObjectDestroyShouldNotBeUsed = new("COIMBRA" + "0008",
                                                                                       "Object.Destroy should not be used with Objects that can be an Actor.",
                                                                                       "Use {0}.Destroy() or Object.DestroyImmediate({0}) instead of Object.Destroy({0}).",
                                                                                       Category,
                                                                                       DiagnosticSeverity.Error,
                                                                                       true);

        public static readonly DiagnosticDescriptor CopyBaseConstructorsRequiresPartialKeyword = new("COIMBRA" + "0009",
                                                                                                     "Type with CopyBaseConstructorsAttribute should be partial.",
                                                                                                     "Add missing partial keyword in {0}.",
                                                                                                     Category,
                                                                                                     DiagnosticSeverity.Warning,
                                                                                                     true);

        public static readonly DiagnosticDescriptor CopyBaseConstructorsDoesntSupportNestedTypes = new("COIMBRA" + "0010",
                                                                                                       "Type with CopyBaseConstructorsAttribute should not be nested.",
                                                                                                       "Move {0} outside of {1}.",
                                                                                                       Category,
                                                                                                       DiagnosticSeverity.Error,
                                                                                                       true);

        public static readonly DiagnosticDescriptor SharedManagedPoolRequiresPartialKeyword = new("COIMBRA" + "0011",
                                                                                                  "Type with SharedManagedPoolAttribute should be partial.",
                                                                                                  "Add missing partial keyword in {0}.",
                                                                                                  Category,
                                                                                                  DiagnosticSeverity.Warning,
                                                                                                  true);

        public static readonly DiagnosticDescriptor SharedManagedPoolDoesntSupportNestedTypes = new("COIMBRA" + "0012",
                                                                                                    "Type with SharedManagedPoolAttribute should not be nested.",
                                                                                                    "Move {0} outside of {1}.",
                                                                                                    Category,
                                                                                                    DiagnosticSeverity.Error,
                                                                                                    true);

        public static readonly DiagnosticDescriptor UseListenerComponentInsteadOfMonoBehaviourCallback = new("COIMBRA" + "0013",
                                                                                                             "MonoBehaviour callbacks should be replaced with their equivalent listener.",
                                                                                                             "Use {0} instead of {1}.",
                                                                                                             Category,
                                                                                                             DiagnosticSeverity.Error,
                                                                                                             true);

        public static readonly DiagnosticDescriptor UseActorAlternative = new("COIMBRA" + "0014",
                                                                              "GameObject API should be replaced by Actor API.",
                                                                              "Use {0} instead of {1}.",
                                                                              Category,
                                                                              DiagnosticSeverity.Error,
                                                                              false);

        public static readonly DiagnosticDescriptor ComponentMethodsShouldNotBeUsedWithActorTypes = new("COIMBRA" + "0015",
                                                                                                        "Component methods should not be used with Actor types.",
                                                                                                        "Use {0} instead of {1}.",
                                                                                                        Category,
                                                                                                        DiagnosticSeverity.Error,
                                                                                                        false);

        public static readonly DiagnosticDescriptor ObjectInstantiateShouldNotBeUsed = new("COIMBRA" + "0016",
                                                                                           "Object.Instantiate should not be used with Objects that can be an Actor.",
                                                                                           "Use IPoolService.Spawn or a custom alternative.",
                                                                                           Category,
                                                                                           DiagnosticSeverity.Error,
                                                                                           true);

        public static readonly DiagnosticDescriptor FieldTypeNotCompatibleWithAttribute = new("COIMBRA" + "0017",
                                                                                              "Attribute being used with incompatible field type.",
                                                                                              "Remove {0} from {1} or change {1} type to {2}.",
                                                                                              Category,
                                                                                              DiagnosticSeverity.Error,
                                                                                              false);

        public static readonly DiagnosticDescriptor IncorrectManagedFieldGenericParameter = new("COIMBRA" + "0018",
                                                                                                "ManagedField being used with incompatible type parameter.",
                                                                                                "Change {0} to a compatible type.",
                                                                                                Category,
                                                                                                DiagnosticSeverity.Error,
                                                                                                false);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DiagnosticDescriptor _ = new("COIMBRA" + "0000",
                                                            "",
                                                            "",
                                                            Category,
                                                            DiagnosticSeverity.Hidden,
                                                            false);
    }
}

#pragma warning restore RS2008
