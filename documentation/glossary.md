<!--Title: Glossary-->


There are some terms that reoccur throughout the documentation and show up in the StructureMap API. Understanding these terms and how they relate to StructureMap isn't a prerequisite to using StructureMap, but it helps.

## Container

Tools like StructureMap are generally known as *Inversion of Control (IoC) Containers* or *Dependency Injection (DI) Containers*. In the Java world, they are also known as *Lightweight Containers* to differentiate them from the older *EJB Containers*.

A container is a tool that can help you compose object graphs and managing their scope (lifecycle). Although you can do Inversion of Control and Dependency Injection manually, using tools like StructureMap makes you far more productive and successful in doing so.

Obviously there is more to a container than resolving services and managing their scope, but in the core that's just what it is. Before you can do so you need to tell StructureMap, the container, how it must compose those objects graphs and what their lifecycle is. This is called registration and can be done in various mixed ways. The strongly recommend way would be using the <[linkto:registration/registry-dsl]>. In your registration, you're basically mapping abstractions to concrete types and defining their lifecycle.

A simple example of a container using the <[linkto:registration/registry-dsl]>:

<[sample:foobar-registry]>

<[sample:glossary-container]>

Because we didn't specify the lifecycle for both registrations, the default `Transient` lifecycle will be used.  This will instruct the container to create a new instance for every request for a plugin type `IFoo` or `IBar`. 

More advanced features that the container can do are things like: Interception, Auto-Wiring, Forwarding Types.  



## Nested Container

A nested container is used to mark the scope of short-lived transactions or web requests and track and clean up objects implementing the `IDisposable` interface for that operation.

You can ask an existing container to create a nested container for you, like in the following example:

<[sample:glossary-nested-container]>

For more detailed information about nested containers and their special properties, you can read the <[linkto:the-container/nested-containers]> topic.


## PluginType and PluggedType

The term plugin type is used throughout the code and documentation to mean the type that you want to register or resolve. More generally, this type is known as the service type. This type can be a concrete class or in most cases, it will be a form of abstraction like an abstract class or interface. 

The term plugged type means the actual concrete type that you get when you request the plugin type. This type must obviously implement the plugin type contract.

In your registration, you could have something like this:

<[sample:foo-registry]>

<[sample:glossary-pluggintype-and-plugged-type]>

If you request an object of `IFoo`, you'll get an instance of the `Foo` class. In this case, `IFoo` is the plugin type (what you're asking for) and `Foo` is the plugged type (the concrete class you'll get that fulfills, implements the plugin type contract).


## PluginFamily

This term you will not see so often because it's mostly used by StructureMap itself. A `PluginFamily` represents a [CLR](http://en.wikipedia.org/wiki/Common_Language_Runtime) type (the plugin type) that StructureMap can build, and all of the possible plugged types that implement the [CLR](http://en.wikipedia.org/wiki/Common_Language_Runtime) type.

In the following code, StructureMap internally creates one `PluginFamily` of the plugin type `IFoo` with two instances `Foo` and `SomeOtherFoo`, where `Foo` is the default instance because it's registered through `For<PLUGIN_TYPE>().Use<PLUGGED_TYPE>()`.

<[sample:glossary-pluginfamily]> 

Before StructureMap 3.0 you have probably seen the term used in an exception message when you request a plugin type that doesn't have a default instance defined.

	StructureMap Exception Code:  202
	No Default Instance defined for PluginFamily [plugin type]

This specific exception message is gone in 3.0 because the exception messages were modernized.


## Plugin Graph

A `PluginGraph` is the configuration model of the runtime configuration of a StructureMap container. The `PluginGraph` model can be manipulated directly in StructureMap 3.0 for
any kind of special convention that doesn't fit into the existing conventional support.


## Instance

In StructureMap terms, an "Instance" is a configured and named strategy to build or locate a named object instance for a requested Plugin Type.  An "Instance" does not automatically equate to a concrete type.  For example, let's say that we're building a system to automate a warehouse.  Our system might consume an interface called IShippingService that acts as a Gateway to various ways of shipping boxes out of our warehouse.

<[sample:IShippingService]>

Our warehouse system might have to interact with three types of shipping:  domestic, international, and intra-company or internal shipments.  The internal shipping service runs in process with the warehouse application, but domestic and international shipping is done by invoking external web services.  The registration of the IShippingService Instances might look like this:

<[sample:ShippingRegistry]>

In the registration code above, there are three "Instances."  You can access the various IShippingService Instance's by name:

<[sample:getting-ishippingservice]>

Asking for the "International" or the "Domestic" instance of IShippingService will both return an object of type ShippingWebService, but the two objects will be differently configured with unique Url's.

There is an actual class in StructureMap that represents an "[Instance](https://github.com/structuremap/structuremap/blob/master/src/StructureMap/Pipeline/Instance.cs)."  


When you call `Container.GetInstance<T>("the instance that I want")` or `Container.GetInstance<T>()`, the internal `Container` object is locating the correct Instance object and then using the Instance's internal _build plan_ to resolve or construct the actual object.

A StructureMap "Instance" is a close analogue for what many other IoC tools call a "Component."



## Lifecycle (or Scope)

The power of an IoC container isn't just in building object graphs for you, it's also about _scoping_ an object graph to what StructureMap calls a _lifecycle_.  Think of it this way:
when you ask StructureMap for a service or (much more commonly) when StructureMap is filling a dependency behind the scenes, do you want:

* A brand new, unique object each time?
* The exact same object as the rest of the graph is using?
* The exact same object every single time throughout the application?

## Registry

A `Registry` or a subclass of `Registry` is a class that lets you create reusable configuration for StructureMap containers.

## Profile

StructureMap 3.0 features a complete rewrite of the ancient _Profile_ functionality where you can create your base Container configuration with additional _Profile_ 
configuration that overrides one or more of the parent Container defaults.  The _Profile_ functionality was originally meant to handle differences between
development, testing, and production environments, but has been more commonly used for multi-tenancy situations.  Think of a _Profile_ as an application or tenant mode.

## Auto wiring

You'd never get anything done if you had to tell StructureMap how to build each and every constructor or setter dependency on every concrete class.  Fortunately, StructureMap like
most IoC container tools, supports the concept of _auto-wiring_ -- meaning that StructureMap can happily infer dependency requirements from constructor functions and setter
rules and fill those dependencies with the default configuration for the declared dependency type.

Let's just see it in action:

<[sample:auto-wiring-sample]>
