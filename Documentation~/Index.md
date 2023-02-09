# Coimbra Framework

Package of general utilities to be used with Unity development.

## List of Content

> Only the core features have a more extensive documentation as their implementation can be complex to understand by just looking into the code.
> For anything else, a link to the given script is provided as they already contain enough XML documentation in all public APIs about their implementation.

- Core Features:
    - [Actor](Actor.md)
    - [Assembly Definition Rules](AssemblyDefinitionRules.md)
    - [Editor Tools](EditorTools.md)
    - [Event Service](EventService.md)
    - [Pooling](Pooling.md)
    - [Property Attributes](PropertyAttributes.md)
    - [Roslyn Analyzers](RoslynAnalyzers.md)
    - [Service Locator](ServiceLocator.md)
    - [Scriptable Settings](ScriptableSettings.md)
- Custom Serializable Types:
    - [AssetReferenceScene](../Coimbra/AssetReferenceScene.cs)
    - [DelegateListener](../Coimbra/DelegateListener.cs)
    - [FloatRange](../Coimbra/FloatRange.cs)
    - [IntRange](../Coimbra/IntRange.cs)
    - [ManagedField](../Coimbra/ManagedField`1.cs)
    - [Reference](../Coimbra/Reference`1.cs)
    - [SerializableType](../Coimbra/SerializableType`1.cs)
    - [SerializableTypeDictionary](../Coimbra/SerializableTypeDictionary`3.cs)
- GUI Scopes:
    - [BackgroundColorScope](../Coimbra.Editor/GUIScopes/BackgroundColorScope.cs)
    - [HierarchyModeScope](../Coimbra.Editor/GUIScopes/HierarchyModeScope.cs)
    - [LabelWidthScope](../Coimbra.Editor/GUIScopes/LabelWidthScope.cs)
    - [ResetIndentLevelScope](../Coimbra.Editor/GUIScopes/ResetIndentLevelScope.cs)
    - [ShowMixedValueScope](../Coimbra.Editor/GUIScopes/ShowMixedValueScope.cs)
- Managed Jobs:
    - [IManagedJob](../Coimbra.Jobs/IManagedJob.cs)
    - [IManagedJobParallelFor](../Coimbra.Jobs/IManagedJobParallelFor.cs)
- Particle System Listeners:
    - [ParticleSystemParticleCollisionListener](../Coimbra.Listeners/ParticleSystem/ParticleSystemParticleCollisionListener.cs)
    - [ParticleSystemStoppedListener](../Coimbra.Listeners/ParticleSystem/ParticleSystemStoppedListener.cs)
    - [ParticleTriggerListener](../Coimbra.Listeners/ParticleSystem/ParticleTriggerListener.cs)
    - [ParticleUpdateJobScheduledListener](../Coimbra.Listeners/ParticleSystem/ParticleUpdateJobScheduledListener.cs)
- Physics Listeners:
    - [ColliderParticleCollisionListener](../Coimbra.Listeners/Physics/ColliderParticleCollisionListener.cs)
    - [CollisionEnterListener](../Coimbra.Listeners/Physics/CollisionEnterListener.cs)
    - [CollisionExitListener](../Coimbra.Listeners/Physics/CollisionExitListener.cs)
    - [CollisionStayListener](../Coimbra.Listeners/Physics/CollisionStayListener.cs)
    - [ControllerColliderHitListener](../Coimbra.Listeners/Physics/ControllerColliderHitListener.cs)
    - [JointBreakListener](../Coimbra.Listeners/Physics/JointBreakListener.cs)
    - [TriggerEnterListener](../Coimbra.Listeners/Physics/TriggerEnterListener.cs)
    - [TriggerExitListener](../Coimbra.Listeners/Physics/TriggerExitListener.cs)
    - [TriggerStayListener](../Coimbra.Listeners/Physics/TriggerStayListener.cs)
- Physics 2D Listeners:
    - [Collider2DParticleCollisionListener](../Coimbra.Listeners/Physics2D/Collider2DParticleCollisionListener.cs)
    - [ColliderOverlap2DListener](../Coimbra.Listeners/Physics2D/ColliderOverlap2DListener.cs)
    - [CollisionEnter2DListener](../Coimbra.Listeners/Physics2D/CollisionEnter2DListener.cs)
    - [CollisionExit2DListener](../Coimbra.Listeners/Physics2D/CollisionExit2DListener.cs)
    - [CollisionStay2DListener](../Coimbra.Listeners/Physics2D/CollisionStay2DListener.cs)
    - [JointBreak2DListener](../Coimbra.Listeners/Physics2D/JointBreak2DListener.cs)
    - [RigidbodyOverlap2DListener](../Coimbra.Listeners/Physics2D/RigidbodyOverlap2DListener.cs)
    - [TriggerEnter2DListener](../Coimbra.Listeners/Physics2D/TriggerEnter2DListener.cs)
    - [TriggerExit2DListener](../Coimbra.Listeners/Physics2D/TriggerExit2DListener.cs)
    - [TriggerStay2DListener](../Coimbra.Listeners/Physics2D/TriggerStay2DListener.cs)
