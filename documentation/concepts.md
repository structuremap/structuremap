<!--Title: Software Design Concepts-->
<!--Url: software-design-concepts-->

## Inversion of Control

Years ago I consulted for a company that had developed a successful software engine for pricing and analyzing potential energy trades. The next step for them was to adapt their pricing engine so that it could be embedded in other software packages or even a simple spreadsheet so that analysts could quickly try out "what if" scenarios before making any kind of deal. The immediate problem this firm had was that their pricing engine was archtected such that the pricing engine business logic directly invoked their proprietary database schema and configuration files. The strategic pricing engine logic was effectively useless without all the rest of their system, so forget embedding the logic into spreadsheet logic.

With the benefit of hindsight, if we were to build an energy trading pricing engine from scratch, we would probably opt to use the software design concept of _[Inversion of Control](https://en.wikipedia.org/wiki/Inversion_of_control)_ such that the actual pricing logic code would be handed all the pricing metadata it needed to perform its work instead of making the pricing logic reach out to get it. In its most general usage, _Inversion of Control_ simply means that a component is given some sort of dependent data or service or configuration instead of that component having to "know" how to fetch or find that resource. 

An IoC container like StructureMap uses the _Inversion of Control_ concept to simplify your internal services by freeing them from having to know how to find, build, or clean up their dependencies.


## Dependency Injection

_[Dependency Injection](https://en.wikipedia.org/wiki/Dependency_injection)_ is nothing more than pushing dependencies of an object into constructor functions or setter properties instead of that object doing everything for itself. If you are strictly using _Dependency Injection_ to fill the dependencies of your classes, your code should have no coupling to StructureMap itself.

<[sample:basic-dependency-injection]>

## Service Locator

StructureMap also fills the role of a _[service locator](https://en.wikipedia.org/wiki/Service_locator_pattern)_. In this usage, your code would directly access StructureMap's `Container` class to build or resolve services upon demand like this sample:

<[sample:basic-service-location]>

Since IoC tools like StructureMap have come onto the software scene, many developers have very badly overused the service locator pattern and many other developers have become very vocal in their distaste for service location. The StructureMap team simply recommends that you favor Dependency Injection wherever possible, but that *some* service location in your system where you may need more advanced building options or lazy resolution of services is probably just fine.

