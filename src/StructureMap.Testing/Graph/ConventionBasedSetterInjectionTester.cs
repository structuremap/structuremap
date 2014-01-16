using NUnit.Framework;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class ConventionBasedSetterInjectionTester
    {
        public class ClassWithNamedProperties
        {
            public int Age { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public IGateway Gateway { get; set; }
            public IService Service { get; set; }
        }


        [Test]
        public void specify_setter_policy_and_construct_an_object()
        {
            var theService = new ColorService("red");

            var container = new Container(x => {
                x.For<IService>().Use(theService);
                x.For<IGateway>().Use<DefaultGateway>();

                x.SetAllProperties(policy => { policy.WithAnyTypeFromNamespace("StructureMap.Testing.Widget3"); });
            });

            var target = container.GetInstance<ClassWithNamedProperties>();
            target.Service.ShouldBeTheSameAs(theService);
            target.Gateway.ShouldBeOfType<DefaultGateway>();
        }

        [Test]
        public void specify_setter_policy_by_a_predicate_on_property_type()
        {
            var theService = new ColorService("red");

            var container = new Container(x => {
                x.For<IService>().Use(theService);
                x.For<IGateway>().Use<DefaultGateway>();

                x.SetAllProperties(policy => { policy.TypeMatches(type => type == typeof (IService)); });
            });

            var target = container.GetInstance<ClassWithNamedProperties>();
            target.Service.ShouldBeTheSameAs(theService);
            target.Gateway.ShouldBeNull();
        }
    }
}