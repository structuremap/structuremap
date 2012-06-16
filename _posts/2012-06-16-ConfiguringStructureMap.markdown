---
title: Configuring StructureMap
layout: default
---

Forms of Configuration
---------------------------------


The first step in using StructureMap is configuring a Container object or
ObjectFactory.  The configuration options have changed more than anything else
from the initial releases to StructureMap 2.5+.  You have three forms of
configuration to choose from:

* [Registry DSL](RegistryDSL.htm) -- The programmatic [Fluent
Interface](http://www.martinfowler.com/bliki/FluentInterface.html) API for
configuring the Container in code.
* [Xml configuration](XmlConfiguration.htm) (StructureMap.config, the App.config
file, or named files)
* [StructureMap Attributes ](UsingAttributes.htm)-- Not fully deprecated, but not
really recommended either. 

The configuration is highly modular and you can mix and match all of the
configuration choices within the same Container instance.  The strong
recommendation is to use the Registry DSL as much as possible, then use the Xml
configuration strictly for configuration that absolutely must be external to the
code (connection strings, file paths, Url's, etc.).  The attributes are
deprecated and largely unnecessary now, but still supported for backwards
compatibility.


Initializing the Container
---------------------------------


The recommended mechanism for initializing the Container or ObjectFactory is
the Initialize() method.  The Initialize() method is new for StructureMap 2.5,
and largely driven by (my current love affair with Lambda's) the confusion and
misuse of StructureMapConfiguration.  StructureMapConfiguration is still
supported, but it is deprecated in the code and will be eliminated in a future
release.  Initialize() is a Nested Closure that gives you a chance to express
directives telling the Container how to construct itself and add one or more
sources of configuration. 


{% highlight csharp %}
public Container(Action<ConfigurationExpression> action)
{
    var expression = new ConfigurationExpression();
    action(expression);

    construct(expression.BuildGraph());
}
{% endhighlight %}


Typically, you would make a single call to the Initialize() method at
application start up (Global.asax for web application, or the main routine for a
desktop application).  The Container (ObjectFactory is simply a static wrapper
around a single Container object) is completely initialized and configured by
the Initialize() method in one atomic action.  **Any successive calls to
Initialize() will effectively wipe out any existing configuration and
effectively restart the Container**.  Here's a sample usage of Initialize():


{% highlight csharp %}
ObjectFactory.Initialize(x =>

    x.UseDefaultStructureMapConfigFile = true;
  
    x.AddRegistry(new CoreRegistry());
    x.AddRegistry(new SearchRegistry());
    x.AddRegistry(new WebCoreRegistry());
    x.AddRegistry(new WebRegistry());
    x.AddRegistry(new RuleRegistry());
});
{% endhighlight %}


Inside the Initialization() method you can declare directives against an
InitializationExpression object.  The InitializationExpression object has these
methods for all all the possible configuration directives.


{% highlight csharp %}
public interface IInitializationExpression
{
    // Directives on how to treat the StructureMap.config file
    bool UseDefaultStructureMapConfigFile { set; }
    bool IgnoreStructureMapConfig { set; }

    // Xml configuration from the App.Config file
    bool PullConfigurationFromAppConfig { set; }

    // Ancillary sources of Xml configuration
    void AddConfigurationFromXmlFile(string fileName);
    void AddConfigurationFromNode(XmlNode node);

    // Specifying Registry's
    void AddRegistry<T>() where T : Registry, new();
    void AddRegistry(Registry registry);

    // Designate the Default Profile.  This will be applied as soon as the 
    // Container is initialized.
    string DefaultProfileName { get; set; }

    // ... and the Registry DSL as well
}
{% endhighlight %}


#### Using the App.Config File


The [System.Configuration](http://msdn.microsoft.com/en-us/library/system.configuration.aspx)
namespace in the .Net framework provides a lot of functionality for caching and
encrypting configuration.  To take advantage of this functionality StructureMap
offers an option to embed configuration directly into the App.config file
(web.config for web development).  Just add a section for StructureMap like
this:


{% highlight xml %}
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="StructureMap" type="StructureMap.Configuration.StructureMapConfigurationSection,StructureMap"/>
  </configSections>
 
  <StructureMap>
    <!-- Put StructureMap configuration here -->
  </StructureMap>
</configuration>
{% endhighlight %}


 Then you need to explicitly tell ObjectFactory to use this configuration:


{% highlight csharp %}
ObjectFactory.Initialize(x =>
{
    // Tell StructureMap to look for configuration 
    // from the App.config file
    // The default is false
    x.PullConfigurationFromAppConfig = true;
});
{% endhighlight %}


#### The StructureMap.config File


In the beginning, StructureMap configuration began and ended with a single file
named "StructureMap.config" in the application base folder that contained
StructureMap Xml configuration (in short, wherever your App.config file would
go).  Today, the default behavior is that StructureMap will automatically read
in configuration data from the StructureMap.config if it is found when either
ObjectFactory.Initialize() is called, or the first time that a service is
requested from ObjectFactory.  You can technically use only the
StructureMap.config file and completely forgo the the usage of any programmatic
bootstrapping.


You can override the default behavior for the StructureMap.config file.  If you
want to make the StructureMap.config file mandatory, you can do this:

{% highlight csharp %}
ObjectFactory.Initialize(x =>
{
		// We put the properties for an NHibernate ISession
		// in the StructureMap.config file, so this file
		// must be there for our application to
		// function correctly
		x.UseDefaultStructureMapConfigFile = true;
});
{% endhighlight %}

At other times you might want to force ObjectFactory to ignore the
StructureMap.config file even if it does exist.

{% highlight csharp %}
ObjectFactory.Initialize(x =>
{
		x.IgnoreStructureMapConfig = true;
});
{% endhighlight %}

Where and How to Bootstrap StructureMap
---------------------------------


Typically, you will bootstrap StructureMap in either the GlobalApplication
(Global.asax) for web applications,


{% highlight csharp %}
public class GlobalApplication : HttpApplication
{
    protected void Application_Start()
    {
        Bootstrapper.Bootstrap();
    }
}
{% endhighlight %}


or for service applications or desktop clients (WPF or WinForms or Console
apps), you would simply embed the initialization in the "Main" method before
anything else runs.  Here's an example from the generic Windows Service project
on my current project.  The very first line of code initializes the StructureMap
container:


{% highlight csharp %}
private static void Main(string[] args)
{
    ObjectFactory.Initialize(initialConfiguration);
    var program = ObjectFactory.GetInstance<Program>();

    try
    {
        var shouldRunAsConsole = false;
        if (args.Any(arg => arg.Equals("/i", StringComparison.OrdinalIgnoreCase)))
        {
            shouldRunAsConsole = true;
            logToTheConsoleWindow();
        }
  
        program.Run(shouldRunAsConsole);
    }
    catch(Exception e)
    {
        LogManager.GetLogger("WindowsService").Error("Service failure.", e);
    }
    finally
    {
        LogManager.Shutdown();
    }
}
{% endhighlight %}


_Important Note:_  You could easily call ObjectFactory.Initialize() directly
within the Global.asax or Main method and even do all of the necessary
configuration within that call to Initialize(), but that's not necessarily a
good idea.  Instead, I strongly recommend that you centralize all container
configuration into a "Bootstrapper" class that can be exercised independently of
the user interface or application.  That strategy is useful for many reasons:

* You can then reuse the Bootstrapper configuration inside integrations tests
* The Bootstrapper can be used to create configuration diagnostics and environment
tests.  See [Diagnostics](Diagnostics.htm) for more information.
* You can potentially reuse the StructureMap configuration in other environments
and configurations than your current application

#### Creating a Bootstrapper


There is not much to building your own Bootstrapper.  There is an interface
called IBootstrapper that you can implement to standardize the bootstrapping:


{% highlight csharp %}
public interface IBootstrapper
{
    void BootstrapStructureMap();
}
{% endhighlight %}


Implementing IBootstrapper is not mandatory, but it does enable the usage of
the StructureMapDoctor tool in automated builds.  Below is an eclided version of
the Bootstrapper that my current project is using.  We've simply added some
static convenience methods to start the ObjectFactory Container if it hasn't
already been initialized.


{% highlight csharp %}
public class Bootstrapper : IBootstrapper
{
    private static bool _hasStarted;

    public void BootstrapStructureMap()
    {
        ObjectFactory.Initialize(x =>
        {
            // We put the properties for an NHibernate ISession
            // in the StructureMap.config file, so this file
            // must be there for our application to 
            // function correctly
            x.UseDefaultStructureMapConfigFile = true;

            x.AddRegistry(new CoreRegistry());
            x.AddRegistry(new SearchRegistry());
            x.AddRegistry(new WebCoreRegistry());
            x.AddRegistry(new RuleRegistry());
        });
    }

    public static void Restart()
    {
        if (_hasStarted)
        {
            ObjectFactory.ResetDefaults();
        }
        else
        {
            Bootstrap();
            _hasStarted = true;
        }
    }

    public static void Bootstrap()
    {
        new Bootstrapper().BootstrapStructureMap();
    }
}
{% endhighlight %}


  


 In test fixtures, we can simply do this:


  


{% highlight csharp %}
[SetUp]
public void SetUp()
{
    Bootstrapper.Restart();
}
{% endhighlight %}


  


Starting the container is a relatively expensive activity, so we only want to
call ObjectFactory.Initialize().  The call to ObjectFactory.ResetDefaults()
simply clears default services injected into the container after
initialization.  This is mostly used to clear out any mock or stub objects
introduced in a unit test after the unit test is over.


  


#### StructureMapConfiguration


StructureMapConfiguration is an earlier version of the Registry DSL and is
deprecated.  It is strongly recommended that you use Registry classes and the
ObjectFactory.Initialize() method for initialization as the
StructureMapConfiguration class has turned out to be very problematic in real
world usage.


 

