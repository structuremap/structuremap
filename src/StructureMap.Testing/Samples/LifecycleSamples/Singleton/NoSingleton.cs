using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Samples.LifecycleSamples.Singleton
{
    // SAMPLE: no-singleton
    public interface ISingletonDependency
    {
        void DoSomething();
    }

    public class Configuration : Registry
    {
        public Configuration()
        {
            // Tell StructureMap that all types
            // of ISingletonDependency are
            // to be scoped as "singletons"
            For<ISingletonDependency>()
                .Singleton();
        }
    }

    // This version of the class just uses constructor 
    // injection to get an object instance of
    // ISingletonDependency. 
    public class DependencyUser
    {
        private readonly ISingletonDependency _dependency;

        public DependencyUser(ISingletonDependency dependency)
        {
            _dependency = dependency;
        }
    }

    // ENDSAMPLE
}