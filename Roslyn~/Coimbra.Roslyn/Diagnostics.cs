using Microsoft.CodeAnalysis;

#pragma warning disable RS2008

namespace Coimbra.Roslyn
{
    public static class Diagnostics
    {
        private const string EventsCategory = "Coimbra.Services.Events";

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
                                                                                                       "Concrete IEvent should have public parameterless constructor.",
                                                                                                       "Make public the parameterless constructor of {0}.",
                                                                                                       EventsCategory,
                                                                                                       DiagnosticSeverity.Error,
                                                                                                       true);
    }
}

#pragma warning restore RS2008
