using NUnit.Framework;
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

        public interface ServiceInterface : IService<string>
        {
        }

        [Test]
        public void find_interface_that_closes_open_interface()
        {
            typeof (Service1).FindInterfaceThatCloses(typeof (IService<>))
                .ShouldEqual(typeof (IService<string>));

            typeof (Service2).FindInterfaceThatCloses(typeof (IService<>))
                .ShouldBeNull();
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
            Assert.IsFalse(typeof (int).IsChild());
            Assert.IsFalse(typeof (bool).IsChild());
            Assert.IsFalse(typeof (double).IsChild());
            Assert.IsFalse(typeof (string).IsChild());
            Assert.IsFalse(typeof (BreedEnum).IsChild());
            Assert.IsFalse(typeof (IGateway[]).IsChild());
            Assert.IsTrue(typeof (IGateway).IsChild());
        }

        [Test]
        public void IsChildArray()
        {
            Assert.IsFalse(typeof (int).IsChildArray());
            Assert.IsFalse(typeof (bool).IsChildArray());
            Assert.IsFalse(typeof (double).IsChildArray());
            Assert.IsFalse(typeof (double[]).IsChildArray());
            Assert.IsFalse(typeof (string).IsChildArray());
            Assert.IsFalse(typeof (string[]).IsChildArray());
            Assert.IsFalse(typeof (BreedEnum).IsChildArray());
            Assert.IsTrue(typeof (IGateway[]).IsChildArray());
            Assert.IsFalse(typeof (IGateway).IsChildArray());
        }

        [Test]
        public void IsPrimitive()
        {
            Assert.IsTrue(typeof (int).IsPrimitive());
            Assert.IsTrue(typeof (bool).IsPrimitive());
            Assert.IsTrue(typeof (double).IsPrimitive());
            Assert.IsFalse(typeof (string).IsPrimitive());
            Assert.IsFalse(typeof (BreedEnum).IsPrimitive());
            Assert.IsFalse(typeof (IGateway).IsPrimitive());
        }

        [Test]
        public void IsSimple()
        {
            Assert.IsTrue(typeof (int).IsSimple());
            Assert.IsTrue(typeof (bool).IsSimple());
            Assert.IsTrue(typeof (double).IsSimple());
            Assert.IsTrue(typeof (string).IsSimple());
            Assert.IsTrue(typeof (BreedEnum).IsSimple());
            Assert.IsFalse(typeof (IGateway).IsSimple());
        }

        [Test]
        public void IsString()
        {
            Assert.IsTrue(typeof (string).IsString());
            Assert.IsFalse(typeof (int).IsString());
        }

        [Test]
        public void get_all_interfaces()
        {
            typeof(C3).AllInterfaces().ShouldHaveTheSameElementsAs(typeof(I1), typeof(I2), typeof(I3));
            typeof(C2).AllInterfaces().ShouldHaveTheSameElementsAs(typeof(I1), typeof(I2));
            typeof(C1).AllInterfaces().ShouldHaveTheSameElementsAs(typeof(I1));
        }
    }


    public interface I1{}
    public interface I2{}
    public interface I3{}

    public class C1 : I1{}
    public class C2 : C1, I2{}
    public class C3 : C2, I3{}
}