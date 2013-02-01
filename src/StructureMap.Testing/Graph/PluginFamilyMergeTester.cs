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
            var source = new PluginFamily(typeof (IWidget));
            var sourceInstance = new ObjectInstance(new AWidget());
            source.AddInstance(sourceInstance);

            var destination = new PluginFamily(typeof (IWidget));
            destination.ImportFrom(source);

            Assert.AreSame(sourceInstance, destination.GetInstance(sourceInstance.Name));
        }

        [Test]
        public void Do_not_override_named_instance()
        {
            var source = new PluginFamily(typeof (IWidget));
            ObjectInstance sourceInstance = new ObjectInstance(new AWidget()).Named("New");
            source.AddInstance(sourceInstance);

            var destination = new PluginFamily(typeof (IWidget));
            ObjectInstance destinationInstance = new ObjectInstance(new AWidget()).Named("New");
            destination.AddInstance(destinationInstance);

            destination.ImportFrom(source);

            Assert.AreSame(destinationInstance, destination.GetInstance(sourceInstance.Name));
        }


    }
}