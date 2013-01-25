using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ProfileExpressionTester : Registry
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void Add_default_instance_by_lambda()
        {
            string theProfileName = "something";

            IContainer container = new Container(r =>
            {
                r.Profile(theProfileName, x =>
                {
                    x.For<IWidget>().Use(() => new AWidget());
                    x.For<Rule>().Use(() => new DefaultRule());
                });




            });

            container.SetDefaultsToProfile(theProfileName);


            container.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
            container.GetInstance<Rule>().ShouldBeOfType<DefaultRule>();
        }


        [Test]
        public void Add_default_instance_by_lambda2()
        {
            string theProfileName = "something";

            IContainer container = new Container(registry =>
            {
                registry.Profile(theProfileName, x =>
                {
                    x.Type<IWidget>().Is.ConstructedBy(() => new AWidget());
                    x.Type<Rule>().Is.ConstructedBy(() => new DefaultRule());
                });
            });

            container.SetDefaultsToProfile(theProfileName);

            container.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
            container.GetInstance<Rule>().ShouldBeOfType<Rule>();
        }


        [Test]
        public void Add_default_instance_with_concrete_type()
        {
            string theProfileName = "something";

            IContainer container = new Container(registry =>
            {
                registry.Profile(theProfileName, p =>
                {
                    p.For<IWidget>().UseConcreteType<AWidget>();
                    p.For<Rule>().UseConcreteType<DefaultRule>();
                });

            });
            container.SetDefaultsToProfile(theProfileName);

            container.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
            container.GetInstance<Rule>().ShouldBeOfType<DefaultRule>();
        }

        [Test]
        public void Add_default_instance_with_literal()
        {
            var registry = new Registry();
            var theWidget = new AWidget();

            string theProfileName = "something";
            registry.Profile(theProfileName)
                .For<IWidget>().Use(theWidget);

            PluginGraph graph = registry.Build();
            var instance = (ObjectInstance) graph.ProfileManager.GetDefault(typeof (IWidget), "something");

            Assert.AreSame(theWidget, instance.Object);
        }

        [Test]
        public void AddAProfileWithANamedDefault()
        {
            string theProfileName = "TheProfile";
            string theDefaultName = "TheDefaultName";

            var registry = new Registry();
            registry.Profile(theProfileName)
                .For<IWidget>().UseNamedInstance(theDefaultName)
                .For<Rule>().UseNamedInstance("DefaultRule");

            ObjectInstance masterInstance =
                registry.For<IWidget>().Add(new AWidget()).WithName(theDefaultName);

            ProfileManager manager = registry.Build().ProfileManager;
            Assert.AreSame(masterInstance, manager.GetDefault(typeof (IWidget), theProfileName));
        }

        [Test]
        public void AddAProfileWithInlineInstanceDefinition()
        {
            string theProfileName = "TheProfile";

            var registry = new Registry();
            registry.Profile(theProfileName)
                .For<IWidget>().UseConcreteType<AWidget>();

            PluginGraph graph = registry.Build();

            ProfileManager profileManager = graph.ProfileManager;
            Instance defaultInstance = profileManager.GetDefault(typeof (IWidget), theProfileName);

            Assert.AreEqual(StructureMap.Pipeline.Profile.InstanceKeyForProfile(theProfileName), defaultInstance.Name);

            var manager = new Container(graph);
            manager.SetDefaultsToProfile(theProfileName);
            var widget = (AWidget) manager.GetInstance<IWidget>();
            Assert.IsNotNull(widget);
        }
    }
}