# [Coimbra Framework](Index.md): Service Locator

Alternative way to design your code without needing the singleton pattern.

Why avoid singletons?

- It violates the single responsibility principle as it controls both the creation and lifecycle of the instance.
- Makes code more tightly coupled as consumers will often depend on implementation details, which makes testing difficulty.
- Hard to integrate with third-party code, often requiring even more coupling to achieve that.
- Hides the dependency chain of your application as the instances are globally accessible from anywhere.

With [ServiceLocator] the creation is controlled by the [IServiceFactory] (with some default ones offered) and the implementation details is hidden behind the public interface defined as an [IService].
As it doesn't relies in specific inheritance chain it also makes easier to integrate with third-party code, which can be consumed privately in a given service implementation.

However, I haven't solved the dependency chain issue as `Unity` itself doesn't support an easy way to explicit forward dependencies without hurting encapsulation.
So unless you are going to avoid [GameObject], [MonoBehaviour], and [ScriptableObject] in your project you are stuck with that issue anyway.
If you are committed to avoid those types, then I recommend you to take a look into some `Dependency Injection` framework instead or consider moving to `Entity Component System` framework.

In the end, there is no silver bullet. It is a design choice that should be made according to the project needs, which is why it is a separated module of the framework.

## Implementing Services

To implement a new service you need the following steps:

- Define a new interface that extends [IService] with the public APIs for your service. This is what other parts of the application will access through the [ServiceLocator] APIs.
- Provide an implementation for this interface. You can use a plain C# class for that (recommended) or inherit from [Actor] if it is a service that should have its lifecycle tied to the [Scene].
- (Optional) apply [DisableDefaultFactoryAttribute] to the implementation class and provide your custom [IServiceFactory] implementation for your [IService] type through the [ServiceLocator] API.
- (Optional) decorate the definition interface and the implementation class with the other attributes available. See [IService] documentation to check which ones are available.

> Do not inherit from [Actor] just to expose things in the inspector. Instead, use [ScriptableSettings] with your services to reference assets or expose configurations in the inspector.

## Default Services

This package comes with some default services implementations. They are all production-ready and provide good example of how to create your own services efficiently.

- [EventService](EventService.md): implemented as plain C# class. Check the linked documentation for further details.
- [PoolService](Pooling.md#poolservice): implemented as [Actor] as it would require a persistent [Transform] anyway to hold the persitent pools. Check the linked documentation for further details.
- [CoroutineService](../Coimbra.Services.Coroutines/ICoroutineService.cs): implemented as [Actor] as only [MonoBehaviour] have access to [Coroutine] APIs.
- [TimerService](../Coimbra.Services.Timers/ITimerService.cs): implemented as [Actor] as only [MonoBehaviour] have access to [Invoke] and [InvokeRepeating] APIs.
- [ApplicationStateService](../Coimbra.Services.ApplicationStateEvents/IApplicationStateService.cs): implemented as [Actor] as only [MonoBehaviour] have access to `OnApplicationXXX` callbacks.
- [PlayerLoopService](../Coimbra.Services.PlayerLoopEvents/IPlayerLoopService.cs): implemented as [Actor] as only [MonoBehaviour] have access to player loop callbacks.

> You might notice that most of those services are [Actor], which seems to go against the recommended approach.
> However you can see that each one has a clear reasoning behind this decision.

[Actor]:<Actor.md>

[ScriptableSettings]:<ScriptableSettings.md>

[DisableDefaultFactoryAttribute]:<../Coimbra.Services/DisableDefaultFactoryAttribute.cs>

[IService]:<../Coimbra.Services/IService.cs>

[IServiceFactory]:<../Coimbra.Services/IServiceFactory.cs>

[ServiceLocator]:<../Coimbra.Services/ServiceLocator.cs>

[Coroutine]:<https://docs.unity3d.com/ScriptReference/Coroutine.html>

[GameObject]:<https://docs.unity3d.com/ScriptReference/GameObject.html>

[Invoke]:<https://docs.unity3d.com/ScriptReference/MonoBehaviour.Invoke.html>

[InvokeRepeating]:<https://docs.unity3d.com/ScriptReference/MonoBehaviour.InvokeRepeating.html>

[MonoBehaviour]:<https://docs.unity3d.com/ScriptReference/MonoBehaviour.html>

[ScriptableObject]:<https://docs.unity3d.com/ScriptReference/ScriptableObject.html>

[Scene]:<https://docs.unity3d.com/ScriptReference/SceneManagement.Scene.html>

[Transform]:<https://docs.unity3d.com/ScriptReference/Transform.html>
