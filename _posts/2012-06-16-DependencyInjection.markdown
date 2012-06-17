---
title: Dependency Injection - What is it, and what is it good for?
layout: default
---

This was originally posted on [Jeremy's
blog](http://codebetter.com/blogs/jeremy.miller/archive/2005/10/06/132825.aspx)
way back in 2005.  A few things have changed, specifically the possibility of
using mocking tools that can mock concrete classes and mock without using
Dependency Injection (DI) -- but Jeremy still thinks it's much cleaner to use DI
;-)


  


Dependency Injection
---------------------------------


  


Dependency Injection is a specific usage of the larger [Inversion of
Control](`InversionOfControl`.htm) concept.  In a nutshell, dependency injection
just means that a given class or system is no longer responsible for
instantiating their own dependencies.  In this case “Inversion of Control”
refers to moving the responsibility for locating and attaching dependency
objects to another class or a DI tool.  That might not sound that terribly
profound, but it opens the door for a lot of interesting scenarios.


  


While there’s a fair amount of unnecessary buzz and hype about the concept,
I’ve found Dependency Injection to be very advantageous for doing Test Driven
Development without pulling your hair out.  If you’ve seen articles or blog
posts about Dependency Injection but don’t quite internalize the value of DI
yet, here are the facts as I see them:


  

1. Dependency Injection is an important pattern for creating classes that are
easier to unit test in isolation
1. Promotes loose coupling between classes and subsystems 
1. Adds potential flexibility to a codebase for future changes
1. Can enable better code reuse
1. **The implementation is simple and does *not* require a fancy DI tool**

  


