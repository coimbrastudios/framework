using Microsoft.CodeAnalysis;
using System.ComponentModel;

#pragma warning disable RS2008

namespace Coimbra.Roslyn
{
    public static class Diagnostics
    {
        private const string DefaultCategory = "Coimbra";

        private const string EventsCategory = "Coimbra.Services.Events";

        private const string ServicesCategory = "Coimbra.Services";

        public static readonly DiagnosticDescriptor ConcreteEventShouldBePartial = new("COIMBRA0001",
                                                                                       "Concrete IEvent should be partial.",
                                                                                       "Add missing partial keyword in {0}.",
                                                                                       EventsCategory,
                                                                                       DiagnosticSeverity.Warning,
                                                                                       true);

        public static readonly DiagnosticDescriptor ConcreteEventShouldNotBeNested = new("COIMBRA0002",
                                                                                         "Concrete IEvent should not be a nested type.",
                                                                                         "Move {0} outside of {1}.",
                                                                                         EventsCategory,
                                                                                         DiagnosticSeverity.Error,
                                                                                         true);

        public static readonly DiagnosticDescriptor ClassEventShouldBeEitherAbstractOrSealed = new("COIMBRA0003",
                                                                                                   "Class events should be either abstract or sealed.",
                                                                                                   "Add sealed or abstract keyword to {0}.",
                                                                                                   EventsCategory,
                                                                                                   DiagnosticSeverity.Error,
                                                                                                   true);

        public static readonly DiagnosticDescriptor ServiceLocatorRequiresInterface = new("COIMBRA0004",
                                                                                          "ServiceLocator APIs requires an interface type as generic parameter.",
                                                                                          "ServiceLocator.{0} requires an interface as the type argument and {1} is not a compatible type.",
                                                                                          ServicesCategory,
                                                                                          DiagnosticSeverity.Error,
                                                                                          true);

        public static readonly DiagnosticDescriptor ServiceLocatorRequiresNonAbstractInterface = new("COIMBRA0005",
                                                                                                     "ServiceLocator APIs requires an interface type without AbstractServiceAttribute as generic parameter.",
                                                                                                     "ServiceLocator.{0} requires another type argument or remove AbstractInterfaceAttribute from {1}.",
                                                                                                     ServicesCategory,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true);

        public static readonly DiagnosticDescriptor ConcreteServiceShouldOnlyImplementOneService = new("COIMBRA0006",
                                                                                                       "Concrete IService should only implement one IService at a time.",
                                                                                                       "Move all but one of the service implementations out from {0}.",
                                                                                                       ServicesCategory,
                                                                                                       DiagnosticSeverity.Error,
                                                                                                       true);

        public static readonly DiagnosticDescriptor ConcreteServiceShouldNotImplementAbstractService = new("COIMBRA0007",
                                                                                                           "Concrete IService should not implement any IService with AbstractServiceAttribute.",
                                                                                                           "Remove all abstract services implementations from {0}.",
                                                                                                           ServicesCategory,
                                                                                                           DiagnosticSeverity.Warning,
                                                                                                           true);

        public static readonly DiagnosticDescriptor SharedManagedPoolDoesntSupportGenericTypes = new("COIMBRA0008",
                                                                                                     "Type with SharedManagedPoolAttribute should not be generic.",
                                                                                                     "Remove SharedManagedPoolAttribute from {0} or make it non-generic.",
                                                                                                     DefaultCategory,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true);

        public static readonly DiagnosticDescriptor EventServiceGenericMethodsShouldNotBeUsedDirectly = new("COIMBRA0009",
                                                                                                            "IEventService generic APIs should not be used directly.",
                                                                                                            "Use {0}.{1} instead.",
                                                                                                            EventsCategory,
                                                                                                            DiagnosticSeverity.Error,
                                                                                                            true);

