<!--Title: A Gentle Quickstart-->
<!--Url: quickstart-->

The first thing you should know is that StructureMap (and other IoC tools like it) are designed to
make compositional and modular software designs easier to build by offloading the grubby mechanics of
resolving dependencies, reading configuration data, and assembling object graphs to the IoC tool instead
of cluttering up your application code.

Before you get started with StructureMap it's recommended that you first get comfortable with the <[linkto:concepts]> 
of [Dependency Injection](http://codebetter.com/jeremymiller/2005/10/06/the-dependency-injection-pattern-%E2%80%93-what-is-it-and-why-do-i-care/) 
and [Inversion Of Control](http://codebetter.com/jeremymiller/2005/09/20/what%E2%80%99s-so-great-about-inversion-of-control/). 
It's important that you structure and build your application with these concepts in mind to fully utilize StructureMap's abilities.

Assuming that you're already familiar with those concepts, or you'd really rather skip the pedantry and jump right into concrete code, the first thing to do is, go <[linkto:get-structuremap]> and jump into usage.

## Main Usage

By and large, you really only do two kinds of things with StructureMap:

1. Configure the container by registering the **what** and **how** StructureMap should build or find requested services based on a type and/or name.

2. Resolve object instances of a service or dependency built out with all of its dependencies.

So let's say that you have a simple object model like so:
<[sample:foobar-model]>

You could explicitly build a StructureMap `Container` object to build these types like this:

<[sample:foobar-quickstart1]>

or utilize StructureMap's type scanning conventions to configure the relationships and do the same thing like this:

<[sample:foobar-quickstart2]>





## Integrating StructureMap within your application

At some point you will want to integrate StructureMap into your application. Whether you are using Windows Presentation Foundation (WPF), FubuMVC, ASP.NET WebForms, ASP.NET MVC or any other framework or technology, you will have to do some sort of plumbing and bootstrapping. Depending on the used technology or framework there can be important integration points that you will have to use to fully enable the power of StructureMap.

While StructureMap doesn't provide integration support out of the box for all of the frameworks and technologies out there, we do find it important to help you get started with integrating StructureMap into your application. That said, StructureMap does provide integration support for [FubuMVC](https://fubumvc.github.io/) (a web framework, which is part of the same family as StructureMap).


## What to do when things go wrong?

StructureMap, and any other IoC tool for that matter, is configuration intensive, which means that their will be problems in that configuration. We're all moving to more convention based type registration, so more stuff is happening off stage and out of your sight, making debugging the configuration even trickier. Not to worry (too much), StructureMap has some diagnostic abilities to help you solve configuration problems:

- <[linkto:interpreting-exceptions]>
- <[linkto:diagnostics]>


## Need Help?

- There is a [google group](http://groups.google.com/group/structuremap-users?hl=en) for StructureMap support.
- You can ask questions on [StackOverflow](http://stackoverflow.com/) with the tag "structuremap."


