<!--Title: Working with Enumerable Types-->
<!--Url: working-with-enumerable-types-->



While you can certainly use *any* `IEnumerable` type as a plugin type with your own explicit configuration, 
StructureMap has *some* built in support for these specific enumerable types:

1. `IEnumerable<T>`
1. `IList<T>`
1. `List<T>`
1. `ICollection<T>`
1. `T[]`

Specifically, if you request one of these types either directly with `GetInstance<IList<IWidget>>()` or as a declared
dependency in a constructor or setter (`new WidgetUser(IList<IWidgets> widgets)` for example) and you have no
specific registration for the enumerable types, StructureMap has a built in policy to return all the registered instances
of `IWidget` **in the exact order that the registrations were made to StructureMap**. 

Note, if there are not any registrations for whatever `T` is, you'll get an empty enumeration.

Here's an acceptance test from the StructureMap codebase that demonstrates this:

<[sample:EnumerableFamilyPolicy_in_action]>

And another showing how you can override this behavior with explicit configuration:

<[sample:explicit-enumeration-behavior]>

## Sample Usage: Validation Rules

One of the ways that I have used the built in `IEnumerable` handling is for extensible validation rules. Say that we are
building a system to process `IWidget` objects. As part of processing a widget, we first need to validate that widget with a
series of rules that we might model with the `IWidgetValidator` interface shown below and used within the main
`WidgetProcessor` class:

<[sample:IWidgetValidator-enumerable]>

We *could* simply configure all of the `IWidgetValidator` rules in one place with an explicit registration of `IEnumerable<IWidgetValidator>`,
but what if we need to have an extensibility to add more validation rules later? What if we want to add these additional rules in addon packages? Or we 
just don't want to continuously break into the centralized `Registry` class every single time we add a new validation rule? 

By relying on StructureMap's `IEnumerable` behavior, we're able to split our `IWidgetValidatior` registration across multiple `Registry` classes and that's not infrequently useful to do.