The [ `PicoContainer`](http://picocontainer.org/) team even has silly tee
shirts printed up that say “I expected a paradigm shift, but all I got was a
lousy constructor function.”  DI is certainly a case where a minimum of effort
supplies quite a bit of benefit.  Don’t blow it off just because it seems
trivial. 


  


There are tools out there that do Dependency Injection in .Net.  I use my own
tool called [StructureMap](http://structuremap.sourceforge.net/) in my
development, but I’m going to focus on only the concept of DI in this post.


  


Example Problem
---------------------------------

My first experience with conscious usage of DI was a WinForms client
communicating with the backend via web services that was built with the [Model
View Presenter](http://codebetter.com/blogs/jeremy.miller/articles/129546.aspx)
(“Humble Dialog Box”) architecture for easier unit testing.  Most screens in the
client end up with something like this set of classes:

* Model – Whatever business object/DataSet/chunk of data is being displayed or
edited
* View – A WinForms UserControl class.  Displays data to a user and captures user
input and screen events (duh).
* Service – A web service proxy class to send requests to the backend
* Presenter – The controller class that coordinates all of the above.

  


The presenter class without Dependency Injection might look like this.


{% highlight csharp %}
public class Presenter
{
    private View _view;
    private Model _model;

    public Presenter(){}

    public object CreateView(Model model)
    {
        _model = model;
        _view = new View();
        _view.DisplayModel(model);
        return _view;
    }

    public void Close()
    {
        bool canClose = true;

        if (_view.IsDirty())
        {
            canClose = _view.CanCloseDirtyScreen();   
        }
 
        if (canClose)
        {
            _view.Close();
        }
    }

    public void Save()
    {
        Service service = new Service();
        service.Persist(_model);
    }
}
{% endhighlight %}

This code cannot be unit tested in isolation because it has a tight coupling to
a concrete implementation of both the WinForms `UserControl` (View) and the
proxy class to a web service (Service).  This code as is cannot function without
both the User Interface and a web server running the backend.   The point of
using the MVP is to isolate most of the user interface logic away from the
WinForms and web service mechanics to enable effective unit testing, so we’re
missing something here.  To unit test the presenter logic we’d like to replace
the user interface and web service dependencies with a
[Mock](http://martinfowler.com/articles/mocksArentStubs.html) object inside
our test fixture classes.  In order to mock the view and service, we first need
to use the [Dependency Inversion Principle](http://codebetter.com/blogs/jeremy.miller/articles/129543.aspx)
to make the Presenter class depend on an abstracted `IView` and `IService`
interface instead of the concrete `UserControl` and `WebProxy` classes.  The
next thing to do is to alter the Presenter class so that we can substitute at
run time the mock objects instead of the concrete classes within the unit
tests.  This is where Dependency Injection comes into play.

There are a couple of different flavors of Dependency Injection (via [Martin
Fowler](http://martinfowler.com/articles/injection.html) + the Pico guys)

1. Constructor Injection – Attach the dependencies through a constructor function
at object creation
1. Setter Injection – Attach the dependencies through setter properties
1. Interface Injection – This is an odd duck.  I’ve never used it or seen this
used.  I suspect its usage is driven by specific DI tools in the Java world.
1. Service Locator – Use a well known class that knows how to retrieve and create
dependencies.  Not technically DI, but this is what most DI/IoC container tools
really do.

Constructor Injection
---------------------------------

My preference is to use the “Constructor Injection” flavor of DI.  The
mechanism here is pretty simple; just push the dependencies in through the
constructor function. 

{% highlight csharp %}
public class Presenter
{
    private IView _view;
    private Model _model;
    private IService _service;
 
    public Presenter(IView view, IService service)
    {
        _view = view;
        _service = service;
    }
 
    public object CreateView(Model model){…}
    public void Close(){…}
    public void Save(){…}
}
 
[TestFixture]
public class PresenterTestFixture
{
    private IMock _serviceMock;
    private IMock _viewMock;
    private Presenter _presenter;
 
    [SetUp]
    public void SetUp()
    {
        // Create the dynamic mock classes for IService and IView
        _serviceMock = new DynamicMock(typeof(IService));
        _viewMock = new DynamicMock(typeof(IView));
 
        // Create an instance of the Presenter class using the mock objects
        _presenter = new Presenter(
            (IView) _viewMock.MockInstance, 
            (IService) _serviceMock.MockInstance);
    }     
}
{% endhighlight %}

One of the benefits of using Constructor Injection is that the constructor
function now explicitly declares the dependencies of a class.  I also think
Constructor Injection makes it easier for other developers to use your class
because it expresses a contract.  Give the class what it needs in its
constructor function and it should be ready to function.  It’s usually a best
practice to create a valid object in as few steps as possible for ease of use. 
Using Constructor Injection also allows you to maintain more encapsulation by
eliminating the need to expose getter and setter properties for dependencies
like the `IService` interface that are immutable.

Exposing the dependencies in a constructor function is arguably a violation of
encapsulation because now a client of Presenter would have to create instances
of `IView` and `IService` first before calling the constructor function.  To get
around this issue I usually suggest a compromise.  Create a second no argument
constructor that builds the default instances for clients of Presenter to use. 
The “full” constructor is usually commented as a testing constructor.  Some
people think this pattern is evil redundancy, but it gets the job done.

{% highlight csharp %}
// Testing constructor
public Presenter(IView view, IService service)
{
    _view = view;
    _service = service;
}

// Default constructor
public Presenter() : this(new View(), new Service()){}
{% endhighlight %}


Setter Injection
---------------------------------

Setter injection is just creating a setter property to replace a dependency on
a previously instantiated object.  I don’t like Setter Injection because it
requires extra, hidden steps to prepare an object to execute.  I’ve been burned
a couple times in the last year when I’ve inherited some code that depended on
setters to set up dependencies.  That being said, Setter Injection does work and
is often necessary when you’re dealing with existing code.  Michael Feathers
recommends using Setter Injection as a dependency breaking technique for legacy
code when a dependency is too difficult to expose through a constructor.

Here’s the Presenter class using Setter Injection. 

{% highlight csharp %}
public class Presenter
{
    private IView _view;
    private Model _model;
    private IService _service;

    public Presenter()
    {
        _view = new View();
        _service = new Service();
    }

    public IView View
    {
        get { return _view; }
        set { _view = value; }
    }

    public IService Service
    {
        get { return _service; }
        set { _service = value; }
    }

    public object CreateView(Model model){…}
    public void Close(){…}
    public void Save(){…}
}

[TestFixture]
public class PresenterTestFixture
{
    private IMock _serviceMock;
    private IMock _viewMock;
    private Presenter _presenter;

    [SetUp]
    public void SetUp()
    {
        // Create the dynamic mock classes for IService and IView
        _serviceMock = new DynamicMock(typeof(IService));
        _viewMock = new DynamicMock(typeof(IView));

        // Create an instance of the Presenter class
        _presenter = new Presenter();

        // Attach the Mock objects
        _presenter.View = (IView) _viewMock.MockInstance;
       _presenter.Service = (IService)
       _serviceMock.MockInstance;
    }
}
{% endhighlight %}
  


Here’s a variation on Setter Injection I’ve seen other teams use.  In the
getter of the property, just create the default instance of the dependency if it
hasn’t been created.  I don’t like this approach because it hides the
dependencies of a class.  I think this is creating “Mystery Meat” dependencies
and brittle code.  In practice I thought that this made it difficult to retrofit
unit tests into existing code.  Of course, retrofitting unit tests onto existing
code is always hard so maybe that’s really not a drawback.

{% highlight csharp %}
// Always access the _view field through the Property
public IView View
{
    get
    {
        if (_view == null)
        {
            _view = new View();
        }

        return _view;
    }
    set { _view = value; }
}
{% endhighlight %}

Service Locator
---------------------------------

An alternative to using Dependency Injection is to use a Service Locator to
fetch the dependency objects.  Using a Service Locator creates a level of
indirection between a class and its dependencies.  Here’s a version of the
Presenter class that gets its `IView` and `IService` dependencies by asking
StructureMap’s `ObjectFactory` class for the default type and configuration of
`IView` and `IService`.  While it is generally possible to use the Service
Locator to return mock or stub objects inside test fixtures, I would still
prefer to leave a testing constructor so the unit tests can be simpler.  I find
that using a Service Locator within a unit test can be confusing because it’s
not clear where the mock object is used.

{% highlight csharp %}
public class Presenter
{
    private IView _view;
    private Model _model;
    private IService _service;

    public Presenter()
    {
        // Call to StructureMap to fetch the default configurations of `IView` and `IService`
        _view = (IView) StructureMap.ObjectFactory.GetInstance(typeof(IView));
        _service = (IService) StructureMap.ObjectFactory.GetInstance(typeof(IService));
    }
    public object CreateView(Model model){…}
    public void Close(){…}
    public void Save(){…}
}
{% endhighlight %}

I’ve seen several teams go their own way to create custom Service Locator’s,
usually with a Singleton like this.

{% highlight csharp %}
public class ServiceFactory
{
    private static IService _instance = new Service();

    private ServiceFactory(){}

    public static IService GetInstance()
    {
        return _instance;
    }

    // Used to register mock or stub instances in place of 
    // the concrete Service class
    public static void RegisterInstance(IService instance)
    {
        _instance = instance;
    }
}
{% endhighlight %}

I detest this pattern and I’ve been eliminating this from my team’s primary
product.  There are just too many opportunities to screw up your tests by not
having isolated unit tests.  It’s also unnecessary because there are existing
tools specifically for this.

Good for More than Unit Testing
---------------------------------

I’ve focused almost entirely on the value of Dependency Injection for unit
testing and I’ve even bitterly referred to using DI as “Mock Driven Design.” 
That’s not the whole story though.  One of original usages for Dependency
Injection was to provide smoother migration paths away from legacy code.  One
evolutionary approach to replacing legacy code is the
“[Strangler](http://www.martinfowler.com/bliki/`StranglerApplication`.html)”
approach.  Making sure that any new code that depends on undesirable legacy code
uses Dependency Injection leaves an easier migration path to eliminate the
legacy code later with all new code.

{% highlight csharp %}
public interface IDataService{}

// Now
public class ProxyToNastyLegacyDataService : IDataService{}

// Later
public class CleanNewCodeDataService : IDataService{}

public class StranglerApplication
{
    public StranglerApplication(IDataService dataService){}
}
{% endhighlight %}

Another benefit of Dependency Injection is increasing the potential for reuse
later.  Going back to the MVP architecture, what if you need to replace the
heavy WinForms client with an ASP.NET system?  The View is obviously useless,
and I think it’s probably silly to use a Web Service when a local class will do
for the `IService` implementation.  We can potentially reuse the Presenter
class; just inject a different implementation for both `IView` and `IService`.

{% highlight csharp %}
public class ASPNetView : System.Web.UI.UserControl, IView
{
    …
}

public class LocalService : IService
{
    …
}

public class WebClientMasterController
{
    public void CreateView(Page page)
    {
        ASPNetView view = (ASPNetView) page.Controls[0];
        Model model = this.GetModel();

        IService service = new LocalService();

        // Presenter can work with a different concrete implementation of
        // `IView` and `IService`
        Presenter presenter = new Presenter(view, service);
        presenter.CreateView(model);
    }

    public Model GetModel(){return new Model();}
}
{% endhighlight %}

Using a Dependency Injection Tool
---------------------------------

Using Dependency Injection potentially adds some overhead to using the classes
that don’t create their own dependencies.  This is where one of the Dependency
Injection tools can pay large dividends by handling this mechanical work and
creating some indirection between clients and dependencies. 

 **Links**

* The canonical article on Dependency Injection is from [Martin
Fowler](http://martinfowler.com/articles/injection.html).
* I have some other information on the
[StructureMap](http://structuremap.sourceforge.net/) website
* [ http://PicoContainer.org](http://picocontainer.org/) has some good
information
* Griffin Caprio (of Spring.Net) on [ MSDN
Magazine](http://msdn.microsoft.com/msdnmag/issues/05/09/DesignPatterns/default.aspx)
* J.B. Rainsberger in [ Better
Software](http://www.diasparsoftware.com/articles/DependencyInjection.pdf) 