- Rendering Listeners:
    - [AnimatorIKListener](../Coimbra.Listeners/Rendering/AnimatorIKListener.cs)
    - [AnimatorMoveListener](../Coimbra.Listeners/Rendering/AnimatorMoveListener.cs)
    - [BecameInvisibleListener](../Coimbra.Listeners/Rendering/BecameInvisibleListener.cs)
    - [BecameVisibleListener](../Coimbra.Listeners/Rendering/BecameVisibleListener.cs)
    - [PostRenderListener](../Coimbra.Listeners/Rendering/PostRenderListener.cs)
    - [PreCullListener](../Coimbra.Listeners/Rendering/PreCullListener.cs)
    - [PreRenderListener](../Coimbra.Listeners/Rendering/PreRenderListener.cs)
    - [RenderImageListener](../Coimbra.Listeners/Rendering/RenderImageListener.cs)
    - [RenderObjectListener](../Coimbra.Listeners/Rendering/RenderObjectListener.cs)
    - [WillRenderObjectListener](../Coimbra.Listeners/Rendering/WillRenderObjectListener.cs)
- Scene Processing:
    - [ISceneProcessorComponent](../Coimbra/ISceneProcessorComponent.cs)
    - [IScenePostProcessorComponent](../Coimbra/IScenePostProcessorComponent.cs)
- Transform Listeners:
    - [BeforeTransformParentChangedListener](../Coimbra.Listeners/Transform/BeforeTransformParentChangedListener.cs)
    - [CanvasGroupChangedListener](../Coimbra.Listeners/Transform/CanvasGroupChangedListener.cs)
    - [CanvasHierarchyChangedListener](../Coimbra.Listeners/Transform/CanvasHierarchyChangedListener.cs)
    - [RectTransformDimensionsChangeListener](../Coimbra.Listeners/Transform/RectTransformDimensionsChangeListener.cs)
    - [TransformChangedListener](../Coimbra.Listeners/Transform/TransformChangedListener.cs)
    - [TransformChildrenChangedListener](../Coimbra.Listeners/Transform/TransformChildrenChangedListener.cs)
    - [TransformParentChangedListener](../Coimbra.Listeners/Transform/TransformParentChangedListener.cs)
- Utilities and other types:
    - [AssetReferenceComponentRestriction](../Coimbra/AssetReferenceComponentRestriction.cs)
    - [CopyBaseConstructorsAttribute](../Coimbra/CopyBaseConstructorsAttribute.cs)
    - [FormerlySerializedAsBackingFieldOfAttribute](../Coimbra/FormerlySerializedAsBackingFieldOfAttribute.cs)
    - [ImageHitTestInitializer](../Coimbra.UI/ImageHitTestInitializer.cs)
    - [PlayerLoopEventListener](../Coimbra.Services.PlayerLoopEvents/Listeners/PlayerLoopEventListener.cs)
    - [ApplicationUtility](../Coimbra/Utilities/ApplicationUtility.cs)
    - [CancellationTokenSourceUtility](../Coimbra/Utilities/CancellationTokenSourceUtility.cs)
    - [DelegateUtility](../Coimbra/Utilities/DelegateUtility.cs)
    - [GameObjectUtility](../Coimbra/Utilities/GameObjectUtility.cs)
    - [ListUtility](../Coimbra/Utilities/ListUtility.cs)
    - [ObjectUtility](../Coimbra/Utilities/ObjectUtility.cs)
    - [PathUtility](../Coimbra/Utilities/PathUtility.cs)
    - [PropertyPathInfoUtility](../Coimbra/Utilities/PropertyPathInfoUtility.cs)
    - [RectUtility](../Coimbra/Utilities/RectUtility.cs)
    - [ReflectionUtility](../Coimbra/Utilities/ReflectionUtility.cs)
    - [ScriptableSettingsTypeUtility](../Coimbra/Utilities/ScriptableSettingsTypeUtility.cs)
    - [TypeUtility](../Coimbra/Utilities/TypeUtility.cs)
    - [ActorUtility](../Coimbra.Editor/Utilities/ActorUtility.cs)
    - [EngineUtility](../Coimbra.Editor/Utilities/EngineUtility.cs)
    - [ReorderableListUtility](../Coimbra.Editor/Utilities/ReorderableListUtility.cs)
    - [ScriptableSettingsUtility](../Coimbra.Editor/Utilities/ScriptableSettingsUtility.cs)
    - [SerializedPropertyUtility](../Coimbra.Editor/Utilities/SerializedPropertyUtility.cs)

[Addressables]:<https://docs.unity3d.com/Manual/com.unity.addressables.html>

[GameObject]:<https://docs.unity3d.com/ScriptReference/GameObject.html>

[PropertyAttribute]:<https://docs.unity3d.com/ScriptReference/PropertyAttribute.html>

[PropertyDrawer]:<https://docs.unity3d.com/ScriptReference/PropertyDrawer.html>

[ScriptableObject]:<https://docs.unity3d.com/ScriptReference/ScriptableObject.html>
