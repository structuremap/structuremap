---
title: Auto Wiring
layout: default
---

The best way to use an IoC container is to allow "Auto Wiring" to do
most of the             work for you.  IoC Containers like StructureMap are an
infrastructure             concern, and as such, should be isolated from as much
of your code as possible.              Before examining Auto Wiring in depth,
let's look at a common anti pattern of             IoC usage:

IoC Container Anti-Pattern
=================================


One of the worst, but sadly  most common, usages of an IoC container
is             shown below:


{% highlight csharp %}
// This is the way to write a Constructor Function with an IoC 
// Let the IoC container "inject" services from outside, and 
// ShippingScreenPresenter ignorant of the IoC 
public ShippingScreenPresenter(IShippingService service, IRepository repository)
{
    _service = service;
    _repository = repository;
}

// FAIL!
// This is the wrong way to use an IoC container.  Do NOT invoke the container 
// the constructor function.  This tightly couples the ShippingScreenPresenter 
// the IoC container in a harmful way.  This class cannot be used in 
// production or testing without a valid IoC configuration.  Plus, you're writing 
// more code

public ShippingScreenPresenter()
{
    _service = ObjectFactory.GetInstance<IShippingService>();
    _repository = ObjectFactory.GetInstance<IRepository>();
}
{% endhighlight %}

Example
=================================


Typically, you’ll try to minimize the number of             Service
Locator             (`Container.Get*****`) usages in             your system to a
bare minimum (I found 8 in my current system, but I think I’ll             find
a way to prune half of those later).  Most of the value of an IoC tool          
is in automatically doing Dependency Injection.  I’m working with the new       
MVC framework at the moment, so it’s a handy sample.  Let’s say that we         
have a Controller class for a typical CRUD screen.  That Controller class       
will generally need to interact with both validation services and the data      
access functionality of the Repository.  Here’s a representative Controller     
class:


{% highlight csharp %}
public class SomeScreenController : IController
{
    private readonly IRepository _repository;
    private readonly IValidator _validator;

    // SomeScreenController depends on both IRepository and 
    public SomeScreenController(IRepository repository, IValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }
}
{% endhighlight %}


So let’s get StructureMap set up for this SomeScreenController class:

{% highlight csharp %}
ObjectFactory.Initialize(x =>
{
    // Set up the IValidator
    x.ForRequestedType<IValidator>().TheDefaultIsConcreteType<Validator>();

    // Set up the IRepository
    x.ForRequestedType<IRepository>().Use<Repository>()
        .WithCtorArg("connectionString").EqualToAppSetting("CONNECTION-STRING");
});
{% endhighlight %}

You’ll notice that we didn’t make any explicit configuration for the
SomeScreenController class, but yet we could now call:

{% highlight csharp %}
var controller = ObjectFactory.GetInstance<SomeScreenController>();
{% endhighlight %}

and StructureMap will happily create a new instance of the
SomeScreenController             class by invoking its constructor and passing
in a new Validator object and a             new Repository object created with
the connection string from the App.config             file.  We didn’t need to
tell StructureMap how to construct             SomeScreenController because:

* StructureMap can look at the constructor function of SomeScreenController and 
see that it depends on IValidator and IRepository
* StructureMap "knows" about the default way to create and return an IValidator 
and an IRepository

This feature is known as “auto wiring,” and all the mainstream IoC
containers             support this feature to some extent or another. 


StructureMap's Policies for Auto Wiring
=================================


By default, as long as an object is being created by invoking its constructor   
function, StructureMap will try to create/resolve/find an object for each       
non-primitive dependency in the requested concrete type.  If StructureMap       
doesn't "know" how to find a requested dependency, it will throw an exception.  
By design, StructureMap cannot auto wire primitive arguments like strings and   
numbers.  The Auto Wiring can be overriden by explicit configuration (this      
might actually be easier with Xml configuration):


{% highlight csharp %}
registry.InstanceOf<Rule>()
    .Is.OfConcreteType<WidgetRule>()
    .WithName("TheWidgetRule")
    .CtorDependency<IWidget>().Is(i => 
        i.TheInstanceNamed("Yellow"));
{% endhighlight %}


In the example above, the IWidget dependency of the WidgetRule class is    
overriden.

**Object Identity within a Single Request**

Within a single object request, StructureMap will only create a single object
for             a single Instance configuration.  What that means in effect is
that if two             or more objects in a single request need the same
dependency, those two objects             will get the exact same instance of
that dependency.  Let's immediately             jump into code to demonstrate
this.


This auto wiring policy was intended for objects that need to be shared by lots 
of other objects.  A common example of this is some sort of DataContext         
class:


{% highlight csharp %}
public class DataContext
{
    private Guid _id = Guid.NewGuid();
    public override string ToString()
    {
        return string.Format("Id: {0}", _id);
    }
}
{% endhighlight %}


Now, let's say that I have a hierarchy of classes that all need to work on a    
DataContext:


{% highlight csharp %}
public class Class1
{             
    public Class1(DataContext context){}               
    public override string ToString() 
    {                
        return string.Format("Class1 has Context: {0}", _context); 
    } 
}  

public class Class2
{             
    public Class2(Class1 class1, DataContext context) {}   
            
    public override string ToString()
    {                
        return string.Format("Class2 has Context: {0}\n{1}", _context, _class1); 
    } 
}  

public class Class3
{             
    public Class3(Class2 class2, DataContext context) {}   
            
