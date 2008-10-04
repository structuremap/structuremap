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
        public void Add_default_instance_by_prototype()
        {
            string theProfileName = "something";
            IWidget theTemplate = new AWidget();

            IContainer manager = new Container(registry => registry.CreateProfile(theProfileName)
                                                               .For<IWidget>().UsePrototypeOf(theTemplate));

            manager.SetDefaultsToProfile(theProfileName);

            IWidget widget1 = manager.GetInstance<IWidget>();
            IWidget widget2 = manager.GetInstance<IWidget>();

            Assert.IsNotNull(widget1);
            Assert.IsNotNull(widget2);
            Assert.AreNotSame(widget1, widget2);
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
            Registry registry = new Registry();
            AWidget theWidget = new AWidget();

            string theProfileName = "something";
            registry.CreateProfile(theProfileName)
                .For<IWidget>().Use(theWidget);

            PluginGraph graph = registry.Build();
            LiteralInstance instance = (LiteralInstance) graph.ProfileManager.GetDefault(typeof (IWidget), "something");

            Assert.AreSame(theWidget, instance.Object);
        }

        [Test]
        public void AddAProfileWithANamedDefault()
        {
            string theProfileName = "TheProfile";
            string theDefaultName = "TheDefaultName";

            Registry registry = new Registry();
            registry.CreateProfile(theProfileName)
                .For<IWidget>().UseNamedInstance(theDefaultName)
                .For<Rule>().UseNamedInstance("DefaultRule");

            var masterInstance = registry.InstanceOf<IWidget>().Is.Object(new AWidget()).WithName(theDefaultName);

            ProfileManager manager = registry.Build().ProfileManager;
            Assert.AreSame(masterInstance, manager.GetDefault(typeof (IWidget), theProfileName));
        }

        [Test]
        public void AddAProfileWithInlineInstanceDefinition()
        {
            string theProfileName = "TheProfile";

            Registry registry = new Registry();
            registry.CreateProfile(theProfileName)
                .For<IWidget>().UseConcreteType<AWidget>();

            PluginGraph graph = registry.Build();

            ProfileManager profileManager = graph.ProfileManager;
            Instance defaultInstance = profileManager.GetDefault(typeof (IWidget), theProfileName);

            Assert.AreEqual(Profile.InstanceKeyForProfile(theProfileName), defaultInstance.Name);

            Container manager = new Container(graph);
            manager.SetDefaultsToProfile(theProfileName);
            AWidget widget = (AWidget) manager.GetInstance<IWidget>();
            Assert.IsNotNull(widget);
        }
    }
}