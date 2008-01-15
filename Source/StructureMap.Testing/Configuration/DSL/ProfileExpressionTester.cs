using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
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
            ((IExpression) expression).Configure(graph);

            Profile profile = graph.DefaultManager.GetProfile(theProfileName);
            Assert.IsNotNull(profile);
            Assert.AreEqual(new InstanceDefault[] {new InstanceDefault(typeof (IWidget), theDefaultName)},
                            profile.Defaults);
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


            Profile profile = manager.DefaultManager.GetProfile(theProfileName);
            InstanceDefault instanceDefault = profile.Defaults[0];
            Assert.AreEqual(Profile.InstanceKeyForProfile(theProfileName), instanceDefault.DefaultKey);

            manager.SetDefaultsToProfile(theProfileName);
            AWidget widget = (AWidget) manager.CreateInstance<IWidget>();
            Assert.IsNotNull(widget);
        }
    }
}