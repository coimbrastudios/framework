# [Coimbra Framework](Index.md): Assembly Definition Rules

Linting tools for assembly definition assets.

This is an editor-only feature that aims into keeping the project source code organized by analyzing all the assembly definition files after each compilation.
They ensure that the assembly definition files are following the defined rules and apply necessary fixes on those.

You can select the defined rules by going to `Project Settings/Linting Settings` and modifying the `Assembly Definition Rules` list.
The rules will be applied in the same order as their are defined in the list, so order matters.

## Implementing Rules

To implement a new rule you need the following steps:

1. Create a new rule by inheriting from [AssemblyDefinitionRuleBase].
2. Override the `Apply` method.
3. (Optional) Add [CreateAssetMenuAttribute] to make easier to create a new asset.
4. Create a new asset and configure it in the inspector.

> You can also just create a new asset from one of the existing rules and customize the options.
> The default ones should give you enough control already for all the basic needs.

## Default Rules

This package comes with some default rules implementations.
They are all production-ready and provide good example of how to create your own rules efficiently.

- [Banned References]
- [Fix Duplicate References]
- [Fix Editor Only]
- [Fix Tests Only]
- [Force Reference By Name]
- [Force Root Namespace Match Name]
- [Required References]
- [Sort Precompiled References]
- [Sort References By Name]

> If you are looking into an advanced example see [Banned References] as it is the one that makes the most from the available APIs, including caching techniques to ensure a smooth editor experience.
> If you are looking into a very basic example see [Force Root Namespace Match Name].

[AssemblyDefinitionRuleBase]:<../Coimbra.Editor.Linting/AssemblyDefinitionRuleBase.cs>

[Banned References]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/BannedReferencesAssemblyDefinitionRule.cs>

[Fix Duplicate References]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/FixDuplicateReferencesAssemblyDefinitionRule.cs>

[Fix Editor Only]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/FixEditorOnlyAssemblyDefinitionRule.cs>

[Fix Tests Only]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/FixTestsOnlyAssemblyDefinitionRule.cs>

[Force Reference By Name]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/ForceReferenceByNameAssemblyDefinitionRule.cs>

[Force Root Namespace Match Name]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/ForceRootNamespaceMatchNameAssemblyDefinitionRule.cs>

[Required References]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/RequiredReferencesAssemblyDefinitionRule.cs>

[Sort Precompiled References]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/SortPrecompiledReferencesAssemblyDefinitionRule.cs>

[Sort References By Name]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/SortReferencesByNameAssemblyDefinitionRule.cs>

[CreateAssetMenuAttribute]:<https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html>
