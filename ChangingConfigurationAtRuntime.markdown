---
title: Changing Configuration at Runtime
layout: default
---

In the beginning (late 2003), there was attributes and there was a copious
amount of Xml configuration, and I called it good.          Look Ma!  I can
radically change the behavior of the code without         recompiling, isn't
that a marvelous thing?          Then we started to use StructureMap on a real
project and quickly realized that         it would be very useful if we could
override some services with mock objects in         our unit tests.  In later
projects I've run into scenarios where it would         be valuable to put an
object into StructureMap after it was created.  Other         users have asked
for the ability to load assemblies or modules of their system         on demand
so as to save memory.  A major goal of the StructureMap 2.5         release has
been to greatly extend its capabilities for service registration at        
runtime.  With a very few exceptions, you can now make any and all        
configuration changes after the first call to ObjectFactory.  My        
recommendation is to use this behavior simply and with caution because it will  
bypass many of the diagnostic abilities built into StructureMap (i.e.        
StructureMapDoctor might miss configuration problems introduced outside of the  
normal configuration).

Adding Configuration to an Existing Container
=================================


In contrast to Initialize(), the Configure() method allows you to add
additional         configuration to an existing Container or ObjectFactory. 
Think of this         scenario.  You're building a composite application that
contains multiple         modules spread over several assemblies, but you might
not want to load any of         the configuration or types for a particular
module until it's requested by the         user.  In that case, you can use the
Configure() method like this:


{% highlight csharp %}
// This code would add any configuration            
// Registry classes found in the            
// assembly named 'MyApplication.Module1'            
ObjectFactory.Configure(x => 
{                 
    x.Scan(scan =>                 
    {                     
        scan.LookForRegistries();                     
        scan.Assembly("MyApplication.Module1");                 
    }); 
});
{% endhighlight %}


To summarize, `Initialize()` completely resets the configuration of a
Container,         and `Configure()` is purely additive.  If `Configure()` should
happen to be         called before `Initialize()`, it will set up the Container
with the configuration         in the `Configure()` call.  `Configure()` offers a
subset of the `Initialize()`         method (it leaves out the directives for the
StructureMap.config file), and it         also exposes the entire Registry DSL. 
You can take advantage of that fact         to add a few types or instances at a
time:


{% highlight csharp %}              
ObjectFactory.Configure(x => 
{                 
    x.ForRequestedType<ISomething>()
        .TheDefaultIsConcreteType<SomethingOne>(); 
});
{% endhighlight %}


Injecting a Single Service at Runtime
=================================


