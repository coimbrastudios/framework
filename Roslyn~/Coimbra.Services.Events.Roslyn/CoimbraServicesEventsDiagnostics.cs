using Microsoft.CodeAnalysis;
using System.ComponentModel;

#pragma warning disable RS2008

namespace Coimbra.Services.Events.Roslyn
{
    public static class CoimbraServicesEventsDiagnostics
    {
        private const string Category = CoimbraServicesEventsTypes.Namespace;

        public static readonly DiagnosticDescriptor ConcreteEventShouldBePartial = new("COIMBRA" + "0201",
                                                                                       "Concrete IEvent should be partial.",
                                                                                       "Add missing partial keyword in {0}.",
                                                                                       Category,
                                                                                       DiagnosticSeverity.Warning,
                                                                                       true);

        public static readonly DiagnosticDescriptor ConcreteEventShouldNotBeNested = new("COIMBRA" + "0202",
                                                                                         "Concrete IEvent should not be a nested type.",
                                                                                         "Move {0} outside of {1}.",
                                                                                         Category,
                                                                                         DiagnosticSeverity.Error,
                                                                                         true);

        public static readonly DiagnosticDescriptor ClassEventShouldBeEitherAbstractOrSealed = new("COIMBRA" + "0203",
                                                                                                   "Class events should be either abstract or sealed.",
                                                                                                   "Add sealed or abstract keyword to {0}.",
                                                                                                   Category,
                                                                                                   DiagnosticSeverity.Error,
                                                                                                   true);

        public static readonly DiagnosticDescriptor EventServiceGenericMethodsShouldNotBeUsedDirectly = new("COIMBRA" + "0204",
                                                                                                            "IEventService generic APIs should not be used directly.",
                                                                                                            "Use {0}.{1} instead.",
                                                                                                            Category,
                                                                                                            DiagnosticSeverity.Error,
                                                                                                            true);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DiagnosticDescriptor _ = new("COIMBRA" + "0200",
                                                            "",
                                                            "",
                                                            Category,
                                                            DiagnosticSeverity.Hidden,
                                                            false);
    }
}

#pragma warning restore RS2008
