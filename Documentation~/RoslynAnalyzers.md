# Coimbra Framework

Some analyzers are provided to guide how to use this framework correctly.

## Analyzers Table

| ID          | Title                                                                                                 | Severity | Code Fix |
|-------------|:------------------------------------------------------------------------------------------------------|----------|----------|
| COIMBRA0001 | Concrete IEvent should be partial.                                                                    | Warning  | Yes      |
| COIMBRA0002 | Concrete IEvent should not be a nested type.                                                          | Error    | Yes      |
| COIMBRA0003 | Class events should be either abstract or sealed.                                                     | Error    | No       |
| COIMBRA0004 | ServiceLocator APIs requires an interface type as generic parameter.                                  | Error    | No       |
| COIMBRA0005 | ServiceLocator APIs requires an interface type without AbstractServiceAttribute as generic parameter. | Error    | No       |
| COIMBRA0006 | Concrete IService should only implement one IService at a time.                                       | Error    | No       |
| COIMBRA0007 | Concrete IService should not implement any IService with AbstractServiceAttribute.                    | Warning  | No       |
| COIMBRA0008 | Type with SharedManagedPoolAttribute should not be generic.                                           | Error    | No       |
| COIMBRA0009 | IEventService generic APIs should not be used directly.                                               | Error    | No       |
| COIMBRA0010 | Type can't implement any IService because parent class already implements one.                        | Error    | No       |
| COIMBRA0011 | Concrete IService should not be a Component unless it inherit from Actor.                             | Warning  | No       |
| COIMBRA0012 | A ScriptableSettings should not implement any IService.                                               | Error    | No       |
| COIMBRA0013 | ProjectSettingsAttribute and PreferencesAttribute should not be used together.                        | Error    | No       |
| COIMBRA0014 | ScriptableSettings has an invalided FileDirectory.                                                    | Error    | No       |
| COIMBRA0015 | ScriptableSettings attributes are not supported on abstract types.                                    | Error    | No       |
| COIMBRA0016 | ScriptableSettings attributes are not supported on generic types.                                     | Error    | No       |
| COIMBRA0017 | Use CreateShared method for any SharedManagedPool.                                                    | Error    | No       |
| COIMBRA0018 | Use constructor when not a SharedManagedPool.                                                         | Error    | No       |
| COIMBRA0019 | Object.Destroy should not be used with Objects that can be an Actor.                                  | Warning  | No       |
| COIMBRA0020 | Type with CopyBaseConstructorsAttribute should be partial.                                            | Warning  | Yes      |
| COIMBRA0021 | Type with CopyBaseConstructorsAttribute should not be nested.                                         | Error    | Yes      |
| COIMBRA0022 | Type with SharedManagedPoolAttribute should be partial.                                               | Warning  | Yes      |
| COIMBRA0023 | Type with SharedManagedPoolAttribute should not be nested.                                            | Error    | Yes      |
