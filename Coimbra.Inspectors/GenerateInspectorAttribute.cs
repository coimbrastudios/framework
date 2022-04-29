#nullable enable

using System;

namespace Coimbra.Inspectors
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class GenerateInspectorAttribute : Attribute
    {
        public readonly bool DrawScriptField;

        public GenerateInspectorAttribute(bool drawScriptField = true)
        {
            DrawScriptField = drawScriptField;
        }
    }
}
