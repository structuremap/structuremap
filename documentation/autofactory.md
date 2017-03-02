<!--title:Auto-factories-->
<div class="alert alert-info" role="alert">You need to install <a href="https://www.nuget.org/packages/StructureMap.Autofactory">StructureMap.Autofactory</a> package to use the functionality described below.</div>

When you need to implement [Abstract Factory](http://en.wikipedia.org/wiki/Abstract_factory_pattern), StructureMap offers a way to do it for you. Let's say you have
<[sample:IDummyService]>
with implementation
<[sample:DummyService]>

Now you declare an interface for your factory:

<[sample:ISimpleDummyFactory]>

All you need to do is to call `CreateFactory` when configuring the container as shown below:

<[sample:simple-factory]>



## Default convention

As for now, Auto-factories support two types of methods: 
1. Methods to list all names of registered implementations.
1. Methods to create object instances i.e. factory methods.

To declare a method that has to return the names of registered implementations, the method signature must satisfy the following conditions:
1. The name has to start with `GetNames`
2. It must be a generic method.
3. The method return type must be assignable from `List<string>` e.g. `IList<string>`, `IEnumerable<string>`

Any other method that has the return type (i.e. doesn't return `void`), is treated as a factory method. In addition, if the method name starts with `GetNamed`, the first method argument is used as the name for the named instance. All the rest arguments are passed as <[linkto:registration/inline-dependencies;title=explicit arguments]> to the implementation constructor.

It is much easier to see it on an example:

<[sample:IDummyFactory]>

## Custom convention

If the default convention doesn't work for you, you can create and use your custom convention. All you need is to implement `IAutoFactoryConventionProvider` and use the corresponding `CreateFactory` overload. `IAutoFactoryConventionProvider` has a single method to implement:

```
IAutoFactoryMethodDefinition GetMethodDefinition(MethodInfo methodInfo, IList<object> arguments);
```

`IAutoFactoryMethodDefinition` is defined as follows:
<[sample:IAutoFactoryMethodDefinition]>