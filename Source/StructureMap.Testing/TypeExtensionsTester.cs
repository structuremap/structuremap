using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap.Testing.GenericWidgets;

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
    }
}
