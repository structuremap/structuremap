---
title: Auto Mocking Container with StructureMap
layout: default
---

StructureMap includes an                        Auto Mocking Container (a couple
actually).  If you're not familiar             with the concept, an Auto Mocking
Container repurposes an IoC container to             automate the creation and
attachment of mock objects to a concrete class within             unit tests. 
The key fact about any auto mocking container is that it             reduces the
mechanical cost of creating                        interaction tests by
obviating the need to create mock objects by hand,             track those mock
objects in the test, and apply the mocks to a concrete class.              Out
of the box, StructureMap includes the RhinoAutoMocker for            RhinoMocks
and             MoqAutoMocker for Moq.  The             Auto Mocking support is
in the StructureMap.AutoMocking.dll.


 


How it Works
=================================


Let's start with an example from the unit tests.  Let's say that you have a     
class named "ConcreteClass" that uses three other types of services to perform  
its responsibilities.


{% highlight csharp %}
public ConcreteClass(IMockedService service, 
    IMockedService2 service2, IMockedService3 service3)
{
    _service = service;
    _service2 = service2;
    _service3 = service3;
}
{% endhighlight %}


Traditionally, I might write integration tests against ConcreteClass by creating
a different mock object for each service dependency, attach each in the    
constructor function of ConcreteClass, and probably track each mock object in a 
field of the unit test fixture.  With the auto mocking container, it's    
simpler:


{% highlight csharp %}
// This sets up a Rhino Auto Mocker in the Arrange, Act, Assert mode
var autoMocker = new RhinoAutoMocker<ConcreteClass>(MockMode.AAA);
{% endhighlight %}


That's the entire mock object setup.  To get an instance of the    
ConcreteClass inside the unit test you access the ClassUnderTest property of the
auto mocker:


{% highlight csharp %}
// Act in the test
ConcreteClass @class = autoMocker.ClassUnderTest;
@class.CallService();
{% endhighlight %}


At the first call to the RhinoAutoMocker.ClassUnderTest, the RhinoAutoMocker:


ClassUnderTest is created upon demand.  Likewise, you can access the
actual             mock objects either to set expectations before ClassUnderTest
is created, or to             assert method calls after the action.  Use the
Get`<T>`() method to access             the mock objects per type that will be
used in the test:


{% highlight csharp %}
// This retrieves the mock object for IMockedService
autoMocker.Get<IMockedService>().AssertWasCalled(s => s.Go());
{% endhighlight %}


How it uses Mock Engines
=================================


If you wanted to use the AutoMocker with another type of Mock Object Library,
you             can simply use the `AutoMocker<CLASSUNDERTEST>` class, but feed
it an             implementation of this interface that works with the mock
object library of your             choice:


{% highlight csharp %}
public interface ServiceLocator
{
    T Service<T>() 
        where T : class;

    object Service(Type serviceType);

    T PartialMock<T>(params object[] args) 
        where T : class;
}
{% endhighlight %}


The RhinoMocks "AAA" mode is shown below:


{% highlight csharp %}
public class RhinoMocksAAAServiceLocator : ServiceLocator
{
    private readonly RhinoMockRepositoryProxy _mocks = new RhinoMockRepositoryProxy();

    public T Service<T>() 
        where T : class
    {
        var instance = (T)_mocks.DynamicMock(typeof (T));
        _mocks.Replay(instance);

        return instance;
    }

    public object Service(Type serviceType)
    {
        var instance = _mocks.DynamicMock(serviceType);
        _mocks.Replay(instance);
        return instance;
    }

    public T PartialMock<T>(params object[] args) 
        where T : class
    {
        var instance = (T)_mocks.PartialMock(typeof(T), args);
        _mocks.Replay(instance);

        return instance;
    }
}
{% endhighlight %}


And this is the Moq version:


{% highlight csharp %}
public class MoqServiceLocator : ServiceLocator
{
    private readonly MoqFactory _moqs = new MoqFactory();

    public T Service<T>() where T : class
    {
        return (T)_moqs.CreateMock(typeof(T));
    }
                       
    public object Service(Type serviceType)
    {
        return _moqs.CreateMock(serviceType);
    }

    public T PartialMock<T>(params object[] args) 
        where T : class
    {
        return (T)_moqs.CreateMockThatCallsBase(typeof (T), args);
    }
}
{% endhighlight %}


Supplying Mocks or Stubs to the AutoMocker
=================================


Frequently you may want to use the AutoMocker, but override the normal mock     
object creation with your own stub or hand rolled mock objects.  That's         
easy enough with the Inject() methods:


{% highlight csharp %}
/// <summary>
/// Method to specify the exact object that will be used for 
/// "pluginType."  Useful for stub objects and/or static mocks
/// </summary>
/// <param name="pluginType"></param>
/// <param name="stub"></param>
void Inject(Type pluginType, object stub);
                         
/// <summary>
/// Method to specify the exact object that will be used for 
/// "pluginType."  Useful for stub objects and/or static mocks
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="target"></param>
void Inject<T>(T target);
{% endhighlight %}


Partial Mock the ClassUnderTest
=================================


Every so often, I find it useful to create a "partial mock"             for the
ClassUnderTest.  That can be done with the AutoMocker by calling             the
PartialMockTheClassUnderTest() method:


{% highlight csharp %}
var autoMocker = createAutoMocker<ConcreteClass>();

var service = autoMocker.Get<IMockedService>();
var service2 = autoMocker.Get<IMockedService2>();
var service3 = autoMocker.Get<IMockedService3>();

autoMocker.PartialMockTheClassUnderTest();
{% endhighlight %}


Expectations and assertions can be made directly on the
AutoMocker.ClassUnderTest     property.  When the PartialMockTheClassUnderTest()
method is called, the     underlying value behind ClassUnderTest will be a
partial mock, but the     AutoMocker will still use mock objects for the
dependencies of the class under     test.

