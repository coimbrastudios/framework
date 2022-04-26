using Microsoft.CodeAnalysis;

#pragma warning disable RS2008

namespace Coimbra.Roslyn
{
    public static class Diagnostics
    {
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

        public static readonly DiagnosticDescriptor ConcreteEventParameterlessCtorShouldBePublic = new("COIMBRA0003",
                                                                                                       "Concrete IEvent parameterless constructor should be public.",
                                                                                                       "Make public the parameterless constructor of {0}.",
                                                                                                       EventsCategory,
                                                                                                       DiagnosticSeverity.Error,
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

        public static readonly DiagnosticDescriptor AbstractServiceShouldBeUsedWithServiceInterfaces = new("COIMBRA0008",
                                                                                                           "AbstractServiceAttribute should only be used with an interface that extends IService.",
                                                                                                           "Remove AbstractServiceAttribute from {0}.",
                                                                                                           ServicesCategory,
                                                                                                           DiagnosticSeverity.Warning,
                                                                                                           true);
    }
}

#pragma warning restore RS2008
