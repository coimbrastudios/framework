# Coimbra Framework: Assembly Definition Rules

Linting tools for assembly definition assets.

This is an editor-only feature that aims into keeping the project source code organized by analyzing all the assembly definition files after each compilation to ensure that they are following the defined rules and apply necessary fixes on those.

You can select the defined rules by going to `Project Settings/Linting Settings` and modifying the `Assembly Definition Rules` list. The rules will be applied in the same order as their are defined in the list, so order matters.

## Implementing Rules

To implement a new rule you need the following steps:

- Create a new rule by inheriting from [AssemblyDefinitionRuleBase](../Coimbra.Editor.Linting/AssemblyDefinitionRuleBase.cs) and override the `Apply` method.
- Create a new asset and configure it in the inspector.
- (Optional) Add `CreateAssetMenuAttribute` to make easier to create a new asset.

> You can also just create a new asset from one of the existing rules and customize the options. The default ones should give you enough control already for all the basic needs.

## Default Rules

This package comes with some default rules implementations. They are all production-ready and provide good example of how to create your own rules efficiently.

- [BannedReferencesAssemblyDefinitionRule]
- [FixDuplicateReferencesAssemblyDefinitionRule](../Coimbra.Editor.Linting/AssemblyDefinitionRules/FixDuplicateReferencesAssemblyDefinitionRule.cs)
- [FixEditorOnlyAssemblyDefinitionRule](../Coimbra.Editor.Linting/AssemblyDefinitionRules/FixEditorOnlyAssemblyDefinitionRule.cs)
- [FixTestsOnlyAssemblyDefinitionRule](../Coimbra.Editor.Linting/AssemblyDefinitionRules/FixTestsOnlyAssemblyDefinitionRule.cs)
- [ForceReferenceByNameAssemblyDefinitionRule](../Coimbra.Editor.Linting/AssemblyDefinitionRules/ForceReferenceByNameAssemblyDefinitionRule.cs)
- [ForceRootNamespaceMatchNameAssemblyDefinitionRule]
- [RequiredReferencesAssemblyDefinitionRule](../Coimbra.Editor.Linting/AssemblyDefinitionRules/RequiredReferencesAssemblyDefinitionRule.cs)
- [SortPrecompiledReferencesAssemblyDefinitionRule](../Coimbra.Editor.Linting/AssemblyDefinitionRules/SortPrecompiledReferencesAssemblyDefinitionRule.cs)
- [SortReferencesByNameAssemblyDefinitionRule](../Coimbra.Editor.Linting/AssemblyDefinitionRules/SortReferencesByNameAssemblyDefinitionRule.cs)

> If you are looking into an advanced example see [BannedReferencesAssemblyDefinitionRule] as it is the one that makes the most from the available APIs, including caching techniques to ensure a smooth editor experience. If you are looking into a very basic example see [ForceRootNamespaceMatchNameAssemblyDefinitionRule].

[BannedReferencesAssemblyDefinitionRule]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/BannedReferencesAssemblyDefinitionRule.cs>
[ForceRootNamespaceMatchNameAssemblyDefinitionRule]:<../Coimbra.Editor.Linting/AssemblyDefinitionRules/ForceRootNamespaceMatchNameAssemblyDefinitionRule.cs>
