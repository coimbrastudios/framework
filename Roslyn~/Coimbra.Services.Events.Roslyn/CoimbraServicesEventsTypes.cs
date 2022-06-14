using Coimbra.Roslyn;

namespace Coimbra.Services.Events.Roslyn
{
    public static class CoimbraServicesEventsTypes
    {
        public const string Namespace = "Coimbra.Services.Events";

        public static readonly TypeString AllowEventServiceUsageForAttribute = new("AllowEventServiceUsageForAttribute", Namespace);

        public static readonly TypeString EventInterface = new("IEvent", Namespace);

        public static readonly TypeString EventServiceInterface = new("IEventService", Namespace);

        public static readonly TypeString EventSystemClass = new("EventSystem", Namespace);
    }
}
