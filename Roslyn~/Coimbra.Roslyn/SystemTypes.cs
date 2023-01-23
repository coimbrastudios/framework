namespace Coimbra.Roslyn
{
    public sealed class SystemTypes
    {
        public static readonly TypeString GeneratedCodeAttribute = new("GeneratedCodeAttribute", "System.CodeDom.Compiler");

        public static readonly TypeString ListClass = new("List", "System.Collections.Generic");

        public static readonly TypeString MethodImplAttribute = new("MethodImplAttribute", "System.Runtime.CompilerServices");

        public static readonly TypeString MethodImplOptionsEnum = new("MethodImplOptions", "System.Runtime.CompilerServices");
    }
}
