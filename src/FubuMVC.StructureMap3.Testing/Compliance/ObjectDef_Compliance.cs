using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using Shouldly;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Compliance
{
    [TestFixture]
    public class ObjectDef_Compliance
    {
        [Test]
        public void simple_ObjectDef_by_type()
        {
            var container = ContainerFacilitySource.New(x => {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
            });

            container.Get<IService>().ShouldBeOfType<SimpleService>();
        }

        [Test]
        public void simple_ObjectDef_by_value()
        {
            var service = new SimpleService();

            var container = ContainerFacilitySource.New(x => {
                x.Register(typeof (IService), ObjectDef.ForValue(service));
            });

            container.Get<IService>().ShouldBeTheSameAs(service);
        }

        [Test]
        public void auto_wiring_applies_when_the_dependency_is_not_set_explicitly()
        {
            var container = ContainerFacilitySource.New(x =>
            {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
            });

            container.Get<GuyWithService>().Service.ShouldBeOfType<SimpleService>();
        }

		[Test]
		public void auto_wiring_applies_even_when_another_dependency_is_set_explicitly() {
			var container = ContainerFacilitySource.New(x => {
				x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
				x.Register(typeof(IThing), ObjectDef.ForType<ThingOne>());

				var def = ObjectDef.ForType<GuyWithServiceAndThing>();
				def.DependencyByType<IThing>(ObjectDef.ForType<ThingTwo>());

				var highLevelDef = ObjectDef.ForType<HighLevelObject>();
				highLevelDef.DependencyByType<GuyWithServiceAndThing>(def);

				x.Register(typeof(IHighLevelObject), highLevelDef);
			});

			var guyWithServiceAndThing = container.Get<GuyWithServiceAndThing>();
			guyWithServiceAndThing.Service.ShouldBeOfType<SimpleService>(); // auto-wired
			guyWithServiceAndThing.Thing.ShouldBeOfType<ThingOne>(); // auto-wired, even though explicitly set to ThingTwo for HighLevelObject
		}

        [Test]
        public void ObjectDef_with_one_explicit_dependency_defined_by_type()
        {
            var container = ContainerFacilitySource.New(x =>
            {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());

                var objectDef = ObjectDef.ForType<GuyWithService>();
                objectDef.DependencyByType<IService>(ObjectDef.ForType<DifferentService>());

                x.Register(typeof(GuyWithService), objectDef);
            });

            // The default IService is SimpleService, but the default ObjectDef (first one) explicitly
            // set up its IService dependency to be a "DifferentService"
            container.Get<GuyWithService>().Service.ShouldBeOfType<DifferentService>();
        }


        [Test]
        public void ObjectDef_with_one_explicit_dependency_defined_by_value()
        {
            var container = ContainerFacilitySource.New(x =>
            {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());

                var objectDef = ObjectDef.ForType<GuyWithService>();
                objectDef.DependencyByType<IService>(ObjectDef.ForValue(new DifferentService()));

                x.Register(typeof(GuyWithService), objectDef);
            });

            // The default IService is SimpleService, but the default ObjectDef (first one) explicitly
            // set up its IService dependency to be a "DifferentService"
            container.Get<GuyWithService>().Service.ShouldBeOfType<DifferentService>();
        }

        [Test]
        public void ObjectDef_with_one_explicit_and_one_implicit_dependency()
        {
            var container = ContainerFacilitySource.New(x => {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
                x.Register(typeof(IThing), ObjectDef.ForType<ThingOne>());

                var def = ObjectDef.ForType<GuyWithServiceAndThing>();
                def.DependencyByType<IThing>(ObjectDef.ForType<ThingTwo>());

                x.Register(typeof(GuyWithServiceAndThing), def);
            });

            var guyWithServiceAndThing = container.Get<GuyWithServiceAndThing>();
            guyWithServiceAndThing.Service.ShouldBeOfType<SimpleService>(); // auto-wired
            guyWithServiceAndThing.Thing.ShouldBeOfType<ThingTwo>(); // explicitly set to be ThingTwo, even though auto-wiring would have put ThingOne here

        }

        [Test]
        public void three_deep_explicitly_configured_dep_tree()
        {
            var container = ContainerFacilitySource.New(x => {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
                x.Register(typeof(IThing), ObjectDef.ForType<ThingOne>());

                var def = ObjectDef.ForType<GuyWithServiceAndThing>();
                def.DependencyByType<IThing>(ObjectDef.ForType<ThingTwo>());

                var highLevelDef = ObjectDef.ForType<HighLevelObject>();
                highLevelDef.DependencyByType<GuyWithServiceAndThing>(def);

                x.Register(typeof(IHighLevelObject), highLevelDef);
            });

            var guyWithServiceAndThing = container.Get<IHighLevelObject>().Guy;
            guyWithServiceAndThing.Service.ShouldBeOfType<SimpleService>(); // auto-wired
            guyWithServiceAndThing.Thing.ShouldBeOfType<ThingTwo>(); // explicitly set to be ThingTwo, even though auto-wiring would have put ThingOne here
        }

        [Test]
        public void can_get_all_registered_implementations_of_a_service()
        {
            var container = ContainerFacilitySource.New(x =>
            {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
                x.Register(typeof(IService), ObjectDef.ForType<DifferentService>());
                x.Register(typeof(IService), ObjectDef.ForType<ExceptionCaseService>());
            });

            container.GetAll<IService>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(SimpleService), typeof(DifferentService), typeof(ExceptionCaseService));
        }

        [Test]
        public void implicitly_auto_wires_all_implementations_of_a_service_if_not_explicitly_overridden()
        {
            var container = ContainerFacilitySource.New(x => {
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
                x.Register(typeof(IService), ObjectDef.ForType<DifferentService>());
                x.Register(typeof(IService), ObjectDef.ForType<ExceptionCaseService>());
            });

            container.Get<ThingThatUsesLotsOfServices>()
                .Services.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(SimpleService), typeof(DifferentService), typeof(ExceptionCaseService));
        }


        [Test]
        public void implicitly_auto_wires_all_implementations_of_a_service_if_not_explicitly_overridden_and_maintains_the_order_of_registration()
        {
            var container = ContainerFacilitySource.New(x =>
            {
                x.Register(typeof(IService), ObjectDef.ForType<ExceptionCaseService>());
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
                x.Register(typeof(IService), ObjectDef.ForType<DifferentService>());
                
            });

            container.Get<ThingThatUsesLotsOfServices>()
                .Services.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(ExceptionCaseService), typeof(SimpleService), typeof(DifferentService));
        }

        [Test]
        public void explicit_registration_of_an_ienumerable_argument()
        {
            var container = ContainerFacilitySource.New(x =>
            {
                x.Register(typeof(IService), ObjectDef.ForType<ExceptionCaseService>());
                x.Register(typeof(IService), ObjectDef.ForType<SimpleService>());
                x.Register(typeof(IService), ObjectDef.ForType<DifferentService>());


                var def = ObjectDef.ForType<ThingThatUsesLotsOfServices>();
                def.EnumerableDependenciesOf<IService>().Add(ObjectDef.ForType<OddballService>());
                def.EnumerableDependenciesOf<IService>().Add(ObjectDef.ForType<DifferentService>());


                x.Register(typeof(ThingThatUsesLotsOfServices), def);
            });

            container.Get<ThingThatUsesLotsOfServices>()
                .Services.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(OddballService), typeof(DifferentService));
        }

        [Test]
        public void explicit_registration_of_a_primitive_argument()
        {
            var container = ContainerFacilitySource.New(x => {
                var def = ObjectDef.ForType<GuyWithPrimitive>();
                def.DependencyByValue("Jeremy");

                x.Register(typeof(GuyWithPrimitive), def);
            });

            container.Get<GuyWithPrimitive>()
                .Name.ShouldBe("Jeremy");
        }
    }

    public class GuyWithPrimitive
    {
        private readonly string _name;

        public GuyWithPrimitive(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    public class ThingThatUsesLotsOfServices
    {
        private readonly IEnumerable<IService> _services;

        public ThingThatUsesLotsOfServices(IEnumerable<IService> services)
        {
            _services = services;
        }

        public IEnumerable<IService> Services
        {
            get { return _services; }
        }
    }

    public interface IHighLevelObject
    {
        GuyWithServiceAndThing Guy { get; }
    }

    public class HighLevelObject : IHighLevelObject
    {
        private readonly GuyWithServiceAndThing _guy;

        public HighLevelObject(GuyWithServiceAndThing guy)
        {
            _guy = guy;
        }

        public GuyWithServiceAndThing Guy
        {
            get { return _guy; }
        }
    }

    public class GuyWithServiceAndThing
    {
        private readonly IService _service;
        private readonly IThing _thing;

        public GuyWithServiceAndThing(IService service, IThing thing)
        {
            _service = service;
            _thing = thing;
        }

        public IService Service
        {
            get { return _service; }
        }

        public IThing Thing
        {
            get { return _thing; }
        }
    }

    public class GuyWithService
    {
        private readonly IService _service;

        public GuyWithService(IService service)
        {
            _service = service;
        }

        public IService Service
        {
            get { return _service; }
        }
    }

    public interface IService
    {
        
    }

    public class DifferentService : IService
    {
        
    }

    public class SimpleService : IService
    {
        
    }

    public class OddballService : IService
    {
        
    }

    public class ExceptionCaseService : IService
    {
        
    }

    public interface IThing
    {
        
    }

    public class ThingOne : IThing
    {
        
    }

    public class ThingTwo : IThing
    {

    }
}