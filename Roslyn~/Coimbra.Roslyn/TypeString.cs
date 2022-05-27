using Microsoft.CodeAnalysis;

namespace Coimbra.Roslyn
{
    public readonly struct TypeString
    {
        public readonly string Name;

        public readonly string Namespace;

        public TypeString(string name, string @namespace)
        {
            Name = name;
            Namespace = @namespace;
        }

        public string FullName => $"{Namespace}.{Name}";

        public static TypeString From(ITypeSymbol typeSymbol)
        {
            return new TypeString(typeSymbol.Name, typeSymbol.ContainingNamespace.ToString());
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
