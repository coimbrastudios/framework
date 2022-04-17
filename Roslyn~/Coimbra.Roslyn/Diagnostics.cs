using Microsoft.CodeAnalysis;

#pragma warning disable RS2008

namespace Coimbra.Roslyn
{
    public static class Diagnostics
    {
        private const string EventsCategory = "Coimbra.Services.Events";

        public static readonly DiagnosticDescriptor ConcreteEventShouldBePartial = new DiagnosticDescriptor("COIMBRA0001",
                                                                                                            "Concrete IEvent should be partial.",
                                                                                                            "{0} is missing the partial keyword.",
                                                                                                            EventsCategory,
                                                                                                            DiagnosticSeverity.Warning,
                                                                                                            true);

        public static readonly DiagnosticDescriptor ConcreteEventShouldNotBeNested = new DiagnosticDescriptor("COIMBRA0002",
                                                                                                              "Concrete IEvent should not be a nested type.",
                                                                                                              "{0} should be moved from outside the scope of {1}.",
                                                                                                              EventsCategory,
                                                                                                              DiagnosticSeverity.Warning,
                                                                                                              true);
    }
}

#pragma warning restore RS2008
