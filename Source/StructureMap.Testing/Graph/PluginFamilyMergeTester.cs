using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class PluginFamilyMergeTester
    {
        [Test]
        public void Add_instance_that_does_not_exist_in_destination()
        {
            PluginFamily source = new PluginFamily(typeof (IWidget));
            LiteralInstance sourceInstance = new LiteralInstance(new AWidget());
            source.AddInstance(sourceInstance);

            PluginFamily destination = new PluginFamily(typeof (IWidget));
            destination.ImportFrom(source);

            Assert.AreSame(sourceInstance, destination.GetInstance(sourceInstance.Name));
        }

        [Test]
        public void Do_not_override_named_instance()
        {
            PluginFamily source = new PluginFamily(typeof (IWidget));
            LiteralInstance sourceInstance = new LiteralInstance(new AWidget()).WithName("New");
            source.AddInstance(sourceInstance);

            PluginFamily destination = new PluginFamily(typeof (IWidget));
            LiteralInstance destinationInstance = new LiteralInstance(new AWidget()).WithName("New");
            destination.AddInstance(destinationInstance);

            destination.ImportFrom(source);

            Assert.AreSame(destinationInstance, destination.GetInstance(sourceInstance.Name));
        }

        [Test]
        public void Do_not_overwrite_existing_plugin()
        {
            PluginFamily source = new PluginFamily(typeof (IWidget));
            source.AddPlugin(typeof (AWidget));

            PluginFamily destination = new PluginFamily(typeof (IWidget));
            Plugin destinationPlugin = destination.AddPlugin(typeof (AWidget));
            destination.ImportFrom(source);

            Assert.AreSame(destinationPlugin, destination.Plugins[typeof (AWidget)]);
        }

        [Test]
        public void Merge_missing_Plugin()
        {
            PluginFamily source = new PluginFamily(typeof (IWidget));
            source.AddPlugin(typeof (AWidget));

            PluginFamily destination = new PluginFamily(typeof (IWidget));
            destination.ImportFrom(source);

            destination.ImportFrom(source);

            Assert.IsTrue(destination.HasPlugin(typeof (AWidget)));
        }
    }
}