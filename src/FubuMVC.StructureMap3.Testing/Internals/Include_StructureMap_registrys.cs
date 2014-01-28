using System.Linq;
using FubuMVC.Core;
using FubuMVC.StructureMap3.Testing.Compliance;
using NUnit.Framework;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Internals
{
    [TestFixture]
    public class Include_StructureMap_registrys
    {
        [Test]
        public void registries_registered_to_fubumvc_by_value_will_be_applied_to_structureMap_container()
        {
            var container = new Container();

            FubuApplication.For<FooFubuRegistry>().StructureMap(container).Bootstrap();

            container.GetAllInstances<IFoo>().Select(x => x.GetType()).OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(BlueFoo), typeof(GreenFoo), typeof(RedFoo));
        }


        [Test]
        public void registries_registered_to_fubumvc_by_type_will_be_applied_to_structuremap_container()
        {
            var container = new Container();

            FubuApplication.For<FooFubuRegistry2>().StructureMap(container).Bootstrap();

            container.GetAllInstances<IFoo>().Select(x => x.GetType()).OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(BlueFoo), typeof(GreenFoo), typeof(RedFoo));
        }
    }

    public class FooFubuRegistry : FubuRegistry
    {
        public FooFubuRegistry()
        {
            Services(x => x.AddService<Registry>(new FooRegistry()));
        }
    }

    public class FooFubuRegistry2 : FubuRegistry
    {
        public FooFubuRegistry2()
        {
            Services(x => x.AddService<Registry, FooRegistry>());
        }
    }

    public class FooRegistry : Registry
    {
        public FooRegistry()
        {
            Scan(x => {
                x.TheCallingAssembly();
                x.AddAllTypesOf<IFoo>();
            });
        }
    }

    public interface IFoo
    {
        
    }

    public class RedFoo : IFoo{}
    public class GreenFoo : IFoo{}
    public class BlueFoo : IFoo{}
}