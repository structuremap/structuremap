using Shouldly;
using StructureMap.Graph;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class FindAllTypesFilterTester
    {
        [Fact]
        public void it_registers_types_that_can_be_cast()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf(typeof(IGeneric<>));
                });
            });

            container.Model.For(typeof(IGeneric<>)).Instances.Any(x => x.ReturnedType == typeof(Generic<>))
                .ShouldBeTrue();
        }

        [Fact]
        public void it_registers_types_implementing_the_closed_generic_version()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf(typeof(IGeneric<>));
                });
            });

            container.Model.For<IGeneric<string>>().Instances.Single()
                .ReturnedType.ShouldBe(typeof(StringGeneric));
        }

        [Fact]
        public void it_registers_open_types_which_can_be_cast()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf(typeof(IGeneric<>));
                });
            });

            container.Model.For(typeof(IGeneric<>)).Instances.Any(x => x.ReturnedType == typeof(ConcreteGeneric<>))
                .ShouldBeTrue();
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