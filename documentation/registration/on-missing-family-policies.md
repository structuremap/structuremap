<!--Title: On Missing Family Policies-->
<!--Url: on-missing-family-policies-->


New for StructureMap 3.0 is a feature to create missing service registrations at runtime based on pluggable rules using the new `IFamilyPolicy`
interface:

<[sample:IFamilyPolicy]>

Internally, if you make a request to `IContainer.GetInstance(type)` for a type that the active `Container` does not recognize, StructureMap will next
try to apply all the registered `IFamilyPolicy` policies to create a `PluginFamily` object for that plugin type that models the registrations for that plugin type, including the default, additional named instances, interceptors or decorators, and lifecycle rules. 

The simplest built in example is the `EnumerableFamilyPolicy` shown below that can fill in requests for `IList<T>`, `ICollection<T>`, and `T[]` with a collection of all the known registrations of the type `T`:

<[sample:EnumerableFamilyPolicy]>

The result of `EnumerableFamilyPolicy` in action is shown by the acceptance test below:

<[sample:EnumerableFamilyPolicy_in_action]>

See also the <[linkto:the-container/handling-missing-named-instances]> for runtime determination of named instances within a known plugin type.







## Built In Policies

StructureMap and StructureMap.AutoMocking use several `IFamilyPolicy` rules internally to create default behavior. In all cases, any custom
`IFamilyPolicy` rule that you explicitly add to a `Container` will be evaluated before the built in policies.

1. [CloseGenericFamilyPolicy](https://github.com/structuremap/structuremap/blob/master/src/StructureMap/Graph/CloseGenericFamilyPolicy.cs) - 
   uses the registration for an open type as a template to create the registrations for a closed type the first time StructureMap encounters
   that closed type. See <[linkto:generics]> for more information.
1. [FuncBuildByNamePolicy](https://github.com/structuremap/structuremap/blob/master/src/StructureMap/Graph/FuncBuildByNamePolicy.cs) - 
   Used internally to create registrations for `Func&lt;string, T&gt;` builders.
1. [EnumerableFamilyPolicy](https://github.com/structuremap/structuremap/blob/master/src/StructureMap/Graph/EnumerableFamilyPolicy.cs) - 
   Shown above.
1. [AutoMockedContainer](https://github.com/structuremap/structuremap/blob/master/src/StructureMap/AutoMocking/AutoMockedContainer.cs) - 
   Used by StructureMap.AutoMocking to create registrations on the fly that just return a mock object for the requested plugin type.



## Using a Custom IFamilyPolicy" id="custom">

FubuMVC 2.0 (still unreleased to the public as of yet, but in production usage) uses a [custom family policy](https://github.com/DarthFubuMVC/fubumvc/blob/master/src/FubuMVC.StructureMap/Settings/SettingPolicy.cs) in its StructureMap
integration to _auto-resolve_ concrete configuration types like the following type:

<[sample:SomeSettings]>

Unless the system using this object has explicitly registered `SomeSettings`, we want StructureMap to resolve this object by
using data from the basic .Net [appSettings collection](http://msdn.microsoft.com/en-us/library/aa903313(v=vs.71).aspx) to create a `SomeSettings` object.

For the sake of the example, assume that you have a functioning service that implements this interface below:

<[sample:ISettingsProvider]>

Assuming that `ISettingsProvider` is registered in your StructureMap `Container`, you could then craft a custom
`IFamilyPolicy` class like this:

<[sample:SettingPolicy]>

`SettingPolicy` is able to create a registration on the fly for any concrete type whose name ends in "Settings" and has a default, no arg
constructor.

To use register the custom `SettingPolicy`, use one of the `Registry.Policies.OnMissingFamily()` methods:

<[sample:registering-custom-family-policy]>


You can see the real implementation of the `SettingPolicy` in action in [its integration tests on GitHub](https://github.com/DarthFubuMVC/fubumvc/blob/master/src/FubuMVC.StructureMap/Settings/SettingPolicy.cs).



