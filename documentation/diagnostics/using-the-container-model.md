<!--Title: Using the Container Model-->
<!--Url: using-the-container-model-->

The queryable `Container.Model` is a power facility to look into your StructureMap `Container` and even to eject previously built services from the Container. The `Container.Model` represents a **static view of what a `Container` already has**, and does not account for policies that may allow StructureMap to validly discover types that it may encounter later at runtime.

## Querying for Plugin Types

The <[linkto:diagnostics/whatdoihave]> mechanism works by just iterating over the `Container.Model.PluginTypes` collection as shown below: 

<[sample:find-all-plugin-types-from-the-current-assembly]>


## Working with a Plugin Type

The `IPluginTypeConfiguration` interface allows you to query and manipulate all the configured Instance's for a given plugin type.

See the entire [IPluginTypeConfiguration interface here](https://github.com/structuremap/structuremap/blob/master/src/StructureMap/Query/IPluginTypeConfiguration.cs).


## Finding the Default for a Plugin Type

To simply find out what the default concrete type would be for a requested plugin type, use one of these two methods:

<[sample:find-default-of-plugintype]>


## Finding an Instance by Type and Name

Use this syntax to find information about an Instance in a given plugin type and name:

<[sample:find-named-instance-by-type-and-name]>


## All Instances for a Plugin Type

This sample shows how you could iterate or query over all the registered instances for a single plugin type:

<[sample:query-instances-of-plugintype]>


## Ejecting Services and/or Configuration

It's possible to remove singleton scoped objects from a running Container to force a new one to be built or even to completely remove configured instance objects and configuration permanently from a Container. This is typically only used in automated testing to flush static state between tests.

To remove and dispose previously built singleton scoped objects from a Container, use this code:

<[sample:eject-an-object]>


To completely eject any singletons and permanently remove all related configuration, use this code:

<[sample:eject-and-remove-configuration]>



## Testing for Registrations

To troubleshoot or automate testing of Container configuration, you can use code like the sample below to
test for the presence of expected configurations:

<[sample:testing-for-registrations]>





## Finding all Possible Implementors of an Interface

Forget about what types are registered for whatever plugin types and consider this, what if you have an interface called
`IStartable` that just denotes objects that will need to be activated after the container is bootstrapped?

If our interface is this below:

<[sample:istartable]>

We could walk through the entire StructureMap model and find every registered instance that implements this interface, create each, and call the `Start()` method like in this code below:

<[sample:calling-startable-start]>

I've also used this mechanism in automated testing to reach out to all singleton services that may have state to clear out their data between tests.


## Working with Individual Instances

You can manipulate an individual instance in several ways:

<[sample:working-with-single-instance-ref]>






