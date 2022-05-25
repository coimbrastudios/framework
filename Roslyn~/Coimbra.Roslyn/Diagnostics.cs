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
                                                                                          "ServiceLocator.{0} requires an interface as the type argument, {1} is not a compatible type.",
                                                                                          ServicesCategory,
                                                                                          DiagnosticSeverity.Error,
                                                                                          true);

        public static readonly DiagnosticDescriptor ServiceLocatorRequiresNonAbstractInterface = new("COIMBRA0005",
                                                                                                     "ServiceLocator APIs requires an interface type without AbstractServiceAttribute as generic parameter.",
                                                                                                     "ServiceLocator.{0} requires another type argument, or remove AbstractInterfaceAttribute from {1}.",
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

        // TODO: Add CodeFix
        public static readonly DiagnosticDescriptor AbstractServiceShouldBeUsedWithServiceInterfaces = new("COIMBRA0008",
                                                                                                           "AbstractServiceAttribute should only be used with an interface that extends IService.",
                                                                                                           "Remove AbstractServiceAttribute from {0}.",
                                                                                                           ServicesCategory,
                                                                                                           DiagnosticSeverity.Warning,
                                                                                                           true);

        // TODO: Add CodeFix
        public static readonly DiagnosticDescriptor EventServiceGenericMethodsShouldNotBeUsedDirectly = new("COIMBRA0009",
                                                                                                            "IEventService generic APIs should not be used directly.",
                                                                                                            "Use {0}.{1} instead.",
                                                                                                            EventsCategory,
                                                                                                            DiagnosticSeverity.Error,
                                                                                                            true);

        // TODO: Add CodeFix
        public static readonly DiagnosticDescriptor DisableDefaultFactoryAttributeIsRedundant = new("COIMBRA0010",
                                                                                                    "DisableDefaultFactoryAttribute should only be used in concrete IService.",
                                                                                                    "Remove DisableDefaultFactoryAttribute from {0}.",
                                                                                                    ServicesCategory,
                                                                                                    DiagnosticSeverity.Warning,
                                                                                                    false);

        public static readonly DiagnosticDescriptor ScriptableSettingsHasConflictingAttributes = new("COIMBRA0011",
                                                                                                     "ProjectSettingsAttribute and PreferencesAttribute should not be used together.",
                                                                                                     "Remove either ProjectSettingsAttribute or PreferencesAttribute from {0}",
                                                                                                     DefaultCategory,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     false);

        public static readonly DiagnosticDescriptor ScriptableSettingsFileDirectoryIsInvalid = new("COIMBRA0012",
                                                                                                   "ScriptableSettings FileDirectory should not be inside the project's Asset folder or contains \"..\" in its path.",
                                                                                                   "Change FileDirectory for {0} in {1}.",
                                                                                                   DefaultCategory,
                                                                                                   DiagnosticSeverity.Error,
                                                                                                   false);

        public static readonly DiagnosticDescriptor SharedManagedPoolHasInvalidInstanceValueField = new("COIMBRA0013",
                                                                                                        "SharedManagedPoolAttribute has an invalid value for InstanceValueField.",
                                                                                                        "Couldn't find {0} in {1}.",
                                                                                                        DefaultCategory,
                                                                                                        DiagnosticSeverity.Error,
                                                                                                        false);

        public static readonly DiagnosticDescriptor SharedManagedPoolHasInvalidNestedInstanceWrapper = new("COIMBRA0014",
                                                                                                           "SharedManagedPoolAttribute has an invalid value for NestedInstanceWrapper.",
                                                                                                           "Couldn't find {0} in {1}.",
                                                                                                           DefaultCategory,
                                                                                                           DiagnosticSeverity.Error,
                                                                                                           false);

        public static readonly DiagnosticDescriptor InheritFromServiceActorBaseInstead = new("COIMBRA0015",
                                                                                             "Concrete IService is a MonoBehaviour but doesn't inherit from ServiceActorBase.",
                                                                                             "Make {0} inherit from ServiceActorBase or ensure that it is implemented correctly.",
                                                                                             ServicesCategory,
                                                                                             DiagnosticSeverity.Info,
                                                                                             false);

        // TODO: Add CodeFix
        public static readonly DiagnosticDescriptor PreloadDefaultFactoryAttributeIsRedundant = new("COIMBRA0016",
                                                                                                    "PreloadServiceAttribute should only be used in concrete IService",
                                                                                                    "Remove PreloadServiceAttribute from {0}.",
                                                                                                    ServicesCategory,
                                                                                                    DiagnosticSeverity.Warning,
                                                                                                    false);

        // TODO: Add CodeFix
        public static readonly DiagnosticDescriptor SharedServiceLocatorShouldNotBeUsedFromWithinServiceImplementation = new("COIMBRA017",
                                                                                                                             "ServiceLocator.Shared should not be accessed from within a IService implementation.",
                                                                                                                             "Use OwningLocator instead of SharedLocator.Shared.",
                                                                                                                             ServicesCategory,
                                                                                                                             DiagnosticSeverity.Error,
                                                                                                                             false);

        public static readonly DiagnosticDescriptor OwningLocatorSetterIsForInternalUseOnly = new("COIMBRA0018",
                                                                                                  "OwningLocator.set is for internal use only, using it may lead to unexpected errors.",
                                                                                                  "Do not set the OwningLocator directly.",
                                                                                                  ServicesCategory,
                                                                                                  DiagnosticSeverity.Error,
                                                                                                  false);

        public static readonly DiagnosticDescriptor ScriptableSettingsShouldNotImplementService = new("COIMBRA0019",
                                                                                                      "A ScriptableSettings should not implement IService.",
                                                                                                      "You can move IService implementation to another class or don't inherit from ScriptableSettings.",
                                                                                                      ServicesCategory,
                                                                                                      DiagnosticSeverity.Error,
                                                                                                      false);

        public static readonly DiagnosticDescriptor UseServiceActorBaseToImplementGameObjectServices = new("COIMBRA0020",
                                                                                                           "Inherit from ServiceActorBase to implement IService on GameObjects.",
                                                                                                           "Change {0} to inherit from ServiceActorBase instead.",
                                                                                                           DefaultCategory,
                                                                                                           DiagnosticSeverity.Error,
                                                                                                           false);

        // TODO: Add CodeFix
        public static readonly DiagnosticDescriptor IncorrectGameObjectProperty = new("COIMBRA0021",
                                                                                      "Component.gameObject should not be used with an Actor.",
                                                                                      "Use {0}.GameObject instead.",
                                                                                      DefaultCategory,
                                                                                      DiagnosticSeverity.Warning,
                                                                                      false);

        // TODO: Add CodeFix
        public static readonly DiagnosticDescriptor IncorrectTransformProperty = new("COIMBRA0022",
                                                                                     "Component.transform should not be used with an Actor.",
                                                                                     "Use {0}.Transform instead.",
                                                                                     DefaultCategory,
                                                                                     DiagnosticSeverity.Warning,
                                                                                     false);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DiagnosticDescriptor _ = new("COIMBRA00",
                                                            "",
                                                            "",
                                                            DefaultCategory,
                                                            DiagnosticSeverity.Hidden,
                                                            false);
    }
}

#pragma warning restore RS2008
