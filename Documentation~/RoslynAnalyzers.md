# Coimbra Framework

Some analyzers are provided to guide how to use this framework correctly. The analyzers are divided according to the assembly they belong to.

| IDs       | Assemblies                    |
|-----------|-------------------------------|
| 0001-0099 | Coimbra</br>Coimbra.Listeners |
| 0101-0199 | Coimbra.Services              |
| 0201-0299 | Coimbra.Services.Events       |

> Actually all IDs begin with `COIMBRA`. This was omitted in the tables for brevity.

## Current Analyzers

| ID   | Title                                                                                                 | Severity | Code Fix |
|------|:------------------------------------------------------------------------------------------------------|----------|----------|
| 0001 | Type with SharedManagedPoolAttribute should not be generic.                                           | Error    | No       |
| 0002 | ProjectSettingsAttribute and PreferencesAttribute should not be used together.                        | Error    | No       |
| 0003 | ScriptableSettings has an invalided FileDirectory.                                                    | Error    | No       |
| 0004 | ScriptableSettings attributes are not supported on abstract types.                                    | Error    | No       |
| 0005 | ScriptableSettings attributes are not supported on generic types.                                     | Error    | No       |
| 0006 | Use CreateShared method for any SharedManagedPool.                                                    | Error    | Planned  |
| 0007 | Use constructor when not a SharedManagedPool.                                                         | Error    | Planned  |
| 0008 | Object.Destroy should not be used with Objects that can be an Actor.                                  | Error    | No       |
| 0009 | Type with CopyBaseConstructorsAttribute should be partial.                                            | Warning  | Yes      |
| 0010 | Type with CopyBaseConstructorsAttribute should not be nested.                                         | Error    | Yes      |
| 0011 | Type with SharedManagedPoolAttribute should be partial.                                               | Warning  | Yes      |
| 0012 | Type with SharedManagedPoolAttribute should not be nested.                                            | Error    | Yes      |
| 0101 | ServiceLocator APIs requires an interface type as generic parameter.                                  | Error    | No       |
| 0102 | ServiceLocator APIs requires an interface type without AbstractServiceAttribute as generic parameter. | Error    | No       |
| 0103 | Concrete IService should only implement one IService at a time.                                       | Error    | No       |
| 0104 | Concrete IService should not implement any IService with AbstractServiceAttribute.                    | Warning  | No       |
| 0105 | Type can't implement any IService because parent class already implements one.                        | Error    | No       |
| 0106 | Concrete IService should not be a Component unless it inherit from Actor.                             | Error    | No       |
| 0107 | A ScriptableSettings should not implement any IService.                                               | Error    | No       |
| 0108 | ServiceLocator.GetChecked should only be used on services with RequiredServiceAttribute.              | Error    | No       |
| 0109 | ServiceLocator.GetChecked is preferred on services with RequiredServiceAttribute.                     | Warning  | No       |
| 0110 | ServiceLocator.Set should not be used on a service without DynamicServiceAttribute.                   | Error    | No       |
| 0201 | Concrete IEvent should be partial.                                                                    | Warning  | Yes      |
| 0202 | Concrete IEvent should not be a nested type.                                                          | Error    | Yes      |
| 0203 | Class events should be either abstract or sealed.                                                     | Error    | No       |
| 0204 | IEventService generic APIs should not be used directly.                                               | Error    | No       |

## Planned Analyzers

| ID   | Title                                                                      | Severity | Code Fix |
|------|:---------------------------------------------------------------------------|----------|----------|
| 0013 | MonoBehaviour callbacks should be replaced with their equivalent listener. | Error    | No       |
| 0014 | GameObject API should be replaced by Actor API.                            | Error    | Planned  |
| 0015 | Component methods should not be used with Actor types.                     | Error    | Planned  |
| 0016 | Object.Instantiate should not be used with Objects that can be an Actor.   | Error    | No       |
| 0017 | Attribute being used with incompatible field type.                         | Error    | No       |
| 0018 | ManagedField being used with incompatible type parameter.                  | Error    | No       |
