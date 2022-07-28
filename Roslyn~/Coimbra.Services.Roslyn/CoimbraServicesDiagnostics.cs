using Microsoft.CodeAnalysis;
using System.ComponentModel;

#pragma warning disable RS2008

namespace Coimbra.Services.Roslyn
{
    public static class CoimbraServicesDiagnostics
    {
        private const string Category = CoimbraServicesTypes.Namespace;

        public static readonly DiagnosticDescriptor ServiceLocatorRequiresInterface = new("COIMBRA" + "0101",
                                                                                          "ServiceLocator APIs requires an interface type as generic parameter.",
                                                                                          "ServiceLocator.{0} requires an interface as the type argument and {1} is not a compatible type.",
                                                                                          Category,
                                                                                          DiagnosticSeverity.Error,
                                                                                          true);

        public static readonly DiagnosticDescriptor ServiceLocatorRequiresNonAbstractInterface = new("COIMBRA" + "0102",
                                                                                                     "ServiceLocator APIs requires an interface type without AbstractServiceAttribute as generic parameter.",
                                                                                                     "ServiceLocator.{0} requires another type argument or remove AbstractInterfaceAttribute from {1}.",
                                                                                                     Category,
                                                                                                     DiagnosticSeverity.Error,
                                                                                                     true);

        public static readonly DiagnosticDescriptor ConcreteServiceShouldOnlyImplementOneService = new("COIMBRA" + "0103",
                                                                                                       "Concrete IService should only implement one IService at a time.",
                                                                                                       "Move all but one of the service implementations out from {0}.",
                                                                                                       Category,
                                                                                                       DiagnosticSeverity.Error,
                                                                                                       true);

        public static readonly DiagnosticDescriptor ConcreteServiceShouldNotImplementAbstractService = new("COIMBRA" + "0104",
                                                                                                           "Concrete IService should not implement any IService with AbstractServiceAttribute.",
                                                                                                           "Remove all abstract services implementations from {0} or make {0} abstract.",
                                                                                                           Category,
                                                                                                           DiagnosticSeverity.Warning,
                                                                                                           true);

        public static readonly DiagnosticDescriptor BaseClassIsConcreteServiceAlready = new("COIMBRA" + "0105",
                                                                                            "Type can't implement any IService because parent class already implements one.",
                                                                                            "{0} can't implement any IService because {1} already implements one.",
                                                                                            Category,
                                                                                            DiagnosticSeverity.Error,
                                                                                            true);

        public static readonly DiagnosticDescriptor InheritFromActorInstead = new("COIMBRA" + "0106",
                                                                                  "Concrete IService should not be a Component unless it inherit from Actor.",
                                                                                  "Make {0} inherit from Actor to avoid undefined behaviours.",
                                                                                  Category,
                                                                                  DiagnosticSeverity.Error,
                                                                                  true);

        public static readonly DiagnosticDescriptor ScriptableSettingsShouldNotImplementService = new("COIMBRA" + "0107",
                                                                                                      "A ScriptableSettings should not implement any IService.",
                                                                                                      "Move IService implementation to another class or make {0} inherit from a type that is not ScriptableSettings.",
                                                                                                      Category,
                                                                                                      DiagnosticSeverity.Error,
                                                                                                      true);

        public static readonly DiagnosticDescriptor MissingRequiredServiceAttribute = new("COIMBRA" + "0108",
                                                                                          "ServiceLocator.GetChecked should only be used on services with RequiredServiceAttribute.",
                                                                                          "Use ServiceLocator.Get/TryGet instead or add RequiredServiceAttribute to {0}.",
                                                                                          Category,
                                                                                          DiagnosticSeverity.Error,
                                                                                          true);

        public static readonly DiagnosticDescriptor UseGetCheckedWithRequiredService = new("COIMBRA" + "0109",
                                                                                           "ServiceLocator.GetChecked is preferred on services with RequiredServiceAttribute.",
                                                                                           "{0} is assumed to never be null, use ServiceLocator.GetChecked instead.",
                                                                                           Category,
                                                                                           DiagnosticSeverity.Warning,
                                                                                           true);

        public static readonly DiagnosticDescriptor SetBeingUsedInNonDynamicService = new("COIMBRA" + "0110",
                                                                                          "ServiceLocator.Set should not be used on a service without DynamicServiceAttribute.",
                                                                                          "Add DynamicServiceAttribute to {0} or use a IServiceFactory instead.",
                                                                                          Category,
                                                                                          DiagnosticSeverity.Error,
                                                                                          true);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DiagnosticDescriptor _ = new("COIMBRA" + "0100",
                                                            "",
                                                            "",
                                                            Category,
                                                            DiagnosticSeverity.Hidden,
                                                            false);
    }
}

#pragma warning restore RS2008