In my desktop applications the main form usually implements some sort of        
IApplicationShell interface.  I've found it valuable to place the main form     
itself into StructureMap, as well as several child controls of the main form as 
well so that various Controllers, Presenters, and Commands can interact with    
parts of the main shell without tight coupling.  I probably could build the
ApplicationShell             itself inside of StructureMap, but the child
controls like (I'm making this up)             IQueryToolBar or IExplorerPane
are easiest to create as part of the             ApplicationShell and loaded
into StructureMap later.


{% highlight csharp %}   
public class ApplicationShell : Form, IApplicationShell 
{ 
    public IQueryToolBar QueryToolBar...
    public IExplorerPane ExplorerPane...
}
{% endhighlight %}


Easy enough.  The main shell has some controls on it.  Now, we may want     a
centralized class to govern the behavior of just the query tool bar along the   
top of the main form.  That class obviously needs to find the IQueryToolBar    
on the main form, but I need a clean way of connecting the IQueryToolBar to this
new QueryController class.


{% highlight csharp %} 
public class QueryController
{ 
    private IQueryToolBar _toolBar;  

    public QueryController(IQueryToolBar toolBar) 
    { 
        _toolBar = toolBar; 
    } 
}
{% endhighlight %}


StructureMap is going to build up the QueryController, but it doesn't help to   
inject in a new IQueryToolBar that isn't visible anywhere in the application.   
We need to get to exactly the right instance of that control.  So let's use    
the new `Inject<T>(T instance)` method on ObjectFactory to register the child   
controls from the main form.


{% highlight csharp %}            
// Familiar stuff for the average WinForms or WPF developer
// Create the main form            
ApplicationShell shell = new 
ApplicationShell();   
           
// Put the main form, and some of its children into StructureMap
// where other Controllers and Commands can get to them
// without being coupled to the main form            
ObjectFactory.Inject<IApplicationShell>(shell);            
ObjectFactory.Inject<IQueryToolBar>(shell.QueryToolBar);            
ObjectFactory.Inject<IExplorerPane>(shell.ExplorerPane);     
           
Application.Run(shell);
{% endhighlight %}


Now, a call to `ObjectFactory.GetInstance<QueryController>()` will poke the    
IQueryToolBar instance we registered above into the constructor of the new    
QueryController object.  Remember, one of the primary usages of     StructureMap
is simply to get the right service dependencies and metadata to the     right
concrete classes so you can concentrate on doing QueryController stuff in    
the QueryController class instead of bootstrapping all the stuff it needs.


I've used this exact strategy on three applications now to great success.     
The previous design had a lot of "thisThing.ThatThing.QueryToolBar"     (who
needs the Law of Demeter?) properties     just to get access to the child
controls on the main form.  It was     devolving into spaghetti code before I
adopted the strategy above.


The `ObjectFactory.Inject<T>(T instance)` method is identical to the older    
`ObjectFactory.InjectStub<T>(T stub)` method.  I've marked the older method    
as deprecated because I think the name is misleading. 


Use this strategy for any type of service or component that you need to inject  
into other services, but isn't convenient or possible to build through    
StructureMap itself.


Injecting a Mock or a Stub at Runtime
=================================


On its maiden cruise in 2004, my team quickly realized that we needed a way toOn
its maiden cruise in 2004, my team quickly realized that we needed a way to     
make StructureMap deliver up mock objects in unit tests.  It obviously        
isn't efficient to mock with an Xml file for each and every unit test that      
requires this function (don't laugh, I've seen people do that with early        
versions of a StructureMap container), so we wanted a way to temporarily load
ObjectFactory up         with a mock object in place of its normal behavior for
a given type.          Originally, there was hard coded support for the NMock
framework, but with the         advent of Rhino Mocks and other new mocking
frameworks, I've removed that         functionality in favor of the simple
`ObjectFactory/Container.Inject<T>(T object)`         methods.


My strong advice is to not use the Container or ObjectFactory in unit tests in  
the mass majority of cases.  Rather, my advice is to use simple           
Dependency Injection to inject mock objects during unit tests.  However, there  
are still times when you want or need a class to use the Container or           
ObjectFactory itself to get dependencies at runtime.  For that case, here       
is a sample:


{% highlight csharp %} 
[TestFixture] 
public class MockingExample 
{ 
    [SetUp] 
    public void SetUp() 
    {             
          // Make sure that the container is bootstrapped
          Bootstrapper.Restart(); 
    }  

    [TearDown] 
    public void TearDown() 
    {             
          // The problem with injecting mocks is in keeping the             
          // mocks from one test getting into another test.              
          // If you build the Container individually for each test run,             
          // this isn't a problem.  However, if you do inject mocks into             
          // the ObjectFactory static container, use the ResetDefaults()             
          // method in the [TearDown] (or Dispose() for xUnit.net) to clear             
          // out runtime injected services between             
          ObjectFactory.ResetDefaults(); 
    }  

    [Test] 
    public void unit_test_that_uses_a_mock() 
    {             
          // Create a mock object with Rhino Mocks
          var serviceMock = MockRepository.GenerateMock<IService>();               
          ObjectFactory.Inject(serviceMock);               

          // or      

          ObjectFactory.Inject("theService", serviceMock);   
            
          // WARNING!  Inject is a generic method

          // This method registers serviceMock as an "IService"             
          ObjectFactory.Inject(serviceMock);   
            
          // and is NOT equivalent to:             
          ObjectFactory.Inject<IBasicService>(serviceMock); 
    } 
}
{% endhighlight %}


Please note that the call to `Inject<T>(T object)` registers the mock object as
the     default for the PluginType "T."  With RhinoMocks (my mocking tool of    
choice) this hasn't been as issue because you create mock objects as the    
interface in question.  I have seen confusion with using other mocking    
engines that return the mock objects as "object" (I'm looking at you TypeMock). 
In that case, be careful to include the generic parameter in the call to    
`Inject<T>()` to avoid registering your new mock object as "object." 


Ejecting all Instances of a PluginType
=================================


From a user request, StructureMap 2.5.2 introduces the ability to remove all    
Instances of a given PluginType by calling the
`Container.EjectAllInstancesOf<T>()`  method, where "T" is the
PluginType.  This functionality will remove all             object instances of
T that are cached with the Singleton scope, but has no             effect on
other scopes.