        public static readonly DiagnosticDescriptor BaseClassIsConcreteServiceAlready = new("COIMBRA0010",
                                                                                            "Type can't implement any IService because parent class already implements one.",
                                                                                            "{0} can't implement any IService because {1} already implements one.",
                                                                                            ServicesCategory,
                                                                                            DiagnosticSeverity.Error,
                                                                                            true);

        public static readonly DiagnosticDescriptor InheritFromActorInstead = new("COIMBRA0011",
                                                                                  "Concrete IService should not be a Component unless it inherit from Actor.",
                                                                                  "Make {0} inherit from Actor to avoid undefined behaviours.",
                                                                                  ServicesCategory,
                                                                                  DiagnosticSeverity.Warning,
                                                                                  true);

        public static readonly DiagnosticDescriptor ScriptableSettingsShouldNotImplementService = new("COIMBRA0012",
                                                                                                      "A ScriptableSettings should not implement any IService.",
                                                                                                      "Move IService implementation to another class or make {0} inherit from a type that is not ScriptableSettings.",
                                                                                                      ServicesCategory,
                                                                                                      DiagnosticSeverity.Error,
                                                                                                      true);

        public static readonly DiagnosticDescriptor ScriptableSettingsHasConflictingAttributes = new("COIMBRA0013",
                                                                                                     "ProjectSettingsAttribute and PreferencesAttribute should not be used together.",
                                                                                                     "Remove either ProjectSettingsAttribute or PreferencesAttribute from {0}",
                                                                                                     DefaultCategory,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true);

        public static readonly DiagnosticDescriptor ScriptableSettingsFileDirectoryIsInvalid = new("COIMBRA0014",
                                                                                                   "ScriptableSettings has an invalided FileDirectory.",
                                                                                                   "{0}.FileDirectory for {1} should not {2}.",
                                                                                                   DefaultCategory,
                                                                                                   DiagnosticSeverity.Error,
                                                                                                   true);

        public static readonly DiagnosticDescriptor ScriptableSettingsShouldNotBeAbstract = new("COIMBRA0015",
                                                                                                "ScriptableSettings attributes are not supported on abstract types.",
                                                                                                "{0} should be removed from abstract type {1}.",
                                                                                                DefaultCategory,
                                                                                                DiagnosticSeverity.Error,
                                                                                                true);

        public static readonly DiagnosticDescriptor ScriptableSettingsShouldNotBeGeneric = new("COIMBRA0016",
                                                                                               "ScriptableSettings attributes are not supported on generic types.",
                                                                                               "{0} should be removed from generic type {1}.",
                                                                                               DefaultCategory,
                                                                                               DiagnosticSeverity.Error,
                                                                                               true);

        // TODO: codefix
        public static readonly DiagnosticDescriptor SharedManagedPoolRequiresToUseCreateShared = new("COIMBRA0017",
                                                                                                     "Use CreateShared method for any SharedManagedPool.",
                                                                                                     "Use {0}.CreateShared instead of new {0}.",
                                                                                                     DefaultCategory,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true);

        // TODO: codefix
        public static readonly DiagnosticDescriptor DoNotUseCreateSharedOutsideFromSharedManagedPool = new("COIMBRA0018",
                                                                                                           "Use constructor when not a SharedManagedPool.",
                                                                                                           "Use new {0} instead of {0}.CreateShared.",
                                                                                                           DefaultCategory,
                                                                                                           DiagnosticSeverity.Error,
                                                                                                           true);

        public static readonly DiagnosticDescriptor ObjectDestroyShouldNotBeUsed = new("COIMBRA0019",
                                                                                       "Object.Destroy should not be used with Objects that can be an Actor.",
                                                                                       "Use {0}.Destroy() or Object.DestroyImmediate({0}) instead of Object.Destroy({0}).",
                                                                                       DefaultCategory,
                                                                                       DiagnosticSeverity.Warning,
                                                                                       true);

