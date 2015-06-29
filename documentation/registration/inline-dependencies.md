<!--Title: Inline Dependencies-->
<!--Url: inline-dependencies-->

While you generally allow StructureMap to just use _auto-wiring_ to fill the dependencies of a concrete type, there are times
when you may want to explicitly configure individual dependencies on a case by case basis. In the case of _primitive_ types
like strings or numbers, StructureMap **will not** do any auto-wiring, so it's incumbent upon you the user to supply the dependency.

Let's say we have a simple class called `ColorWidget` like the following:

<[sample:inline-dependencies-ColorWidget]>

To register the `ColorWidget`, you would supply the value of the `color` parameter to the constructor function like so:

<[sample:inline-dependencies-value]>

<div class="alert alert-info" role="alert">
The most common reason to define string arguments to classes seems to be passing in database connection strings or occasional file paths. 
I used this strategy very commonly in the early days of StructureMap, but for the past 5-6 years I strongly prefer to use <a href="http://jeremydmiller.com/2014/11/07/strong_typed_configuration/"><i>Settings</i> classes</a>
to inject configuration through StructureMap.
</div>

## Event Condition Action Rules

The ability to explicitly define dependencies inline isn't commonly used these days, but was actually one of the very core use cases in the initial versions of StructureMap. One of the first usages of StructureMap in a production application was in a configurable rules engine using an <i><a href="http://en.wikipedia.org/wiki/Event_condition_action">Event-Condition-Action</a></i> architecture where the conditions and actions were configured in StructureMap as inline dependencies of _Rule_ objects. Using StructureMap's old Xml configuration, we could define rules for new customers by registering rule objects with the container that reused existing _condition_ and _action_ classes in new configurations.

To make that concrete and establish a sample problem domain, consider these types:

<[sample:inline-dependencies-rule-classes]>

Now, let's move on to seeing how we could use inline dependency configuration to register new rules.



## Constructor Parameters by Type

First off, let's say that we have a `SimpleRule` that takes a single condition and action:

<[sample:inline-dependencies-SimpleRule]>

Now, since `SimpleRule` has only a single dependency on both `IAction` and `ICondition`, we can create new rules by registering new Instance's
of `SimpleRule` with different combinations of its dependencies:

<[sample:inline-dependencies-simple-ctor-injection]>

The inline dependency configuration using the `Ctor&lt;T&gt;().Is()` syntax supports all the common StructureMap configuration options: define by type, by lambdas, by value, or if you really want to risk severe eye strain, you can use your own Instance objects and define the configuration of your dependency's dependencies.


## Specifying the Argument Name

If for some reason you need to specify an inline constructor argument dependency, and the concrete type has more than one dependency for that type, 
you just need to specify the parameter name as shown in this sample:

<[sample:inline-dependencies-ctor-by-name]>

## Setter Dependencies

You can also configure setter dependencies with a similar syntax, but with additional options to specify the property name 
by using an `Expression` as shown below:

<[sample:inline-dependencies-setters]>

<div class="alert alert-info" role="alert">The `Ctor` and `Setter` methods are just syntactic sugar. Both methods store data to the same underlying structure. </div>


## Enumerable Dependencies

TODO(show a sample of using enumerable dependencies)


## Programmatic Configuration outside of the Registry DSL

In some cases, you may want to skip the Registry DSL and go straight for the raw dependencies structures. Let's say that
we're using an open generic type for our rules engine so that we can respond to multiple event types:

<[sample:inline-dependencies-open-types]>

As an alternative approach, we _could_ build up `ConstructorInstance` objects to represent our rules like so:

<[sample:inline-dependencies-programmatic-configuration]>

It's frequently useful to explicitly configure all the elements for an enumerable argument (arrays, IEnumerable, or IList). 
StructureMap provides this syntax to do just that:

<[sample:inline-dependencies-enumerables]>


