using NUnit.Framework;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class BuildUpIntegratedTester
    {
        [Test]
        public void create_a_setter_rule_and_see_it_applied_in_BuildUp()
        {
            var theGateway = new DefaultGateway();
            var container = new Container(x => {
                x.For<IGateway>().Use(theGateway);
                x.Polices.SetAllProperties(y => { y.OfType<IGateway>(); });
            });

            var target = new BuildUpTarget1();
            container.BuildUp(target);

            target.Gateway.ShouldBeTheSameAs(theGateway);
            target.Service.ShouldBeNull();
        }

        [Test]
        public void create_a_setter_rule_and_see_it_applied_in_BuildUp_through_ObjectFactory()
        {
            var theGateway = new DefaultGateway();
            ObjectFactory.Initialize(x => {
                x.For<IGateway>().Use(theGateway);

                // First we create a new Setter Injection Policy that
                // forces StructureMap to inject all public properties
                // where the PropertyType is IGateway
                x.Polices.SetAllProperties(y => { y.OfType<IGateway>(); });
            });

            // Create an instance of BuildUpTarget1
            var target = new BuildUpTarget1();

            // Now, call BuildUp() on target, and
            // we should see the Gateway property assigned
            ObjectFactory.BuildUp(target);

            target.Gateway.ShouldBeTheSameAs(theGateway);
        }

        [Test]
        public void use_predefined_setter_values_for_buildup()
        {
            var theGateway = new DefaultGateway();
            var container = new Container(x => {
                x.ForConcreteType<BuildUpTarget1>().Configure
                    .Setter(y => y.Gateway).Is(theGateway);
            });

            var target = new BuildUpTarget1();
            container.BuildUp(target);

            target.Gateway.ShouldBeTheSameAs(theGateway);
        }
    }

    public class BuildUpTarget1
    {
        public IGateway Gateway { get; set; }
        public IService Service { get; set; }
    }
}