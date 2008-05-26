using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
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
        public void AddAProfileWithANamedDefault()
        {
            string theProfileName = "TheProfile";
            string theDefaultName = "TheDefaultName";
            
            Registry registry = new Registry();
            registry.CreateProfile(theProfileName)
                .For<IWidget>().UseNamedInstance(theDefaultName)
                .For<Rule>().UseNamedInstance("DefaultRule");

            LiteralInstance masterInstance = new LiteralInstance(new AWidget()).WithName(theDefaultName);
            registry.AddInstanceOf<IWidget>(masterInstance);

            ProfileManager manager = registry.Build().ProfileManager;
            Assert.AreSame(masterInstance, manager.GetDefault(typeof(IWidget), theProfileName));
        }

        [Test]
        public void AddAProfileWithInlineInstanceDefinition()
        {
            string theProfileName = "TheProfile";

            PluginGraph graph = new PluginGraph();
            Registry registry = new Registry(graph);
            registry.CreateProfile(theProfileName)
                .For<IWidget>().Use(
                    Instance<IWidget>().UsingConcreteType<AWidget>()
                );

            IInstanceManager manager = registry.BuildInstanceManager();

            ProfileManager profileManager = graph.ProfileManager;
            Instance defaultInstance = profileManager.GetDefault(typeof(IWidget), theProfileName);

            Assert.AreEqual(Profile.InstanceKeyForProfile(theProfileName), defaultInstance.Name);

            manager.SetDefaultsToProfile(theProfileName);
            AWidget widget = (AWidget)manager.CreateInstance<IWidget>();
            Assert.IsNotNull(widget);
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
        public void Add_default_instance_with_concrete_type()
        {
            string theProfileName = "something";
            Registry registry = new Registry();
            registry.CreateProfile(theProfileName)
                .For<IWidget>().UseConcreteType<AWidget>()
                .For<Rule>().UseConcreteType<DefaultRule>();

            IInstanceManager manager = registry.BuildInstanceManager();
            manager.SetDefaultsToProfile(theProfileName);

            Assert.IsInstanceOfType(typeof(AWidget), manager.CreateInstance<IWidget>());
            Assert.IsInstanceOfType(typeof(DefaultRule), manager.CreateInstance<Rule>());
            
        }

        [Test]
        public void Add_default_instance_by_lambda()
        {
            string theProfileName = "something";
            Registry registry = new Registry();
            registry.CreateProfile(theProfileName)
                .For<IWidget>().Use(delegate() { return new AWidget(); })
                .For<Rule>().Use(delegate() { return new DefaultRule(); });

            IInstanceManager manager = registry.BuildInstanceManager();
            manager.SetDefaultsToProfile(theProfileName);

            Assert.IsInstanceOfType(typeof(AWidget), manager.CreateInstance<IWidget>());
            Assert.IsInstanceOfType(typeof(DefaultRule), manager.CreateInstance<Rule>());
        }

        [Test]
        public void Add_default_instance_by_prototype()
        {
            string theProfileName = "something";
            IWidget theTemplate = new AWidget();

            Registry registry = new Registry();
            registry.CreateProfile(theProfileName)
                .For<IWidget>().UsePrototypeOf(theTemplate);

            IInstanceManager manager = registry.BuildInstanceManager();
            manager.SetDefaultsToProfile(theProfileName);

            IWidget widget1 = manager.CreateInstance<IWidget>();
            IWidget widget2 = manager.CreateInstance<IWidget>();

            Assert.IsNotNull(widget1);
            Assert.IsNotNull(widget2);
            Assert.AreNotSame(widget1, widget2);
        }
    }
}