    public override string ToString() 
    {                
        return string.Format("Class3 has Context: {0}\n{1}", _context, _class2); 
    } 
}
{% endhighlight %}


When you request an object of Class3 with a call to    
`Container.GetInstance<Class3>()` like this:


{% highlight csharp %}
[Test] 
public void demonstrate_session_identity() 
{             
    var class3 = container.GetInstance<Class3>();             
    Debug.WriteLine(class3); 
}
{% endhighlight %}


The output is:

```
Class3 has Context: Id: 3abe0330-e94f-48a3-b8c3-56d278eea07f
Class2 has Context: Id: 3abe0330-e94f-48a3-b8c3-56d278eea07f
Class1 has Context: Id: 3abe0330-e94f-48a3-b8c3-56d278eea07f
```

In the sample above, when we write out the Class3, Class2, and Class1 objects to
Debug.WriteLine, we find that each of these objects have a reference to the same
DataContext.  If we were to run this test again, the output might be:

```
Class3 has Context: Id: 109329ce-4058-4a35-9fd1-46d47c1e69e7
Class2 has Context: Id: 109329ce-4058-4a35-9fd1-46d47c1e69e7
Class1 has Context: Id: 109329ce-4058-4a35-9fd1-46d47c1e69e7
```

We see the exact same behavior, but it was a different object instance of
DataContext for the new object graph.


This behavior also applies to objects passed in to the Container as an explicit
argument:


{% highlight csharp %}
[Test] 
public void demonstrate_session_identity_with_explicit_argument() 
{             
    DataContext context = new DataContext();             
    Debug.WriteLine("The context being passed in is " + context);               
    var class3 = container.With(context).GetInstance<Class3>();             
    Debug.WriteLine(class3); 
}
{% endhighlight %}


The output of this unit test is:

```
The context being passed in is Id: 87ddccfd-a441-41fd-a86d-3f32987496ba
Class3 has Context: Id: 3abe0330-e94f-48a3-b8c3-56d278eea07f
Class2 has Context: Id: 3abe0330-e94f-48a3-b8c3-56d278eea07f
Class1 has Context: Id: 3abe0330-e94f-48a3-b8c3-56d278eea07f
```

The point of the sample above is just to show that the object instance of
DataContext passed into the Container is used to create the Class3, Class2, and
Class1 objects.

**Injecting Arrays of Services**

StructureMap has always supported Dependency Injection of arrays of dependency  
objects.  New in StructureMap 2.5+ is a policy that if any array of            
dependencies is not explicitly specified, StructureMap will inject all possible 
instances of that dependency type.  The sample below illustrates this auto      
wiring policy.  I have a class called "ClassThatUsesValidators" that needs      
an array of IValidator objects.  Below, I've configured two different           
Instances of ClassThatUsesValidator, one that explicitly configures its children
IValidator and another Instance that is just going to let auto wiring inject the
IValidator's.


{% highlight csharp %}
public interface IValidator
{ 
}  

public class Validator : IValidator
{ 
    private readonly string _name;  

    public Validator(string name) 
    { 
        _name = name; 
    }  

    public override string ToString() 
    {             
        return string.Format("Name: {0}", _name); 
    } 
}  

public class ClassThatUsesValidators
{ 
    private readonly IValidator[] _validators;  

    public ClassThatUsesValidators(IValidator[] validators) 
    { 
        _validators = validators; 
    }  

    public void Write() 
    {             
        foreach (IValidator validator in _validators) 
        {                
            Debug.WriteLine(validator); 
        } 
    } 
}  

[TestFixture] 
public class ValidatorExamples 
{ 
    private Container container;  

    [SetUp] 
    public void SetUp() 
    { 
        container = new Container(x => 
        {          
            x.ForRequestedType<IValidator>().AddInstances(o => 
            {                     
                o.OfConcreteType<Validator>().WithCtorArg("name").EqualTo("Red").WithName("Red");                
                o.OfConcreteType<Validator>().WithCtorArg("name").EqualTo("Blue").WithName("Blue");
                o.OfConcreteType<Validator>().WithCtorArg("name").EqualTo("Purple").WithName("Purple");
                o.OfConcreteType<Validator>().WithCtorArg("name").EqualTo("Green").WithName("Green");
            });   
                    
            x.ForRequestedType<ClassThatUsesValidators>().AddInstances(o => 
            {                    
                // Define an Instance of ClassThatUsesValidators that depends on AutoWiring
                o.OfConcreteType<ClassThatUsesValidators>().WithName("WithAutoWiring");   
                       
                // Define an Instance of ClassThatUsesValidators that overrides AutoWiring
                o.OfConcreteType<ClassThatUsesValidators>().WithName("ExplicitArray")
                    .TheArrayOf<IValidator>().Contains(y =>
                        {
                            y.TheInstanceNamed("Red");                             
                            y.TheInstanceNamed("Green");                         
                        });
            }); 
        }); 
    }

    [Test] 
    public void what_are_the_validators() 
    {             
        Debug.WriteLine("With Auto Wiring"); 
            container.GetInstance<ClassThatUsesValidators>("WithAutoWiring").Write();             
        Debug.WriteLine("=================================");             
        Debug.WriteLine("With Explicit Configuration"); 
            container.GetInstance<ClassThatUsesValidators>("ExplicitArray").Write(); 
    } 
}
{% endhighlight %}


The output of what_are_the_validators() is:

```
With Auto Wiring
Name: Red
Name: Blue
Name: Purple
Name: Green
 =================================
With Explicit Configuration
Name: Red
Name: Green
```
