<!--Title: Using Attributes for Configuration-->

In the early days of StructureMap we had several attributes for basic configuration that we'd just as soon
forget ever existed. Further more, the StructureMap strongly believes that the usage of StructureMap should
have as little impact on application code as possible -- and forcing users to spray .Net attributes all over 
their own code is in clear violation of this philosophy. _In other words, we don't want to be MEF._

That being said, there are plenty of times when simple attribute usage is effective for one-off deviations from
your normal registration conventions or cause less harm than having to constantly change a centralized `Registry` or
just seem more clear and understandable to users than a convention. For those usages, StructureMap 4.0 has introduced a
new base class that can be extended and used to explicitly customize your StructureMap configuration:

<[sample:StructureMapAttribute]>

There's a couple thing to note, here about this new attibute:

* StructureMap internals are looking for any attribute of the base class. Attributes that affect types are read and applied early, 
  while attributes decorating properties or constructor parameters are only read and applied during the creation of <[linkto:diagnostics/build-plans]>.
* Unlike many other frameworks, the attributes in StructureMap are not executed at build time. Instead, StructureMap uses attributes *one time*
  to determine the build plan. 

## Attribute Targeting Plugin Type or Concrete Type

Take the new `[Singleton]` attribute shown below:

<[sample:SingletonAttribute]>

This new attribute can be used on either the plugin type (typically an interface) or on a concrete type
to make an individual type registration be a singleton. You can see the usage on some types below:

<[sample:[Singleton]-usage]>



## Attribute Targeting Constructor Parameters or Setter Properties

As an example, let's say that you want a new attribute type that can decorate either properties or constructor parameters
to say "use the value from the old .Net AppSettings collection as the value for this property/parameter." To build that new
custom attribute, you would create a new attribute that subclasses `StructureMapAttribute` and override the two methods shown below:

<[sample:AppSettingAttribute]>

To test out the new attribute above, say we have a concrete type like this one that we
decorate with the new `[AppSetting]` attribute:

<[sample:AppSettingTarget]>

The following unit test demonstrates our new custom `[AppSetting]` attribute in action:

<[sample:using_parameter_and_property_attibutes]>

The <[linkto:diagnostics/build-plans;title=build plan]> for `AppSettingTarget` is determined by the active StructureMap 
container to be this:

<pre>
PluginType: StructureMap.Testing.Acceptance.attribute_usage+AppSettingTarget
Lifecycle: Transient
new AppSettingTarget(String name)
  ┗ String name = Value: Jeremy
Set String HomeState = Value: Missouri
</pre>

Note that the values retrieved from `AppSettings` are essentially hard coded into the underlying builder function that StructureMap compiled for 
`AppSettingTarget`. You *could* instead add a <[linkto:the-container/lambdas;title="lambda builder"]> dependency so that StructureMap had to 
use the live value for `AppSettings` as it constructs objects.

## Built In Attributes

StructureMap supplies a handful of built in attributes for customizing configuration:

 * `[ValidationMethod]` - Allows you to expose <[linkto:diagnostics/environment-tests]> in your StructureMap registrations
 * `[SetterProperty]` - See <[linkto:setter-injection]>
 * `[DefaultConstructor]` - Declare which constructor function should be used by StructureMap. See <[linkto:registration/constructor-selection]> for more information
 * `[AlwaysUnique]` and `[Singleton]` - These attributes, new for StructureMap 4.0, just add another mechanism for <[linkto:object-lifecycle/configuring-lifecycles;title=lifecycle configuration]>
