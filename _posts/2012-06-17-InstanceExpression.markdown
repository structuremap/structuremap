---
title: Configuring Instances
layout: default
---

One of the most common tasks in the [Registry DSL](RegistryDSL.htm) is
defining how a objects will be created by configuring
[Instances](Glossary.htm).  While you can create a custom Instance class, out of
the box StructureMap provides all of the most common scenarios -- and some that
probably aren't that common.  See [Using Open Generic Types](Generics.htm) for
more specific information about configuring open generic types.

Instance Expression
---------------------------------

In an effort to standardize the Registry DSL and make the API more predictable
and consistent, we have introduced the `InstanceExpression` as a shared
[Expression Builder](http://martinfowler.com/dslwip/ExpressionBuilder.html)
that is used in every place that the Registry DSL needs to define an Instance. 
Here are several examples of how and when the `InstanceExpression` is invoked. 
In the code sample below, the text `**********;` represents a call to an
`InstanceExpression`.


{% highlight csharp %}
public class InstanceExampleRegistry : Registry
{
    public InstanceExampleRegistry()
    {
        // Set the default Instance of a PluginType
        ForRequestedType<IService>().TheDefault.Is.**********;

        // Add an additional Instance of a PluginType
        InstanceOf<IService>().Is.**********;

        // Add multiple additional Instances of a PluginType
        ForRequestedType<IService>().AddInstances(x =>
        {
            x.**********;

            x.**********;

            x.**********;
        });

        // Use the InstanceExpression to define the default Instance
        // of a PluginType within a Profile
        CreateProfile("Connected", x =>
        {
            x.Type<IService>().Is.**********;
        });
    }
}
{% endhighlight %}


#### InstanceExpression Operations

As of 2.5+, all operations in the Registry DSL that require the definition of an
Instance expose an option to use an `InstanceExpression` with these options:

{% highlight csharp %}
public interface IInstanceExpression<T> : span style="color: #2b91af;">IsExpression<T>
{
    // Attach an Instance object that is configured
    /// independently of the DSL.  This is an extensibility point
    void Instance(Instance instance);
    void IsThis(Instance instance);

    // Use a pre-built object
    LiteralInstance IsThis(T obj);
    LiteralInstance Object(T theObject);

    // Use a type and build with its constructor function
    SmartInstance<PLUGGEDTYPE> OfConcreteType<PLUGGEDTYPE>() where PLUGGEDTYPE : T;
    ConfiguredInstance OfConcreteType(Type type);

    // Build by a Lambda or an Anonymous Delegate
    ConstructorInstance<T> ConstructedBy(Func<T> func);
    ConstructorInstance<T> ConstructedBy(Func<IContext, T> func);

    // Refer to a named Instance
    ReferencedInstance TheInstanceNamed(string key);
    DefaultInstance TheDefault();

    // Use a cloned copy of the template
    PrototypeInstance PrototypeOf(T template);

    // Cache the template as a binary serialized blob
    SerializedInstance SerializedCopyOf(T template);

    // Load an ASCX control
    UserControlInstance LoadControlFrom(string url);
}
{% endhighlight %}

_New for StructureMap 2.5.2+ is the "Conditional" construction choice._

{% highlight csharp %}
// Conditional object construction
ConditionalInstance<T> Conditional(
    Action<ConditionalInstance<T>.ConditionalInstanceExpression&<T>> configuration);
{% endhighlight %}


 Building with Constructors and Setters
---------------------------------

We received a lot of feedback on the Registry DSL introduced in 2.0.  Because of
that feedback, StructureMap 2.5+ contains a new feature called the
`SmartInstance` that is just a better Fluent Interface for specifying
constructor and setter arguments and dependencies of a concrete type.  The main
complaint with the 2.0 API was that `SetProperty()` is overloaded to mean either
setter or constructor arguments.  The underlying mechanisms in StructureMap
stores the information the same way, but the API was causing real confusion. 
The `SmartInstance` API is designed to eliminate the confusion over constructor
vs setter arguments, and also to take advantage of newer .Net 3.5 features.


#### Designating the Type

The first step is to define the actual concrete type of this Instance by using
the `OfConcreteType<T>()` method on `InstanceExpression` to start the
definition of a `SmartInstance`.

{% highlight csharp %}
// Add an additional Instance of a PluginType
InstanceOf<IService>().Is.OfConcreteType<RemoteService>();
{% endhighlight %}


#### Defining primitive constructor arguments

Primitive arguments like strings or value types (including enumerations) are
defined with the `WithCtorArg(name)` expression.  In this example below, the
Thing class has several constructor arguments.

{% highlight csharp %}
public Thing(int count, string name, double average, Rule rule)
{
    _count = count;
    _name = name;
    _average = average;
    _rule = rule;
}
{% endhighlight %}

In order to make a `SmartInstance` for the `Thing` type, I need to specify
values for all of the simple constructor arguments ("count", "name", and
"average").  That syntax is shown in the sample below taken from a unit test:


#### Defining primitive setter properties

You have additional options for setting a primitive setter property.  The
simplest approach is to just use a Lambda expression that will be applied to the
object as soon as it’s built.  Intellisense and compiler safety are good things,
so you might as well use it.  StructureMap now supports optional setter
injection, meaning that you no longer need to do the `[Setter]` attributes in the
concrete classes.  If you specify the value of a setter, StructureMap will use
that value regardless of whether or not the `[Setter]` property exists.  The same
rule applies to non-primitive setter dependencies.


Let's say I have a class like this:


{% highlight csharp %}
public class SimplePropertyTarget
{
    public string Name { get; set; }
    public int Age { get; set; }
}
{% endhighlight %}


To set the setter properties in the configuration you can use the
`SetProperty(Action<T>)` expression like this to set as many properties as you
need:


{% highlight csharp %}
var container = new Container(x =>
{
    x.ForConcreteType<SimplePropertyTarget>().Configure
        .SetProperty(target =>
        {
            target.Name = "Max";
            target.Age = 4;
        });
});
{% endhighlight %}


#### Overriding Constructor Dependencies

When you want to override the auto wiring behavior, you can explicitly specify
the constructor dependency of an Instance by using the `CtorDependency<T>()`
expression:

{% highlight csharp %}
var theContainer = new Container(registry =>
{
    registry.InstanceOf<Rule>().Is.OfConcreteType<WidgetRule>().WithName(instanceKey)
        .CtorDependency<IWidget>().Is(i =>
        {
            i.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("Orange").WithName("Orange");
        });
});
{% endhighlight %}

Or, if a concrete type should have two or more dependencies of the same type,
you can also specify the name of the constructor argument:


{% highlight csharp %}
registry.InstanceOf<Rule>().Is.OfConcreteType<WidgetRule>().WithName("RuleThatUsesMyInstance")
    .CtorDependency<IWidget>("widget").Is(x => x.TheInstanceNamed("Purple"));
{% endhighlight %}


The Lambda expression inside of `CtorDependency().Is()` is another
`InstanceExpression`, so all the normal options for defining an Instance are
available.  Please note that I just specified the type of constructor argument
in the call to `CtorDependency<T>()` above.  If you have more than one
constructor argument of the same type, you'll need to specify the name of the
constructor argument with the `CtorDependency<T>(argumentName)` method.

Setter dependencies are configured much like constructor dependencies:

{% highlight csharp %}
registry.ForRequestedType<Thing>().Use<Thing>()
    .WithCtorArg("name").EqualTo("Jeremy")
    .WithCtorArg("count").EqualTo(4)
    .WithCtorArg("average").EqualTo(.333)
    .SetterDependency<Rule>().Is(x =>
    {
        x.OfConcreteType<WidgetRule>().SetterDependency<IWidget>().Is(
            c => c.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("yellow"));
    });
{% endhighlight %}

The only difference is that Setter dependencies can also be specified by using
an Expression to declare the property to be injected:

{% highlight csharp %}
var container = new Container(x =>
{
    x.ForConcreteType<ClassWithWidgetProperty>().Configure
        .SetterDependency<IWidget>(o => o.Widget).Is(o => o.Object(new ColorWidget("Red")));
});
{% endhighlight %}


#### Array or Non Primitive Dependencies


StructureMap supports Array arguments of non primitive objects.  An array of
dependencies or type T can be defined with the `TheArrayOf<T>()` method shown
below.

{% highlight csharp %}
var container = new Container(x =>/p> 
{
    x.ForRequestedType<Processor>().Use<Processor>()
        .WithCtorArg("name").EqualTo("Jeremy")
ArrayOf<IHandler>().Contains(y =>
        {
            y.OfConcreteType<Handler1>();
            y.OfConcreteType<Handler2>();
            y.OfConcreteType<Handler3>();
        });
});
{% endhighlight %}


`TheArrayOf<T>()` method is another Nested Closure that takes in an
`Action<IInstanceExpression>` to configure the child instances in the array. 


#### Primitive Arrays and Dictionaries

Arrays of primitive types (strings, numbers, etc) and Dictionary types can be
configured just by treating them as a child dependency that is configured
inline.  Let's say you have a class like this that needs a couple Array objects
in its constructor function:

{% highlight csharp %}
public class ClassWithStringAndIntArray
{
    private readonly int[] _numbers;
    private readonly string[] _strings;

    public ClassWithStringAndIntArray(int[] numbers, string[] strings)
    {
        _numbers = numbers;
        _strings = strings;
    }

    public int[] Numbers
    {
        get { return _numbers; }
    }

    public string[] Strings
    {
        get { return _strings; }
    }
}
{% endhighlight %}

You can configure the values for the "numbers" and "strings" constructor
function arguments like this:


{% highlight csharp %}
[Test]
public void specify_a_string_array()
{
    var container = new Container(x =>
    {
        x.ForConcreteType<ClassWithStringAndIntArray>().Configure
            .CtorDependency<string[]>().Is(new[] {"a", "b", "c"})
            .CtorDependency<int[]>().Is(new[] {1, 2, 3});
    });

    var objectWithArrays = container.GetInstance<ClassWithStringAndIntArray>();
    objectWithArrays.Numbers.ShouldEqual(new[] {1, 2, 3});
    objectWithArrays.Strings.ShouldEqual(new[] {"a", "b", "c"});
} 
{% endhighlight %}


Using a Custom Instance
---------------------------------

Occasionally, it might be advantageous to create a custom form of Instance. 
Fortunately, there's a mechanism in the `InstanceExpression` to attach Instance
objects built independently of the Registry DSL.  Let's say that you've created
a custom Instance called `XmlFileInstance<T>` that deserializes an object from
an Xml file.  The registration of that custom Instance might look something like
this:     

{% highlight csharp %}
public class CustomInstanceRegistry : Registry
{
    public CustomInstanceRegistry()
    {
        ForRequestedType<Address>().TheDefault.IsThis(new XmlFileInstance<Address>("address1.xml"));
    }
}
{% endhighlight %}

See Extending StructureMap for more specific information on using custom Instance
classes.

Using an Externally Constructed Object 
--------------------------------------

It's frequently useful to simply
inject an existing object into StructureMap.  This frequently arises in desktop
applications where parts of the application shell may be created independently
of the container, but you still with these objects to be injected into other
services.  Here's an example from my current project where we need to
programmatically build up some services (`IUrlRegistry`, `IActionRegistry`, and
`ITypeRegistry`) before injecting them into StructureMap:

{% highlight csharp %}
private readonly TypeRegistry _typeRegistry = new TypeRegistry();
private UrlGraph _graph;
private ControllerRegistry _registry;

public WebRegistry(ICollection<RouteBase> routes)
{
    createUrlGraph(routes);

    ForRequestedType<IUrlRegistry>().TheDefault.Is.Object(_graph);
    ForRequestedType<IActionRegistry>().TheDefault.Is.Object(_actionRegistry);
    ForRequestedType<ITypeRegistry>().TheDefault.Is.Object(_typeRegistry);
}
{% endhighlight %}

The `Object()` method is the pertinant method in the
`InstanceExpression`.  There is also an alternative syntax called "`IsThis`":

{% highlight csharp %}
var container = new Container(x =>
{
    x.ForRequestedType<ClassWithDependency>().Use<ClassWithDependency>()
        .TheArrayOf<Rule>().Contains(arr =>
        {
            arr.IsThis(new ColorRule("Red"));
        });
});
{% endhighlight %}

Choosing `IsThis` or `Object` is
strictly a matter of aesthetics.  Also see
[Changing Configuration at Runtime](ChangingConfigurationAtRuntime.htm)
for some related behavior.

Constructing Objects with Lambda Functions 
------------------------------------------

Some classes simply cannot be created
with constructor functions.  Other objects may need to be located by invoking
some sort of other infrastructure (the built in Provider model in ASP.Net 2.0+
is a good example of this).  In those cases, StructureMap can instead build or
find these objects by registering a Lambda expression with the `ConstructedBy()`
method of `InstanceExpression`.  Let's say that your system includes an
important legacy class named `WeirdLegacyRepository` that manages its own
lifecycle somehow, and new instances can only be accessed by a static method on
the `WeirdLegacyRepository`.  You still want to inject these objects into the
classes that depend on `WeirdLegacyRepository`, so you might do something like
this code below:


{% highlight csharp %}
ForRequestedType<IRepository>().TheDefault.Is.ConstructedBy(() =>
WeirdLegacyRepository.Current).WithName("Weird"); 
{% endhighlight %}

That example used a no argument Lambda function.  You can also register a Lambda
expression that uses the current [`BuildSession`/`IContext`](UsingSessionContext.htm)
to access other services:            

{% highlight csharp %}
ForRequestedType<ISession>().TheDefault.Is.ConstructedBy(
    context => context.GetInstance<ISessionSource>().CreateSession());

ForRequestedType<ITransaction>().TheDefault.Is.ConstructedBy(
    context => context.GetInstance<ISession>().Transaction);
{% endhighlight %}

See [Using the Session Context](UsingSessionContext.htm) for more information.

Referencing a Named Instance
----------------------------

Many times it is helpful to simply say "use the
Instance with this name here" when configuring array dependencies or overriding
the autowiring defaults.  Use the `TheInstanceNamed()` function as shown below
to do this:             

{% highlight csharp %}
IContainer manager = new Container(r =>
{
    r.InstanceOf<IHandler>().Is.OfConcreteType<Handler1>().WithName("One");
    r.InstanceOf<IHandler>().Is.OfConcreteType<Handler2>().WithName("Two");

    r.ForRequestedType<Processor>().Use<Processor>()
        .WithCtorArg("name").EqualTo("Jeremy")
        .TheArrayOf<IHandler>().Contains(x =>
        {
            x.TheInstanceNamed("Two");
            x.TheInstanceNamed("One");
        });
});
{% endhighlight %}

Using the Default Instance 
--------------------------

Sometimes you may want to just tell
StructureMap to use the default Instance of a type.  The most common usage of
this is telling StructureMap to inject an optional setter property.  Use the
`IsTheDefault()` or `TheDefault()` method to force the injection of a setter
on an Instance by Instance basis.

{% highlight csharp %}
[Test]
public void AutoFill_a_property()
{
    var container = new Container(r =>
    {
        r.ForConcreteType<ClassWithDependency>().Configure
            .SetterDependency<Rule>().IsTheDefault();

        r.ForRequestedType<Rule>().TheDefault.Is.Object(new ColorRule("Green"));
    });


    container.GetInstance<ClassWithDependency>().Rule.ShouldBeOfType(typeof (ColorRule));
}
{% endhighlight %}

While this functionality will be supported in the
future, it may be better to use the new [Setter Injection
Policies](`ConstructorAndSetterInjection`.htm). 

Specifying a Prototype Object with Cloning 
------------------------------------------

You can have StructureMap construct objects by having it simply
clone a supplied template object:             

{% highlight csharp %}
// Build an instance for IWidget, then setup StructureMap to return cloned instances of the
// "Prototype" (GoF pattern) whenever someone asks for IWidget named "Jeremy"
var theWidget = new CloneableWidget("Jeremy");

container = new Container(x =>
{
    x.InstanceOf<IWidget>().Is.PrototypeOf(theWidget).WithName("Jeremy");
});
{% endhighlight %}

There is also the option to use binary serialization as the
"cloning" method.  The old advice from Microsoft is to use cloning for flat
objects, and favor serialization copying for objects with lots of children.


Specifying a Prototype Object with Serialization 
------------------------------------------------

You can have StructureMap
construct objects by having it simply clone a supplied template object by using
binary serialization:             

{% highlight csharp %}
// Build an instance for IWidget, then setup StructureMap to return cloned instances of the
// "Prototype" (GoF pattern) whenever someone asks for IWidget named "Jeremy"
var theWidget = new CloneableWidget("Jeremy");

container = new Container(x =>
{
    x.InstanceOf<IWidget>().Is.SerializedCopyOf(theWidget).WithName("Jeremy");
});
{% endhighlight %}


Configuring Conditional Construction 
------------------------------------

There have been several
questions on the StructureMap users list about doing conditional construction
(i.e., return this object if this condition, else this other object).  In order
to meet this apparent need, StructureMap 2.5.2 introduces the new
`ConditionalInstance` that allows a user to effectively switch the active
Instance based on a `Predicate<IContext>` boolean test.  Here's a quick example
of using the new `Conditional()` syntax of `InstanceExpression`:

{% highlight csharp %}
var container = new Container(x =>
{
    x.InstanceOf<Rule>().Is.Conditional(o =>
    {
        o.If(c => false).ThenIt.Is.OfConcreteType<ARule>();
        o.If(c => true).ThenIt.IsThis(GREEN);
        o.TheDefault.IsThis(RED);
    }).WithName("conditional");
});
{% endhighlight %}

The syntax above is configuring
and attaching a `ConditionalInstance` object.  Internally, this syntax is
telling the `ConditionalInstance` to: 

1. Return the concrete type "`ARule`" if the
  condition "c => false" is met (in real usage the predicate would do something
  more intelligent ;-) ) 
