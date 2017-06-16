using Shouldly;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;
using StructureMap.TypeRules;
using System.Linq;
using Xunit;

namespace StructureMap.Testing
{
    public class TypeExtensionsTester
    {
        public class Service1 : IService<string>
        {
        }

        public class Service2
        {
        }

        public class Service2<T> : IService<T>
        {
        }

        public class SuperService : IService<decimal>, IService<float>
        {
        }

        public class SpecificService : Service2<string>
        {
        }

        public interface ServiceInterface : IService<string>
        {
        }

        [Fact]
        public void find_first_interface_that_closes_open_interface()
        {
            typeof(Service1).FindFirstInterfaceThatCloses(typeof(IService<>))
                .ShouldBe(typeof(IService<string>));

            typeof(Service2).FindFirstInterfaceThatCloses(typeof(IService<>))
                .ShouldBeNull();
        }

        [Fact]
        public void find_all_interfaces_that_close_an_open_interface()
        {
            typeof(SuperService).FindInterfacesThatClose(typeof(IService<>))
                .ShouldHaveTheSameElementsAs(typeof(IService<decimal>), typeof(IService<float>));
        }

        [Fact]
        public void find_all_interfaces_that_close_an_open_interface_should_not_return_the_same_type_twice()
        {
            var types = typeof(SpecificService).FindInterfacesThatClose(typeof(IService<>));
            types.Count().ShouldBe(1);
            types.ShouldHaveTheSameElementsAs(typeof(IService<string>));
        }

        [Fact]
        public void get_all_interfaces()
        {
            typeof(C3).AllInterfaces()
                .OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(I1), typeof(I2), typeof(I3));
            typeof(C2).AllInterfaces().OrderBy(x => x.Name).ShouldHaveTheSameElementsAs(typeof(I1), typeof(I2));
            typeof(C1).AllInterfaces().OrderBy(x => x.Name).ShouldHaveTheSameElementsAs(typeof(I1));
        }

        [Fact]
        public void implements_interface_template()
        {
            typeof(Service1).ImplementsInterfaceTemplate(typeof(IService<>))
                .ShouldBeTrue();

            typeof(Service2).ImplementsInterfaceTemplate(typeof(IService<>))
                .ShouldBeFalse();

            typeof(ServiceInterface).ImplementsInterfaceTemplate(typeof(IService<>))
                .ShouldBeFalse();
        }

        [Fact]
        public void IsChild()
        {
            typeof(int).IsChild().ShouldBeFalse();
            typeof(bool).IsChild().ShouldBeFalse();
            typeof(double).IsChild().ShouldBeFalse();
            typeof(string).IsChild().ShouldBeFalse();
            typeof(BreedEnum).IsChild().ShouldBeFalse();
            typeof(IGateway[]).IsChild().ShouldBeFalse();
            typeof(IGateway).IsChild().ShouldBeTrue();
        }

        [Fact]
        public void IsChildArray()
        {
            typeof(int).IsChildArray().ShouldBeFalse();
            typeof(bool).IsChildArray().ShouldBeFalse();
            typeof(double).IsChildArray().ShouldBeFalse();
            typeof(double[]).IsChildArray().ShouldBeFalse();
            typeof(string).IsChildArray().ShouldBeFalse();
            typeof(string[]).IsChildArray().ShouldBeFalse();
            typeof(BreedEnum).IsChildArray().ShouldBeFalse();
            typeof(IGateway[]).IsChildArray().ShouldBeTrue();
            typeof(IGateway).IsChildArray().ShouldBeFalse();
        }

        [Fact]
        public void IsPrimitive()
        {
            typeof(int).IsPrimitive().ShouldBeTrue();
            typeof(bool).IsPrimitive().ShouldBeTrue();
            typeof(double).IsPrimitive().ShouldBeTrue();
            typeof(string).IsPrimitive().ShouldBeFalse();
            typeof(BreedEnum).IsPrimitive().ShouldBeFalse();
            typeof(IGateway).IsPrimitive().ShouldBeFalse();
        }

        [Fact]
        public void IsSimple()
        {
            typeof(int).IsSimple().ShouldBeTrue();
            typeof(bool).IsSimple().ShouldBeTrue();
            typeof(double).IsSimple().ShouldBeTrue();
            typeof(string).IsSimple().ShouldBeTrue();
            typeof(BreedEnum).IsSimple().ShouldBeTrue();
            typeof(IGateway).IsSimple().ShouldBeFalse();
            typeof(int?).IsSimple().ShouldBeTrue();
        }

        [Fact]
        public void IsString()
        {
            typeof(string).IsString().ShouldBeTrue();
            typeof(int).IsString().ShouldBeFalse();
        }
    }

    public interface I1
    {
    }

    public interface I2
    {
    }

    public interface I3
    {
    }

    public class C1 : I1
    {
    }

    public class C2 : C1, I2
    {
    }

    public class C3 : C2, I3
    {
    }
}