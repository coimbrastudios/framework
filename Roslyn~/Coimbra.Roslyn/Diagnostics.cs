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
                                                                                         DiagnosticSeverity.Warning,
                                                                                         true);

        public static readonly DiagnosticDescriptor ClassEventShouldBeEitherAbstractOrSealed = new("COIMBRA0003",
                                                                                                   "Class events should be either abstract or sealed.",
                                                                                                   "Add sealed or abstract keyword to {0}.",
                                                                                                   EventsCategory,
                                                                                                   DiagnosticSeverity.Warning,
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

        public static readonly DiagnosticDescriptor C0008 = new("COIMBRA0008",
                                                                "",
                                                                "",
                                                                DefaultCategory,
                                                                DiagnosticSeverity.Hidden,
                                                                false);

        // TODO: add CodeFix
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

        public static readonly DiagnosticDescriptor InheritFromServiceActorBaseInstead = new("COIMBRA0011",
                                                                                             "Concrete IService should not be a Component unless it inherit from ServiceActorBase.",
                                                                                             "Make {0} inherit from ServiceActorBase to avoid undefined behaviours.",
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

        // TODO: add CodeFix
        public static readonly DiagnosticDescriptor OwningLocatorShouldBeUsedInsteadOfShared = new("COIMBRA017",
                                                                                                   "ServiceLocator.Shared should not be accessed inside IService.",
                                                                                                   "Use OwningLocator instead of ServiceLocator.Shared.",
                                                                                                   ServicesCategory,
                                                                                                   DiagnosticSeverity.Error,
                                                                                                   true);

        public static readonly DiagnosticDescriptor OwningLocatorSetterIsForInternalUseOnly = new("COIMBRA0018",
                                                                                                  "OwningLocator.set is for internal use only.",
                                                                                                  "Do not set the OwningLocator manually.",
                                                                                                  ServicesCategory,
                                                                                                  DiagnosticSeverity.Error,
                                                                                                  true);

        // TODO: add CodeFix
        public static readonly DiagnosticDescriptor ObjectDestroyShouldNotBeUsed = new("COIMBRA0019",
                                                                                       "Object.Destroy should be avoided.",
                                                                                       "Use {0}.Destroy() instead of Object.Destroy({0}).",
                                                                                       DefaultCategory,
                                                                                       DiagnosticSeverity.Info,
                                                                                       true);

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
