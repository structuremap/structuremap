using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using Shouldly;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Compliance
{
    [TestFixture]
    public class IServiceLocator_Compliance
    {
        [Test]
        public void get_instance_by_type()
        {
            var services = ContainerFacilitySource.Services(x => {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
                x.Register(typeof(IThing), ObjectDef.ForType<ThingOne>());
                
            });

            services.GetInstance<IService>().ShouldBeOfType<SimpleService>();
            services.GetInstance<IThing>().ShouldBeOfType<ThingOne>();

            services.GetInstance(typeof(IService)).ShouldBeOfType<SimpleService>();
            services.GetInstance(typeof(IThing)).ShouldBeOfType<ThingOne>();
        }

        [Test]
        public void get_instance_by_name()
        {
            var services = ContainerFacilitySource.Services(x =>
            {
                x.Register(typeof(IThing), ObjectDef.ForType<ThingOne>().Named("One"));
                x.Register(typeof(IThing), ObjectDef.ForType<ThingTwo>().Named("Two"));

            });

            services.GetInstance<IThing>("One").ShouldBeOfType<ThingOne>();
            services.GetInstance<IThing>("Two").ShouldBeOfType<ThingTwo>();
        }
    }
}