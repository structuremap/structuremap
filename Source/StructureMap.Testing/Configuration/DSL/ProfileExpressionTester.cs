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

            IContainer manager = new Container(registry => registry.CreateProfile(theProfileName)
                                                               .For<IWidget>().Use(delegate { return new AWidget(); })
                                                               .For<Rule>().Use(delegate { return new DefaultRule(); }));

            manager.SetDefaultsToProfile(theProfileName);

            Assert.IsInstanceOfType(typeof (AWidget), manager.GetInstance<IWidget>());
            Assert.IsInstanceOfType(typeof (DefaultRule), manager.GetInstance<Rule>());
        }


        [Test]
        public void Add_default_instance_by_lambda2()
        {
            string theProfileName = "something";

            IContainer manager = new Container(registry =>
            {
                registry.CreateProfile(theProfileName, x =>
                {
                    x.Type<IWidget>().Is.ConstructedBy(() => new AWidget());
                    x.Type<Rule>().Is.ConstructedBy(() => new DefaultRule());
                });
            });

            manager.SetDefaultsToProfile(theProfileName);

            Assert.IsInstanceOfType(typeof(AWidget), manager.GetInstance<IWidget>());
            Assert.IsInstanceOfType(typeof(DefaultRule), manager.GetInstance<Rule>());
        }



        [Test]
        public void Add_default_instance_with_concrete_type()
        {
            string theProfileName = "something";

            IContainer manager = new Container(registry => registry.CreateProfile(theProfileName)
                                                               .For<IWidget>().UseConcreteType<AWidget>()
                                                               .For<Rule>().UseConcreteType<DefaultRule>());
            manager.SetDefaultsToProfile(theProfileName);

            Assert.IsInstanceOfType(typeof (AWidget), manager.GetInstance<IWidget>());
            Assert.IsInstanceOfType(typeof (DefaultRule), manager.GetInstance<Rule>());
        }

        [Test]
        public void Add_default_instance_with_literal()
        {
            var registry = new Registry();
            var theWidget = new AWidget();

            string theProfileName = "something";
            registry.CreateProfile(theProfileName)
                .For<IWidget>().Use(theWidget);

            PluginGraph graph = registry.Build();
            var instance = (LiteralInstance) graph.ProfileManager.GetDefault(typeof (IWidget), "something");

            Assert.AreSame(theWidget, instance.Object);
        }

        [Test]
        public void AddAProfileWithANamedDefault()
        {
            string theProfileName = "TheProfile";
            string theDefaultName = "TheDefaultName";

            var registry = new Registry();
            registry.CreateProfile(theProfileName)
                .For<IWidget>().UseNamedInstance(theDefaultName)
                .For<Rule>().UseNamedInstance("DefaultRule");

            LiteralInstance masterInstance =
                registry.InstanceOf<IWidget>().Is.Object(new AWidget()).WithName(theDefaultName);

            ProfileManager manager = registry.Build().ProfileManager;
            Assert.AreSame(masterInstance, manager.GetDefault(typeof (IWidget), theProfileName));
        }

        [Test]
        public void AddAProfileWithInlineInstanceDefinition()
        {
            string theProfileName = "TheProfile";

            var registry = new Registry();
            registry.CreateProfile(theProfileName)
                .For<IWidget>().UseConcreteType<AWidget>();

            PluginGraph graph = registry.Build();

            ProfileManager profileManager = graph.ProfileManager;
            Instance defaultInstance = profileManager.GetDefault(typeof (IWidget), theProfileName);

            Assert.AreEqual(Profile.InstanceKeyForProfile(theProfileName), defaultInstance.Name);

            var manager = new Container(graph);
            manager.SetDefaultsToProfile(theProfileName);
            var widget = (AWidget) manager.GetInstance<IWidget>();
            Assert.IsNotNull(widget);
        }
    }
}