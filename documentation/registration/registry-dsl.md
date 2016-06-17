<!--Title: Registry DSL-->
<!--Url: registry-dsl-->

Creating `Registry` classes is the recommended way of using the Registry DSL. 

The Registry DSL is mostly a [fluent interface][1] with some nested [closure][2] 
usage. The intent of the Registry DSL is to make the configuration process as 
error free as possible by using "compiler safe" expressions and defensive 
programming to point out missing data.

## The Registry Class

On all but the smallest systems, the main unit of configuration will probably be 
the `Registry` class.  Typically, you would subclass the `Registry` class, then 
use the [fluent interface](https://en.wikipedia.org/wiki/Fluent_interface) methods exposed by the Registry class to create Container 
configuration. Here's a sample `Registry` class below used to configure an 
instance of an `IWidget` interface:

<[sample:simple-registry]>

## Including Registries

The next question is "how does my new `Registry` class get used?" 

When you set up a `Container`, you need to simply direct the 
`Container` to use the configuration in that `Registry` class:

<[sample:including-registries]>


## _Use_ versus _Add_

Registrations in the Registry DSL can be done with either `Add()` or `Use()` methods, but they have
a different semantic meaning to StructureMap. `Add()` means _add another Instance to this plugin type_
while `Use()` means _this one is the default_.

One of the things that is different about StructureMap is that if it has multiple registrations of any
given plugin type, one of these registrations has to be explicitly marked as **the** default usage for that plugin type
or StructureMap will blow up in the call to `Container.GetInstance()`. Other IoC tools will magically use
the first registration or the last registration (and some even allow you to configure that behavior). We chose to 
make that determination be explicit. 

As of StructureMap 3.0, the <[linkto:diagnostics/whatdoihave;title=WhatDoIHave()]> output is part of any exception thrown by StructureMap when
it cannot determine a default registration for a requested type if there is more than one registration for that
type.

If there are multiple calls to `Use()` for the same plugin type, the last one wins. For more control over this behavior in
modularity scenarios, see <[linkto:registration/fallback-services]> and <[linkto:registration/clear-or-replace]>.




## For().Use()

To register the default `Instance` of a type, the syntax is one of the `Registry.For().Use()` overloads shown below:

<[sample:SettingDefaults]>

## For().Add()

To register additional `Instances` for a plugin type, use one of the overloads of `For().Add()`:

<[sample:AdditionalRegistrations]>

## Add Many Registrations with For().AddInstances()

If you need to add several `Instances` to a single plugin type, the `AddInstances()` syntax
shown below may be quicker and easier to use:

<[sample:Using-AddInstances]>


## Named Instances

When you have multiple implementations of an interface, it can often be useful to
name instances. To retrieve a specific implementation:

<[sample:named-instance]>

You can also register named instances with the following shorthand:

<[sample:named-instances-shorthand]>

[1]: http://martinfowler.com/bliki/FluentInterface.html
[2]: http://en.wikipedia.org/wiki/Closure_%28computer_programming%29

