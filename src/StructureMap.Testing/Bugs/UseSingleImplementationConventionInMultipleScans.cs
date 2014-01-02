using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMapBugRepo.NS1;
using StructureMapBugRepo.NS2;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class UseSingleImplementationConventionInMultipleScans
    {
        public void Repro()
        {
            var container = new Container(i => {
                // Add our two registries. One is shared, the other is specific to this project.
                // NOTE: Changing the order of these two will determine which GetInstance call fails.
                i.AddRegistry<SharedRegistry>();
                i.AddRegistry<MyRegistry>();
            });

            // One of these will, depending on the order of the two AddRegistry calls above.
            container.TryGetInstance<IShared>().ShouldNotBeNull();
            container.TryGetInstance<IMine>().ShouldNotBeNull();
        }
    }
}

namespace StructureMapBugRepo.NS1
{
    public class SharedRegistry : Registry
    {
        public SharedRegistry()
        {
            Scan(s => {
                // For this sample, we're using Namespaces, but in my real project, 
                // it's two different assemblies, and has the exact same issue.
                s.TheCallingAssembly();
                s.IncludeNamespaceContainingType<IShared>();
                s.SingleImplementationsOfInterface();
            });
        }
    }

    // Dummy class/interfaces just for sample.
    public interface IShared
    {
    }

    public class Shared : IShared
    {
    }
}

namespace StructureMapBugRepo.NS2
{
    public class MyRegistry : Registry
    {
        public MyRegistry()
        {
            Scan(s => {
                // For this sample, we're using Namespaces, but in my real project, 
                // it's two different assemblies, and has the exact same issue.
                s.TheCallingAssembly();
                s.IncludeNamespaceContainingType<IMine>();
                s.SingleImplementationsOfInterface();
            });
        }
    }

    // Dummy class/interfaces just for sample.
    public interface IMine
    {
    }

    public class Mine : IMine
    {
    }
}