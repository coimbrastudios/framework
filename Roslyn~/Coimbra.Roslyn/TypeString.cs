using Microsoft.CodeAnalysis;
using System;

namespace Coimbra.Roslyn
{
    public readonly struct TypeString : IEquatable<TypeString>
    {
        public static TypeString Null = default;

        public readonly string FullName;

        public readonly string Name;

        public readonly string Namespace;

        public TypeString(string name, string @namespace)
        {
            Name = name;
            Namespace = @namespace;
            FullName = $"{Namespace}.{Name}";
        }

        public static bool operator ==(TypeString left, TypeString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeString left, TypeString right)
        {
            return !left.Equals(right);
        }

        public static TypeString From(ITypeSymbol typeSymbol)
        {
            return new TypeString(typeSymbol.Name, typeSymbol.ContainingNamespace.ToString());
        }

        public override bool Equals(object obj)
        {
            return obj is TypeString other && Equals(other);
        }

        public bool Equals(TypeString other)
        {
            return FullName == other.FullName;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
