<!--Title: Construction Policies-->

<div class="alert alert-info" role="alert">The features described in this page were all introduced in StructureMap 4.0.</div>

StructureMap has long supported <[linkto:registration/auto-registration-and-conventions;title=conventional policies for registration based on type scanning]>
and 3.0 introduced cleaner mechanisms for <[linkto:interception-and-decorators;title=interception policies]>.
The 4.0 release extends StructureMap's support for conventional build policies with a new mechanism for altering
_how_ object instances are built based on user created _meta-conventions_ using the `IInstancePolicy` shown below:

<[sample:IInstancePolicy]>

These policies are registered as part of the <[linkto:registration/registry-dsl;title=registry dsl]> with
the `Policies.Add()` method:

<[sample:policies.add]>

The `IInstancePolicy` mechanism probably works differently than other IoC containers in that the policy
is applied to the container's underlying configuration model instead of at runtime. Internally, StructureMap lazily creates
a "<[linkto:diagnostics/build-plans;title=build plan]>" for each configured Instance at the first time
that that Instance is built or resolved. As part of creating that build plan, StructureMap runs
all the registered `IInstancePolicy` objects against the Instance in question to capture any
potential changes before "baking" the build plan into a .Net `Expression` that is then compiled into a `Func`
for actual construction.

The `Instance` objects will give you access to the types being created, the configured name of the Instance (if any),
the ability to add interceptors and to modify the lifecycle. If you wish to add inline dependencies to
Instances that are built by calling constructor function and setter properties, you may find it easier
to use the `ConfiguredInstancePolicy` base class as a convenience:

<[sample:ConfiguredInstancePolicy]>


For more information, see:

* <[linkto:diagnostics/build-plans]>
* <[linkto:registration/configured-instance]>
* <[linkto:object-lifecycle]>



## A Quick Note on Using Policies Sanely



## Example 1: Constructor arguments

So let me say upfront that I don't like this approach, but other folks have asked for this ability
over the years. Say that you have some legacy code where many concrete classes have a constructor argument
called "connectionString" that needs to be the connection string to the application database like these classes: 

<[sample:database-users]>


Instead
of explicitly configuring every single concrete class in StructureMap with that inline constructor argument,
we can make a policy to do that in one place:

<[sample:connectionstringpolicy]>

Now, let's use that policy against the types that need "connectionString" and see what happens:

<[sample:use_the_connection_string_policy]>


Years ago StructureMap was knocked by an "IoC expert" for not having this functionality. I said at the
time -- and still would -- that I would strongly recommend that you simply **don't directly
open database connections** in more than one or a very few spots in your code anyway. If I did
need to configure a database connection string in multiple concrete classes, I prefer [strong typed configuration](http://jeremydmiller.com/2014/11/07/strong_typed_configuration/).




## Example 2: Connecting to Databases based on Parameter Name

From another common user request over the years, let's say that your application needs to connect to 
multiple databases, but your data access service in both cases is an interface called `IDatabase`, and
that's all the consumers of any database should ever need to know.

To make this concrete, let's say that our data access is all behind an interface and concrete class pair named
`Database/IDatabase` like so:

<[sample:IDatabase]>

For a registration policy, let's say that the parameter name of an `IDatabase` dependency
in a constructor function should match an identifier of one of the registered `IDatabase` services.
That policy would be:

<[sample:InjectDatabaseByName]>

And because I'm generally pretty boring about picking test data names, let's say that two of
our databases are named "red" and "green" with this container registration below:

<[sample:choose_database_container_setup]>

For more context, the classes that use `IDatabase` would need to have constructor functions like
these below:

<[sample:database-users]>


Finally, we can exercise our new policy and see it in action:

<[sample:inject-database-by-name-in-usage]>



**How I prefer to do this** - my strong preference would be to use separate interfaces for the different
databases even if that type is just an empty type marker that implements the same base.
I feel like using separate interfaces makes the code easier to trace and understand than trying
to make StructureMap vary dependencies based on naming conventions or what namespace a concrete type
happens to be in. At least now though, you have the choice of my way or using policies based on
naming conventions.





## Example 3: Make objects singletons based on type name

Unlike the top two examples, this is taken from a strategy that I used in [FubuMVC](http://github.com/darthfubumvc/fubumvc)
for its service registration. In that case, we wanted any concrete type whose name ended with
"Cache" to be a singleton in the container registration. With the new `IInstancePolicy` feature in StructureMap 4,
we could create a new policy class like so:

<[sample:CacheIsSingleton]>
    
Now, let's say that we have an interface named `IWidgets` and a single implementation called `WidgetCache` that
should track our widgets in the application. Using our new policy, we should see `WidgetCache` being
made a singleton:
  
<[sample:set_cache_to_singleton]>