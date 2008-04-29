using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ProfileExpressionTester
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

            ProfileExpression expression = new ProfileExpression(theProfileName);

            ProfileExpression expression2 = expression.For<IWidget>().UseNamedInstance(theDefaultName);
            Assert.AreSame(expression, expression2);

            PluginGraph graph = new PluginGraph();
            ((IExpression)expression).Configure(graph);


            ProfileManager manager = graph.ProfileManager;
            ReferencedInstance defaultInstance = (ReferencedInstance) manager.GetDefault(typeof (IWidget), theProfileName);
            Assert.AreEqual(theDefaultName, defaultInstance.ReferenceKey);
        }

        [Test]
        public void AddAProfileWithInlineInstanceDefinition()
        {
            string theProfileName = "TheProfile";

            PluginGraph graph = new PluginGraph();
            Registry registry = new Registry(graph);
            registry.CreateProfile(theProfileName)
                .For<IWidget>().Use(
                Registry.Instance<IWidget>().UsingConcreteType<AWidget>()
                );

            IInstanceManager manager = registry.BuildInstanceManager();

            ProfileManager profileManager = graph.ProfileManager;
            Instance defaultInstance = profileManager.GetDefault(typeof(IWidget), theProfileName);

            Assert.AreEqual(Profile.InstanceKeyForProfile(theProfileName), defaultInstance.Name);

            manager.SetDefaultsToProfile(theProfileName);
            AWidget widget = (AWidget)manager.CreateInstance<IWidget>();
            Assert.IsNotNull(widget);
        }
    }
}