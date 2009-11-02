using NUnit.Framework;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class TypeExtensionsTester
    {
        public class Service1 : IService<string>{}
        public class Service2 {}
        public class Service2<T> : IService<T>{}
        public interface ServiceInterface : IService<string>{}

        [Test]
        public void implements_interface_template()
        {
            typeof(Service1).ImplementsInterfaceTemplate(typeof(IService<>))
                .ShouldBeTrue();

            typeof(Service2).ImplementsInterfaceTemplate(typeof(IService<>))
                .ShouldBeFalse();

            typeof(ServiceInterface).ImplementsInterfaceTemplate(typeof(IService<>))
                .ShouldBeFalse();

            
        }

        [Test]
        public void find_interface_that_closes_open_interface()
        {
            typeof (Service1).FindInterfaceThatCloses(typeof (IService<>))
                .ShouldEqual(typeof (IService<string>));

            typeof(Service2).FindInterfaceThatCloses(typeof(IService<>))
                .ShouldBeNull();
        }

        [Test]
        public void IsChild()
        {
            Assert.IsFalse(TypeExtensions.IsChild(typeof(int)));
            Assert.IsFalse(TypeExtensions.IsChild(typeof(bool)));
            Assert.IsFalse(TypeExtensions.IsChild(typeof(double)));
            Assert.IsFalse(TypeExtensions.IsChild(typeof(string)));
            Assert.IsFalse(TypeExtensions.IsChild(typeof(BreedEnum)));
            Assert.IsFalse(TypeExtensions.IsChild(typeof(IGateway[])));
            Assert.IsTrue(TypeExtensions.IsChild(typeof(IGateway)));
        }

        [Test]
        public void IsChildArray()
        {
            Assert.IsFalse(TypeExtensions.IsChildArray(typeof(int)));
            Assert.IsFalse(TypeExtensions.IsChildArray(typeof(bool)));
            Assert.IsFalse(TypeExtensions.IsChildArray(typeof(double)));
            Assert.IsFalse(TypeExtensions.IsChildArray(typeof(double[])));
            Assert.IsFalse(TypeExtensions.IsChildArray(typeof(string)));
            Assert.IsFalse(TypeExtensions.IsChildArray(typeof(string[])));
            Assert.IsFalse(TypeExtensions.IsChildArray(typeof(BreedEnum)));
            Assert.IsTrue(TypeExtensions.IsChildArray(typeof(IGateway[])));
            Assert.IsFalse(TypeExtensions.IsChildArray(typeof(IGateway)));
        }

        [Test]
        public void IsPrimitive()
        {
            Assert.IsTrue(typeof(int).IsPrimitive());
            Assert.IsTrue(typeof(bool).IsPrimitive());
            Assert.IsTrue(typeof(double).IsPrimitive());
            Assert.IsFalse(typeof(string).IsPrimitive());
            Assert.IsFalse(typeof(BreedEnum).IsPrimitive());
            Assert.IsFalse(typeof(IGateway).IsPrimitive());
        }

        [Test]
        public void IsSimple()
        {
            Assert.IsTrue(TypeExtensions.IsSimple(typeof(int)));
            Assert.IsTrue(TypeExtensions.IsSimple(typeof(bool)));
            Assert.IsTrue(TypeExtensions.IsSimple(typeof(double)));
            Assert.IsTrue(TypeExtensions.IsSimple(typeof(string)));
            Assert.IsTrue(TypeExtensions.IsSimple(typeof(BreedEnum)));
            Assert.IsFalse(TypeExtensions.IsSimple(typeof(IGateway)));
        }

        [Test]
        public void IsString()
        {
            Assert.IsTrue(TypeExtensions.IsString(typeof(string)));
            Assert.IsFalse(TypeExtensions.IsString(typeof(int)));
        }

    }
}
