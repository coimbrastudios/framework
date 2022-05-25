namespace Coimbra.Roslyn
{
    public sealed class TypeString
    {
        public readonly string Name;

        public readonly string Namespace;

        public TypeString(string name, string @namespace)
        {
            Name = name;
            Namespace = @namespace;
        }

        public string FullName => $"{Namespace}.{Name}";

        public override string ToString()
        {
            return Name;
        }
    }
}
