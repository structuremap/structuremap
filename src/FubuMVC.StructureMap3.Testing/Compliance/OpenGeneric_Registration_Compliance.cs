using System.Diagnostics;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using Shouldly;
using StructureMap;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Compliance
{
    [TestFixture]
    public class OpenGeneric_Registration_Compliance
    {
        [Test]
        public void close_an_open_generic_type_from_registration_if_nothing_explicit_is_added()
        {
            var facility = ContainerFacilitySource.New(x => {
                x.Register(typeof(IService<>), new ObjectDef(typeof(SimpleService<>)));
            });

            facility.Get<IService<string>>().ShouldBeOfType<SimpleService<string>>();
        }

        [Test]
        public void use_the_closed_type_if_it_exists()
        {
            var facility = ContainerFacilitySource.New(x =>
            {
                x.Register(typeof(IService<>), new ObjectDef(typeof(SimpleService<>)));
                x.Register(typeof (IService<IThing>), ObjectDef.ForType<ThingService>());
            });

            facility.Get<IService<IThing>>().ShouldBeOfType<ThingService>();
        }
    }

    public interface IService<T>
    {
        
    }

    public class SimpleService<T> : IService<T>{}

    public class ThingService : IService<IThing>{}
}