using Microsoft.CodeAnalysis;

namespace Coimbra.SourceGenerators
{
    public static class DiagnosticDescriptors
    {
        private const string EventsCategory = "Coimbra.Services.Events.SourceGenerators";

        public static readonly DiagnosticDescriptor ConcreteEventShouldBePartial = new DiagnosticDescriptor("COIMBRA0001",
                                                                                                            "Concrete IEvent should be partial.",
                                                                                                            "{0} is missing the partial keyword.",
                                                                                                            EventsCategory,
                                                                                                            DiagnosticSeverity.Warning,
                                                                                                            true);

        public static readonly DiagnosticDescriptor ConcreteEventShouldNotBeNested = new DiagnosticDescriptor("COIMBRA0002",
                                                                                                              "Concrete IEvent should not be a nested type.",
                                                                                                              "{0} should not be a nested type.",
                                                                                                              EventsCategory,
                                                                                                              DiagnosticSeverity.Warning,
                                                                                                              true);
    }
}
