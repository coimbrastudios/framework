namespace Coimbra.Roslyn
{
    public sealed class UnityEngineTypes
    {
        public const string Namespace = "UnityEngine";

        public static readonly TypeString ComponentClass = new("Component", Namespace);

        public static readonly TypeString GameObjectClass = new("GameObject", Namespace);

        public static readonly TypeString GuiContentClass = new("GUIContent", Namespace);

        public static readonly TypeString ObjectClass = new("Object", Namespace);

        public static readonly TypeString RuntimeInitializeLoadTypeEnum = new("RuntimeInitializeLoadType", Namespace);

        public static readonly TypeString RuntimeInitializeOnLoadMethodAttribute = new("RuntimeInitializeOnLoadMethodAttribute", Namespace);
    }
}
