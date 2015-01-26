using StructureMap.Configuration.DSL;

namespace StructureMap.Docs.samples
{
// SAMPLE: foobar-registry
public class FooBarRegistry : Registry
{
    public FooBarRegistry()
    {
        For<IFoo>().Use<Foo>();
        For<IBar>().Use<Bar>();
    }
}
// ENDSAMPLE

// SAMPLE: foo-registry
public class FooRegistry : Registry
{
    public FooRegistry()
    {
        For<IFoo>().Use<Foo>();        
    }
}
// ENDSAMPLE
}
