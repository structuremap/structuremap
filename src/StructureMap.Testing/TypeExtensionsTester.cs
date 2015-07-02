using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;
using StructureMap.TypeRules;

namespace StructureMap.Testing
{
    [TestFixture]
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

        [Test]
        public void find_first_interface_that_closes_open_interface()
        {
            typeof (Service1).FindFirstInterfaceThatCloses(typeof (IService<>))
                .ShouldBe(typeof (IService<string>));

            typeof (Service2).FindFirstInterfaceThatCloses(typeof (IService<>))
                .ShouldBeNull();
        }

        [Test]
        public void find_all_interfaces_that_close_an_open_interface()
        {
            typeof (SuperService).FindInterfacesThatClose(typeof (IService<>))
                .ShouldHaveTheSameElementsAs(typeof (IService<decimal>), typeof (IService<float>));
        }

        [Test]
        public void find_all_interfaces_that_close_an_open_interface_should_not_return_the_same_type_twice()
        {
            var types = typeof (SpecificService).FindInterfacesThatClose(typeof (IService<>));
            types.Count().ShouldBe(1);
            types.ShouldHaveTheSameElementsAs(typeof (IService<string>));
        }


        [Test]
        public void get_all_interfaces()
        {
            typeof (C3).AllInterfaces()
                .OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof (I1), typeof (I2), typeof (I3));
            typeof (C2).AllInterfaces().OrderBy(x => x.Name).ShouldHaveTheSameElementsAs(typeof (I1), typeof (I2));
            typeof (C1).AllInterfaces().OrderBy(x => x.Name).ShouldHaveTheSameElementsAs(typeof (I1));
        }

        [Test]
        public void implements_interface_template()
        {
            typeof (Service1).ImplementsInterfaceTemplate(typeof (IService<>))
                .ShouldBeTrue();

            typeof (Service2).ImplementsInterfaceTemplate(typeof (IService<>))
                .ShouldBeFalse();

            typeof (ServiceInterface).ImplementsInterfaceTemplate(typeof (IService<>))
                .ShouldBeFalse();
        }

        [Test]
        public void IsChild()
        {
            typeof (int).IsChild().IsFalse();
            typeof (bool).IsChild().IsFalse();
            typeof (double).IsChild().IsFalse();
            typeof (string).IsChild().IsFalse();
            typeof (BreedEnum).IsChild().IsFalse();
            typeof (IGateway[]).IsChild().IsFalse();
            typeof (IGateway).IsChild().IsTrue();
        }

        [Test]
        public void IsChildArray()
        {
            typeof (int).IsChildArray().IsFalse();
            typeof (bool).IsChildArray().IsFalse();
            typeof (double).IsChildArray().IsFalse();
            typeof (double[]).IsChildArray().IsFalse();
            typeof (string).IsChildArray().IsFalse();
            typeof (string[]).IsChildArray().IsFalse();
            typeof (BreedEnum).IsChildArray().IsFalse();
            typeof (IGateway[]).IsChildArray().IsTrue();
            typeof (IGateway).IsChildArray().IsFalse();
        }

        [Test]
        public void IsPrimitive()
        {
            typeof (int).IsPrimitive().IsTrue();
            typeof (bool).IsPrimitive().IsTrue();
            typeof (double).IsPrimitive().IsTrue();
            typeof (string).IsPrimitive().IsFalse();
            typeof (BreedEnum).IsPrimitive().IsFalse();
            typeof (IGateway).IsPrimitive().IsFalse();
        }

        [Test]
        public void IsSimple()
        {
            typeof (int).IsSimple().IsTrue();
            typeof (bool).IsSimple().IsTrue();
            typeof (double).IsSimple().IsTrue();
            typeof (string).IsSimple().IsTrue();
            typeof (BreedEnum).IsSimple().IsTrue();
            typeof (IGateway).IsSimple().IsFalse();
        }

        [Test]
        public void IsString()
        {
            typeof (string).IsString().IsTrue();
            typeof (int).IsString().IsFalse();
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