        public static readonly DiagnosticDescriptor CopyBaseConstructorsRequiresPartialKeyword = new("COIMBRA0020",
                                                                                                     "Type with CopyBaseConstructorsAttribute should be partial.",
                                                                                                     "Add missing partial keyword in {0}.",
                                                                                                     DefaultCategory,
                                                                                                     DiagnosticSeverity.Warning,
                                                                                                     true);

        public static readonly DiagnosticDescriptor CopyBaseConstructorsDoesntSupportNestedTypes = new("COIMBRA0021",
                                                                                                       "Type with CopyBaseConstructorsAttribute should not be nested.",
                                                                                                       "Move {0} outside of {1}.",
                                                                                                       DefaultCategory,
                                                                                                       DiagnosticSeverity.Error,
                                                                                                       true);

        public static readonly DiagnosticDescriptor SharedManagedPoolRequiresPartialKeyword = new("COIMBRA0022",
                                                                                                  "Type with SharedManagedPoolAttribute should be partial.",
                                                                                                  "Add missing partial keyword in {0}.",
                                                                                                  DefaultCategory,
                                                                                                  DiagnosticSeverity.Warning,
                                                                                                  true);

        public static readonly DiagnosticDescriptor SharedManagedPoolDoesntSupportNestedTypes = new("COIMBRA0023",
                                                                                                    "Type with SharedManagedPoolAttribute should not be nested.",
                                                                                                    "Move {0} outside of {1}.",
                                                                                                    DefaultCategory,
                                                                                                    DiagnosticSeverity.Error,
                                                                                                    true);

        public static readonly DiagnosticDescriptor UseListenerComponentInsteadOfMonoBehaviourCallback = new("COIMBRA0024",
                                                                                                             "MonoBehaviour callbacks should be replaced with their equivalent listener when possible.",
                                                                                                             "Use {0} instead of {1}.",
                                                                                                             DefaultCategory,
                                                                                                             DiagnosticSeverity.Warning,
                                                                                                             true);

        // TODO: codefix
        public static readonly DiagnosticDescriptor UseActorAlternative = new("COIMBRA0025",
                                                                              "GameObject API should be replaced by Actor API.",
                                                                              "Use {0} instead of {1}.",
                                                                              DefaultCategory,
                                                                              DiagnosticSeverity.Warning,
                                                                              false);

        // TODO: codefix
        public static readonly DiagnosticDescriptor ComponentMethodsShouldNotBeUsedWithActorTypes = new("COIMBRA0026",
                                                                                                        "Component methods should not be used with Actor types.",
                                                                                                        "Use {0} instead of {1}.",
                                                                                                        DefaultCategory,
                                                                                                        DiagnosticSeverity.Error,
                                                                                                        false);

        public static readonly DiagnosticDescriptor FilterTypesAttributeUsedWithIncompatibleField = new("COIMBRA0027",
                                                                                                        "FilterTypesAttribute being used with incompatible field type.",
                                                                                                        "Remove {0} from {1} or change {1} type.",
                                                                                                        DefaultCategory,
                                                                                                        DiagnosticSeverity.Warning,
                                                                                                        false);

        public static readonly DiagnosticDescriptor IncorrectTypeDropdownAttributeUsage = new("COIMBRA0028",
                                                                                              "TypeDropdownAttribute being used with incompatible field type.",
                                                                                              "Remove {0} from {1} or change {1} type.",
                                                                                              DefaultCategory,
                                                                                              DiagnosticSeverity.Error,
                                                                                              false);

        public static readonly DiagnosticDescriptor IncorrectManagedFieldGenericParameter = new("COIMBRA0029",
                                                                                                "ManagedField being used with incompatible type parameter.",
                                                                                                "Change {0} to a compatible type.",
                                                                                                DefaultCategory,
                                                                                                DiagnosticSeverity.Error,
                                                                                                false);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DiagnosticDescriptor _ = new("COIMBRA",
                                                            "",
                                                            "",
                                                            DefaultCategory,
                                                            DiagnosticSeverity.Hidden,
                                                            false);
    }
}

#pragma warning restore RS2008
