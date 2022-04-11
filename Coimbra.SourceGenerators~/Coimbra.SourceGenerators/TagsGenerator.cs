using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace Coimbra.SourceGenerators
{
    [Generator]
    public sealed class TagsGenerator : ISourceGenerator
    {
        private readonly string[] _defaultTags =
        {
            "Untagged",
            "Respawn",
            "Finish",
            "EditorOnly",
            "MainCamera",
            "Player",
            "GameController",
        };

        public void Execute(GeneratorExecutionContext context)
        {
            StringBuilder sourceBuilder = new StringBuilder(@"// This file is auto-generated!

using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Strongly-typed representation of a Unity tag.
    /// </summary>
    [Preserve]
    public readonly struct Tag
    {");

            foreach (string value in _defaultTags)
            {
                sourceBuilder.Append($@"
        /// <summary>
        /// {value}
        /// </summary>
        public static readonly Tag {value} = new Tag(""{value}"");
");
            }

            sourceBuilder.Append(@"
        public readonly string Value;

        public Tag(string value)
        {
            Value = value;
        }

        public static explicit operator Tag(string value)
        {
            return new Tag(value);
        }

        public static implicit operator string(Tag tag)
        {
            return tag.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
");

            context.AddSource("Tag.generated", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context) { }
    }
}
