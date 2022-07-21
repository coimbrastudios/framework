using Coimbra.Roslyn;

namespace Coimbra.Services.Roslyn
{
    public static class CoimbraServicesTypes
    {
        public const string Namespace = "Coimbra.Services";

        public static readonly TypeString AbstractServiceAttribute = new("AbstractServiceAttribute", Namespace);

        public static readonly TypeString DefaultServiceActorFactoryClass = new("DefaultServiceActorFactory", Namespace);

        public static readonly TypeString DefaultServiceFactoryClass = new("DefaultServiceFactory", Namespace);

        public static readonly TypeString DisableDefaultFactoryAttribute = new("DisableDefaultFactoryAttribute", Namespace);

        public static readonly TypeString DynamicServiceAttribute = new("DynamicServiceAttribute", Namespace);

        public static readonly TypeString PreloadServiceAttribute = new("PreloadServiceAttribute", Namespace);

        public static readonly TypeString RequiredServiceAttribute = new("RequiredServiceAttribute", Namespace);

        public static readonly TypeString ServiceInterface = new("IService", Namespace);

        public static readonly TypeString ServiceLocatorClass = new("ServiceLocator", Namespace);
    }
}
