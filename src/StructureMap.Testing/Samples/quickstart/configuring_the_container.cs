using System.Data;
using System.Data.SqlClient;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Docs.samples.quickstart
{
    class configuring_the_container
    {
        public void configure_the_container()
        {
// SAMPLE: quickstart-configure-the-container
// Example #1 - Create an container instance and directly pass in the configuration.
var container1 = new Container(c =>
{
    c.For<IFoo>().Use<Foo>();
    c.For<IBar>().Use<Bar>();
});

// Example #2 - Create an container instance but add configuration later.
var container2 = new Container();

container2.Configure(c =>
{
    c.For<IFoo>().Use<Foo>();
    c.For<IBar>().Use<Bar>();
});

// Example #3 - Initialize the static ObjectFactory container.
// NOTE: ObjectFactory has been deprecated and will be removed
// in a future 4.0 release
ObjectFactory.Initialize(i =>
{
    i.For<IFoo>().Use<Foo>();
    i.For<IBar>().Use<Bar>();
});
// ENDSAMPLE
        }

        public void configure_the_container_using_a_registry()
        {
// SAMPLE: quickstart-configure-the-container-using-a-registry
// Example #1
var container1 = new Container(new FooBarRegistry());

// Example #2
var container2 = new Container(c =>
{
    c.AddRegistry<FooBarRegistry>();
});

// Example #3 -- create a container for a single Registry
var container3 = Container.For<FooBarRegistry>();
// ENDSAMPLE
        }
        
        public void configure_the_container_using_auto_registrations_and_conventions()
        {
// SAMPLE: quickstart-configure-the-container-using-auto-registrations-and-conventions
// Example #1
var container1 = new Container(c =>
    c.Scan(scanner =>
    {
        scanner.TheCallingAssembly();
        scanner.WithDefaultConventions();
    }));

// Example #2
var container2 = new Container();

container2.Configure(c =>
    c.Scan(scanner =>
    {
        scanner.TheCallingAssembly();
        scanner.WithDefaultConventions();
    }));


// ENDSAMPLE
        }
        
        public void configure_the_container_and_provide_a_primitive()
        {
// SAMPLE: quickstart-container-with-primitive-value
var container = new Container(c =>
{
    //just for demo purposes, normally you don't want to embed the connection string directly into code.
    c.For<IDbConnection>().Use<SqlConnection>().Ctor<string>().Is("YOUR_CONNECTION_STRING");    
    //a better way would be providing a delegate that retrieves the value from your app config.    
});
// ENDSAMPLE
        }


        public void configure_multiple_services_of_the_same_type()
        {
// SAMPLE: quickstart-configure-multiple-services-of-the-same-type
var container = new Container(i => {
    i.For<IFoo>().Use<Foo>();
    i.For<IFoo>().Use<SomeOtherFoo>();
    i.For<IBar>().Use<Bar>();
});
// ENDSAMPLE
        }
    }
}
