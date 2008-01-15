using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class CreateProfileTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            /*
            pluginGraph = new PluginGraph();
            Registry registry = new Registry(pluginGraph);
            registry.ScanAssemblies().IncludeAssemblyContainingType<IWidget>();
            registry.AddInstanceOf<IWidget>
                .Called("DarkGreen").OfConcreteType<ColorWidget>.WithProperty("Color").EqualTo("DarkGreen");



            registry.CreateProfile("Green")
                .UseConcreteType<SimpleThing<string>>().For<ISimpleThing<string>>()

                .UseInstanceNamed("Green").For<Rule>();

            manager = registry.BuildInstanceManager();
             */
        }

        #endregion

        private InstanceManager manager;
        private PluginGraph pluginGraph;

        [Test]
        public void CreateProfile()
        {
        }
    }
}