2. Else, return a Rule object specified by the variable
  named GREEN if the condition "c => true" is met 
3. Finally, if none of the predicates match, return the Rule object specified 
  by the variable named RED 

The syntax `If( predicate ).*************` uses an `InstanceExpression` and all
possible Instance types are available.

Internally, the `ConditionalInstance` looks like this:

{% highlight csharp %}
public class ConditionalInstance<T> : ExpressedInstance<ConditionalInstance<T>>
{
    // Conditional Instance keeps track of zero or more internal Instance
    // objects against a Predicate<IContext> condition
    private readonly List<InstanceCase> _cases = new List<InstanceCase>();

    // The "default" Instance to use if none of the conditional predicates
    // are met.  If this is not explicitly defined, the ConditionalInstance
    // will simply look for the default Instance of the desired
    // PluginType
    public Instance _default = new DefaultInstance();
}

public class InstanceCase
{
    public Predicate<IContext> Predicate { get; set; }
    public Instance Instance { get; set; }
}
{% endhighlight %}

When a call is made
to container.`GetInstance<Rule>("conditional")`: 

1. Internally, the Container
  object finds the `ConditionalInstance` object that was configured and named
  "conditional" for the `PluginType` "Rule"
2. The Container invokes the
  `ConditionalInstance.Build(Type, BuildSession)` method 
3. The `ConditionalInstance`
  evaluates its `InstanceCase` collection to find the first `InstanceCase` that
  matches the current `IContext` and invokes the internal Instance of that
  `InstanceCase`
4. Lastly, if `ConditionalInstance` does not find any matching
  `InstanceCase` objects, it will invoke its default Instance to build the
  requested object 

It might be easier to just see the code for this:        

{% highlight csharp %}
protected override object build(Type pluginType, BuildSession session)
{
    // Find the first InstanceCase that matches the BuildSession/IContext
    var instanceCase = _cases.Find(c => c.Predicate(session));

    // Use the Instance from the InstanceCase if it exists,
    // otherwise, use the "default"
    var instance = instanceCase == null ? _default : instanceCase.Instance;

    // delegate to the chosen Instance
    return instance.Build(pluginType, session);
}
{% endhighlight %}

Please see
[Using the Session Context](UsingSessionContext.htm) for more information on
what is possible with the `IContext`.

