using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Source;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class NormalGraphBuilderTester
    {
        [Test]
        public void Call_the_action_on_configure_family_if_the_pluginType_is_found()
        {
            var typePath = new TypePath(typeof (IGateway));

            bool iWasCalled = false;
            var builder = new GraphBuilder(new PluginGraph());
            builder.ConfigureFamily(typePath, f =>
            {
                Assert.AreEqual(typeof (IGateway), f.PluginType);
                iWasCalled = true;
            });


            Assert.IsTrue(iWasCalled);
        }

        [Test]
        public void Configure_a_family_that_does_not_exist_and_log_an_error_with_PluginGraph()
        {
            var builder = new GraphBuilder(new PluginGraph());
            builder.ConfigureFamily(new TypePath("a,a"), delegate { });

            builder.PluginGraph.Log.AssertHasError(103);
        }

        [Test]
        public void Do_not_call_the_action_on_ConfigureFamily_if_the_type_path_blows_up()
        {
            var builder = new GraphBuilder(new PluginGraph());
            builder.ConfigureFamily(new TypePath("a,a"), obj => Assert.Fail("Should not be called"));
        }

    }
}