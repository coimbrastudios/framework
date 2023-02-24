# [Coimbra Framework](Index.md): Listeners

Here we call a listener any component provided just to replace the Unity magic callbacks.
There is a listener component for each existent callback.

## Custom Implementations

There are a few implementations that doesn't have a direct equivalent native callback:

- [Overlap2DListenerBase](../Coimbra.Listeners/Overlap2DListenerBase`1.cs)
    - [ColliderOverlap2DListener](../Coimbra.Listeners/Physics2D/ColliderOverlap2DListener.cs)
    - [RigidbodyOverlap2DListener](../Coimbra.Listeners/Physics2D/RigidbodyOverlap2DListener.cs)
- [PlayerLoopEventListenerBase](../Coimbra.Listeners/ParticleCollisionListenerBase`1.cs)
    - [PlayerLoopEventListener](../Coimbra.Services.PlayerLoopEvents/Listeners/PlayerLoopEventListener.cs)
- [TransformChangedListener](../Coimbra.Listeners/Transform/TransformChangedListener.cs)

## Particle System

All callbacks related to [ParticleSystem](https://docs.unity3d.com/ScriptReference/ParticleSystem.html).

- [ParticleCollisionListenerBase](../Coimbra.Listeners/ParticleCollisionListenerBase`1.cs)
    - [Collider2DParticleCollisionListener](../Coimbra.Listeners/Physics2D/Collider2DParticleCollisionListener.cs)
    - [ColliderParticleCollisionListener](../Coimbra.Listeners/Physics/ColliderParticleCollisionListener.cs)
    - [ParticleSystemParticleCollisionListener](../Coimbra.Listeners/ParticleSystem/ParticleSystemParticleCollisionListener.cs)
- [ParticleSystemStoppedListener](../Coimbra.Listeners/ParticleSystem/ParticleSystemStoppedListener.cs)
- [ParticleTriggerListener](../Coimbra.Listeners/ParticleSystem/ParticleTriggerListener.cs)
- [ParticleUpdateJobScheduledListener](../Coimbra.Listeners/ParticleSystem/ParticleUpdateJobScheduledListener.cs)

## Physics

All callbacks related to [Rigidbody](https://docs.unity3d.com/ScriptReference/Rigidbody.html) and [CharacterController](https://docs.unity3d.com/ScriptReference/CharacterController.html).

- [CollisionListenerBase.cs](../Coimbra.Listeners/CollisionListenerBase.cs)
    - [CollisionEnterListener](../Coimbra.Listeners/Physics/CollisionEnterListener.cs)
    - [CollisionExitListener](../Coimbra.Listeners/Physics/CollisionExitListener.cs)
    - [CollisionStayListener](../Coimbra.Listeners/Physics/CollisionStayListener.cs)
- [ControllerColliderHitListener](../Coimbra.Listeners/Physics/ControllerColliderHitListener.cs)
- [JointBreakListener](../Coimbra.Listeners/Physics/JointBreakListener.cs)
- [TriggerListenerBase](../Coimbra.Listeners/TriggerListenerBase.cs)
    - [TriggerEnterListener](../Coimbra.Listeners/Physics/TriggerEnterListener.cs)
    - [TriggerExitListener](../Coimbra.Listeners/Physics/TriggerExitListener.cs)
    - [TriggerStayListener](../Coimbra.Listeners/Physics/TriggerStayListener.cs)

## Physics 2D

All callbacks related to [Rigidbody2D](https://docs.unity3d.com/ScriptReference/Rigidbody2D.html).

- [Collision2DListenerBase.cs](../Coimbra.Listeners/Collision2DListenerBase.cs)
    - [CollisionEnter2DListener](../Coimbra.Listeners/Physics2D/CollisionEnter2DListener.cs)
    - [CollisionExit2DListener](../Coimbra.Listeners/Physics2D/CollisionExit2DListener.cs)
    - [CollisionStay2DListener](../Coimbra.Listeners/Physics2D/CollisionStay2DListener.cs)
- [JointBreak2DListener](../Coimbra.Listeners/Physics2D/JointBreak2DListener.cs)
- [Trigger2DListenerBase](../Coimbra.Listeners/Trigger2DListenerBase.cs)
    - [TriggerEnter2DListener](../Coimbra.Listeners/Physics2D/TriggerEnter2DListener.cs)
    - [TriggerExit2DListener](../Coimbra.Listeners/Physics2D/TriggerExit2DListener.cs)
    - [TriggerStay2DListener](../Coimbra.Listeners/Physics2D/TriggerStay2DListener.cs)

## Rendering

All callbacks related to [Animator](https://docs.unity3d.com/ScriptReference/Animator.html) and [Camera](https://docs.unity3d.com/ScriptReference/Camera.html).

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

## Transform & UI

All callbacks related to [Transform](https://docs.unity3d.com/ScriptReference/Transform.html) and [UI](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.UIBehaviour.html).

- [BeforeTransformParentChangedListener](../Coimbra.Listeners/Transform/BeforeTransformParentChangedListener.cs)
- [CanvasGroupChangedListener](../Coimbra.Listeners/Transform/CanvasGroupChangedListener.cs)
- [CanvasHierarchyChangedListener](../Coimbra.Listeners/Transform/CanvasHierarchyChangedListener.cs)
- [RectTransformDimensionsChangeListener](../Coimbra.Listeners/Transform/RectTransformDimensionsChangeListener.cs)
- [TransformChildrenChangedListener](../Coimbra.Listeners/Transform/TransformChildrenChangedListener.cs)
- [TransformParentChangedListener](../Coimbra.Listeners/Transform/TransformParentChangedListener.cs)
