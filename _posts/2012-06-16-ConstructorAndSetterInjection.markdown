---
title: Constructor and Setter Injection
layout: default
---

StructureMap supports two forms of Dependency Injection:

* Constructor Injection -- "Pushing" dependencies into a concrete class through
constructor arguments.
* Setter Injection -- "Pushing" dependencies into a concrete class through public
properties.  The "Setter" nomenclature is taken from Java where properties are
getSomething() and setSomething(value).

You can certainly mix and match Setter Injection with Constructor Injection on
the same classes, but Constructor Injection will always be used (except for
empty constructors) and Setter Injection has to be explicitly configured.  See [
Martin Fowler's discussion on Constructor versus Setter
Injection](http://martinfowler.com/articles/injection.html#ConstructorVersusSetterInjection)
for more information.  My feeling has always been that Constructor Injection is
preferrable from a design perspective because it creates a stronger, more
intention rea.  When you exclusively use Constructor Injection, the code is
somewhat more self-documenting because the constructor arguments will clearly
delineate the dependencies of a concrete class.  It's also important to think
about the constructor method of a class being a contract.  If you satisfy all of
the arguments of the constructor method, the class should be ready to function. 
Relying on Setter Injection can make a class harder to use because it isn't
always obvious which setters need to be created externally to use the class.  Of
course, not using any form of Dependency Injection can be the worst answer
because then you have no idea what it really takes to bootstrap the service.


Despite my personal distaste for Setter Injection, I gave into user demand and
greatly increased StructureMap's support for Setter Injection -- and promptly
found that support to be considerably more useful than I thought it would be. 
Mea culpa.


 


Using Constructor Injection
---------------------------------


Now, the first question you might ask is how does StructureMap know which
constructor function to use in a class that has multiple constructors?  The
answer is that StructureMap will automatically select the "greediest" public
constructor of a class to use for injection.  In this case, the "greediest"
constructor is the constructor with the most arguments.  In the case of a tie,
StructureMap will use the first constructor that it encountered.  For example,
in the code below, the second constructor that takes in two arguments would be
used because it is "greedier."


{% highlight csharp %}
public GreaterThanRule()
{
}
 
public GreaterThanRule(string Attribute, int Value)
{
    _Attribute = Attribute;
    _Value = Value;
}
  
{% endhighlight %}


StructureMap will only use constructor functions that are accessible to the
StructureMap assembly.  I've had a few complaints about this issue in the past
from people wanting to use internal constructors, but for now constructors have
to be either public or you have to use the `[InternalsVisibleTo]` attribute to
give StructureMap access to your own internal members.  StructureMap 2.5 is now
strongly signed (thanks to [Steve Harman](http://stevenharman.net/)) partially
for this scenario. 


#### Overriding the Constructor with an Attribute


You can always override this behavior by decorating the constructor you want
StructureMap to use with the `[StructureMap.DefaultConstructor]` attribute like
this sample:


{% highlight csharp %}
[DefaultConstructor]
public DataSession(IDatabaseEngine database)
    : this(database,
           new CommandFactory(database),
           new AutoCommitExecutionState(database.GetConnection(), database.GetDataAdapter()),
           new TransactionalExecutionState(database.GetConnection(), database.GetDataAdapter()))
{
}
{% endhighlight %}


#### Overriding the Constructor in the Registry DSL


StructureMap has always allowed you to override the constructor choice with an
attribute, but increasingly, many people are unwilling to use attributes in
their code for infrastructure concerns.  Other times you may want to override
the constructor choice of a class that you don't control.  Either way, it would
be useful to select the constructor function used by StructureMap to build a
concrete code in the Registry DSL.  The syntax to do just that is shown below:


Let's say that you have this class (from the unit tests):


{% highlight csharp %}
public class ClassWithTwoConstructors
{
    public ClassWithTwoConstructors(int age, string name)
    {
    }
 
    public ClassWithTwoConstructors(int age)
    {
    }
}
{% endhighlight %}


By default, StructureMap would choose the "greediest" constructor.  In this
case, it would be the constructor that takes in "age" and "name."  To force
StructureMap into using the other constructor, use the SelectConstructor()
method on the Registry:


{% highlight csharp %}
var container = new Container(x =>
{
    x.SelectConstructor<ClassWithTwoConstructors>(()=>new ClassWithTwoConstructors(0));
    x.ForConcreteType<ClassWithTwoConstructors>().Configure
        .WithCtorArg("age").EqualTo(34);
});
{% endhighlight %}


The argument to the SelectConstructor is an Expression of type
`Expression<Func<T>>` where T is the concrete class.  StructureMap parses the
Expression to find the constructor function in the Expression object.


Using Setter Injection
---------------------------------


Setter injection is a pattern of "injecting" dependencies via public
properties.  Setter Injection with StructureMap is somewhat a second class
citizen, but this is partially by design.  My strong recommendation is to use
constructor injection for all code that you control, and save setter injection
strictly for classes from external libraries where you do not have any control. 
As a summary, these are the Setter Injection features that are described in
detail below:


#### Configuring Primitive Setter Properties


Ok, now that you've read that I don't believe that setter injection is a good
idea, but you're bound and determined to use it anyway, let's talk about how to
do it with StructureMap.  Let's say we have this class with a string property
called "Name":


{% highlight csharp %}
public class OptionalSetterTarget
{
    public string Name { get; set;; }
}
{% endhighlight %}


I can specify the value of the "Name" property in the configuration API like
this:


{% highlight csharp %}
[Test] 
public void optional_setter_injection_with_string()
{
    var container = new Container(r =>
    {
        // The "Name" property is not configured for this instance
        r.InstanceOf<OptionalSetterTarget>().Is.OfConcreteType<OptionalSetterTarget>().WithName("NoName");

        // The "Name" property is configured for this instance
        r.ForConcreteType<OptionalSetterTarget>().Configure
            .WithProperty("Name").EqualTo("Jeremy");
    });
  
    container.GetInstance<OptionalSetterTarget>().Name.ShouldEqual("Jeremy");
    container.GetInstance<OptionalSetterTarget>("NoName").Name.ShouldBeNull();
}
{% endhighlight %}


In the case above I specified the value for the "Name" setter directly by
embedding the property value directly with the
`WithProperty("Name").EqualTo("Jeremy")`.  You could also retrieve the value for
the property from the AppSettings portion of a .Net application config file like
this with the WithProperty("Name").EqualToAppSetting("name") syntax.


{% highlight csharp %}
var container = new Container(r =>
{
    r.ForConcreteType<OptionalSetterTarget>().Configure
        .WithProperty("Name").EqualToAppSetting("name");
});
{% endhighlight %}


In both of the cases above the property name is designated by a string.  Using
strings to designate property names is always going to be somewhat problematic,
so there's a new option in 2.5 to specify property values with a Lambda
expression (inspired by [this post from Udi
Dahan](http://www.udidahan.com/2008/06/13/external-value-configuration-with-ioc/))
using an `Action<T>`.


{% highlight csharp %}
r.ForConcreteType<OptionalSetterTarget>().Configure
    .SetProperty(x => x.Name = "Jeremy");
{% endhighlight %}


The value of the Lamdba expression mechanism is that it makes the configuration
static-typed with all of the advantages that static typing brings.  This action
is executed on the object after creation.  Technically, this mechanism can be
used to do anything to the new object before StructureMap returns the new object
to the code that requested it.  There is no limitation to the number of
`Action<T>` handlers you can use.


#### Configuring Setter Dependencies


Let's say you have a class that has a public property for a singular dependency
and another public property for an array of dependencies.


{% highlight csharp %}
public class ClassWithDependency
{
    public Rule Rule { get; set; }
    public Rule[] Rules { get; set; }
}
{% endhighlight %}


I can explicitly configure what gets injected into the "Rule" property for a
specific Instance of ClassWithDependency like this with the
`.SetterDependency<T>()` method that will look for the first public setter of
type `T`:


{% highlight csharp %}
r.ForConcreteType<ClassWithDependency>().Configure
    .SetterDependency<Rule>().Is(new ColorRule("Red"));
{% endhighlight %}


or for cases where a class may have multiple public setters of the same type,
you can specify the exact property with an Expression (`.SetterDependency<T>(
expression )` ):


{% highlight csharp %}
var container = new Container(r =>
{
    r.ForConcreteType<ClassWithDependency>().Configure
        .SetterDependency<Rule>(x => x.Rule).Is(new ColorRule("Red"));
});
{% endhighlight %}


#### "Auto Filling" a Setter Dependency


Sometimes all you want to do is to simply say "fill in this property for me." 
That's what the code below does for the "Rule" property with the
`.SetterDependency<Rule>().IsTheDefault()` syntax:


{% highlight csharp %}
var container = new Container(r =>
{
    r.ForConcreteType<ClassWithDependency>().Configure
        .SetterDependency<Rule>().IsTheDefault();

    r.ForRequestedType<Rule>().TheDefault.Is.Object(new ColorRule("Green"));
});
{% endhighlight %}


For certain dependency types you might want StructureMap to automatically fill
any public property of that type.  The first usage that comes to mind is
logging.  Let's say that we have an interface for our logging support called
ILogger.  We can specify that any concrete class that has a public property for
the ILogger type will be filled in construction with code like this
(`FillAllPropertiesOfType<ILogger>()`):


{% highlight csharp %}
var container = new Container(r =>
{
    r.FillAllPropertiesOfType<ILogger>().TheDefault.Is
        .ConstructedBy(context => new Logger(context.ParentType));
});
{% endhighlight %}


Now, if I have some classes like:


{% highlight csharp %}
public class ClassWithLogger
{
    public ILogger Logger { get; set; }
}
  
public class ClassWithLogger2
{
    public ILogger Logger { get; set; }
}
{% endhighlight %}


Now, when StructureMap builds new instances of these classes above the Logger
properties will be filled automatically without any explicit configuration for
either type.  Here's a sample from the unit tests that constructs objects of
both `ClassWithLogger` and `ClassWithLogger2` and verifies that the "Logger"
property was filled for both types without any further configuration.


{% highlight csharp %}
container.GetInstance<ClassWithLogger>().Logger.ShouldBeOfType<Logger>();
container.GetInstance<ClassWithLogger2>().Logger.ShouldBeOfType<Logger>();
{% endhighlight %}


Applying Setter Injection to an Existing Object (BuildUp)
---------------------------------


Many times you simply cannot control when an object is going to be created
(ASP.Net WebForms), but you may still want to inject dependencies and even
primitive values into an already constructed object.  To fill this gap,
StructureMap 2.5.2+ introduces the `BuildUp()` method on Container and
ObjectFactory.  `BuildUp()` works by finding the default Instance for the concrete
type passed into the `BuildUp()` method (or create a new Instance if one does not
already exist), then applying any setters from that Instance configuration.  At
this time, StructureMap does not apply interception inside of `BuildUp()`.


Let's say that we have a class called "BuildTarget1" like this:


{% highlight csharp %}
public class BuildUpTarget1
{
    public IGateway Gateway { get; set; }
}
{% endhighlight %}


In usage, we'd like to have the Gateway dependency injected into a new instance
of the `BuildTarget1` class when we call `BuildUp()`:


{% highlight csharp %}
[Test]
public void create_a_setter_rule_and_see_it_applied_in_BuildUp_through_ObjectFactory()
{
    var theGateway = new DefaultGateway();
    ObjectFactory.Initialize(x =>
    {
        x.ForRequestedType<IGateway>().TheDefault.IsThis(theGateway);
  
        // First we create a new Setter Injection Policy that
        // forces StructureMap to inject all public properties
        // where the PropertyType is IGateway
        x.SetAllProperties(y =>
        {
            y.OfType<IGateway>();
        });
    });
  
    // Create an instance of BuildUpTarget1
    var target = new BuildUpTarget1();

    // Now, call BuildUp() on target, and
    // we should see the Gateway property assigned
    ObjectFactory.BuildUp(target);
  
    target.Gateway.ShouldBeTheSameAs(theGateway);
}
{% endhighlight %}


`BuildUp()` also works with primitive properties (but I'm not sure how useful this
will really be):


{% highlight csharp %}
public class ClassThatHasConnection
{
    public string ConnectionString { get; set; }
}
  
[TestFixture]
public class demo_the_BuildUp
{
    [Test]
    public void push_in_a_string_property()
    {
        // There is a limitation to this.  As of StructureMap 2.5.2,
        // you can only use the .WithProperty().EqualTo() syntax
        // for BuildUp()
        // SetProperty() will not work at this time.
        var container = new Container(x =>
        {
            x.ForConcreteType<ClassThatHasConnection>().Configure
                .WithProperty(o => o.ConnectionString).EqualTo("connect1");

        });
  
        var @class = new ClassThatHasConnection();
        container.BuildUp(@class);

        @class.ConnectionString.ShouldEqual("connect1");
    }
}
{% endhighlight %}


Creating Policies for Setter Injection
---------------------------------


New in StructureMap 2.5.2+ is the ability to create setter injection policies. 
What this means is that you create conventions to define which public setter
properties will be mandatory in the construction of objects.  Setter Injection
policies are set with the new "SetAllProperties" method in the Registry DSL:


{% highlight csharp %}
x.SetAllProperties(policy =>
{
    policy.Matching(prop =>
    {
        return prop.PropertyType.CanBeCastTo(typeof (IService))
               && !prop.Name.Contains("Ignore");
    });
});
{% endhighlight %}


In the end, all you're doing is telling StructureMap that any public Setter that
matches a `Predicate<PropertyInfo>` policy should be a mandatory Setter.  You
can make multiple declarations inside the [nested
closure](http://martinfowler.com/dslwip/NestedClosure.html) for
SetAllProperties.  Any additional calls to SetAllProperties() are purely
additive.


In the sections below we'll look at some helper methods inside
`SetAllProperties()`:


#### Specify a Setter Policy by Property Name


The sample below will make all public setter properties mandatory where the
property name is suffixed by "Service."  The call to NameMatches() takes in a
`Predicate<string>` that is a test against the property name.  The NameMatches()
method just applies a `Predicate<string>` test against the name of a public
setter.


{% highlight csharp %}
var container = new Container(x =>
{
    x.SetAllProperties(policy =>
    {
        policy.NameMatches(name => name.EndsWith("Service"));
    });
});
{% endhighlight %}


#### Specify a Setter Policy by Property Type


Setter injection policies can also be defined by a simple test against the
PropertyInfo.PropertyType.  Here's the shorthand method:


{% highlight csharp %}
var container = new Container(x =>
{
    x.SetAllProperties(policy =>
    {
        policy.TypeMatches(type => type == typeof (IService));
    });
});
{% endhighlight %}


The `OfType<T>` method is shorthand for:  `policy.Matching( property =>
typeof(T).IsAssignableTo(property.PropertyInfo) )`


{% highlight csharp %}
var container = new Container(x =>
{
    x.SetAllProperties(policy =>
    {
        policy.OfType<string>();
        policy.OfType<IGateway>();
    });
});
{% endhighlight %}


You can also specify that all setter dependencies where the property type is
inside a namespace should be a mandatory setter (this check holds true for
subfolders of a namespace).  This can be handy if you place all services in a
well known namespace.


{% highlight csharp %}
var container = new Container(x =>
{
    x.SetAllProperties(policy =>
    {
        policy.WithAnyTypeFromNamespaceContainingType<ClassWithNamedProperties>();
    });
});
{% endhighlight %}


Here's another way to specify the namespace:


{% highlight csharp %}
var container = new Container(x =>
{
    x.SetAllProperties(policy =>
    {
        policy.WithAnyTypeFromNamespace("StructureMap.Testing.Widget3");
    });
});
{% endhighlight %}


Defining Setter Properties with Attributes
---------------------------------


Just use the `[StructureMap.Attributes.SetterProperty]` to denote properties that
need to be filled by StructureMap.  Marking a property with the `[SetterProperty]`
makes the setter mandatory.  StructureMap will throw an exception if the
"ShouldCache" property isn't specified for the concrete type shown below.  If
the "Provider" property isn't explicitly configured, StructureMap will use the
default instance of IDataProvider for the "Provider" property (or throw an
exception if StructureMap doesn't know how to build the type IDataProvider).


{% highlight csharp %}
public class Repository
{
    private IDataProvider _provider;
  
    // Adding the SetterProperty to a setter directs
    // StructureMap to use this property when
    // constructing a Repository instance
    [SetterProperty]/p> 
    public IDataProvider Provider
    {
        set
        {
            _provider = value;
        }
    }
  
    [SetterProperty]
    public bool ShouldCache { get; set; }
}
{% endhighlight %}


Defining Setter Properties in Xml
---------------------------------


Setter properties can be defined in the Xml configuration by explicitly
directing StructureMap to use setter properties while building a concrete type. 
In the Xml, Setter configuration is done with the exact syntax as constructor
arguments.  For example, the "Name" property of the OptionalSetterTarget class
shown in previous sections can be set just like a constructor argument in an Xml
node:


{% highlight xml %}
<StructureMap MementoStyle="Attribute">  
  <DefaultInstance
    PluginType="StructureMap.Testing.Pipeline.OptionalSetterTarget, StructureMap.Testing"
    PluggedType="StructureMap.Testing.Pipeline.OptionalSetterTarget, StructureMap.Testing"
    Name="Jeremy" />
</StructureMap>
{% endhighlight %}


Setter properties can also be designated as mandatory setters with the
`<Setter>` node in the Xml configuration.  From the unit tests, I have a class
called OtherGridColumn that exposes several properties:


{% highlight csharp %}
public class OtherGridColumn : IGridColumn
{
    public IWidget Widget { get; set; }
  
    public string ReadOnly
    {
        get { return "whatever"; }
    }
  
    public FontStyleEnum FontStyle { get; set; }
    public string ColumnName { get; set; }
    public Rule[] Rules { get; set; }
    public bool WrapLines { get; set; }
    public bool Displayed { get; set; }
    public int Size { get; set; }
}
{% endhighlight %}


I can direct StructureMap to make the properties on the OtherGridColumn class
mandatory in a `<Plugin>` node for OtherGridColumn.  This probably isn't
necessary with the new optional setter injection capabilities, but it is still
valid and the equivalent of using the `[SetterProperty]` attribute.


{% highlight xml %}
<PluginFamily/span> Type="StructureMap.Testing.Widget5.IGridColumn" Assembly="StructureMap.Testing.Widget5" DefaultKey="">
  <Plugin Assembly="StructureMap.Testing.Widget5" Type="StructureMap.Testing.Widget5.OtherGridColumn" ConcreteKey="Other">
    <Setter Name="ColumnName" />
    <Setter Name="FontStyle" />
    <Setter Name="Rules" />
    <Setter Name="Widget" />
    <Setter Name="WrapLines" />
  </Plugin>
</PluginFamily>
{% endhighlight %}


 

