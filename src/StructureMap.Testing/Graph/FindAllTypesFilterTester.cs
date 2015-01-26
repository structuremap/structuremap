using System.Linq;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class FindAllTypesFilterTester
    {
        [Test]
        public void it_registers_types_that_can_be_cast()
        {
            var filter = new FindAllTypesFilter(typeof (IGeneric<>));
            var registry = new Registry();

            filter.Process(typeof (Generic<>), registry);

            var graph = registry.Build();

            graph.Families[typeof (IGeneric<>)].Instances.Single().ShouldBeOfType<ConstructorInstance>()
                .PluggedType.ShouldEqual(typeof (Generic<>));
        }


        [Test]
        public void it_registers_types_implementing_the_closed_generic_version()
        {
            var filter = new FindAllTypesFilter(typeof (IGeneric<>));
            var registry = new Registry();

            filter.Process(typeof (StringGeneric), registry);

            var graph = registry.Build();

            graph.Families[typeof (IGeneric<string>)].Instances.Single().ShouldBeOfType<ConstructorInstance>()
                .PluggedType.ShouldEqual(typeof (StringGeneric));
        }

        [Test]
        public void it_registers_open_types_which_can_be_cast()
        {
            var filter = new FindAllTypesFilter(typeof (IGeneric<>));
            var registry = new Registry();

            filter.Process(typeof (ConcreteGeneric<>), registry);

            var graph = registry.Build();

            graph.Families[typeof (IGeneric<>)].Instances.Single().ShouldBeOfType<ConstructorInstance>()
                .PluggedType.ShouldEqual(typeof (ConcreteGeneric<>));
        }


        public class Generic<T> : IGeneric<T>
        {
            public void Nop()
            {
            }
        }

        public interface IGeneric<T>
        {
            void Nop();
        }

        public class StringGeneric : Generic<string>
        {
        }

        public class ConcreteGeneric<T> : Generic<T>
        {
        }
    }
}