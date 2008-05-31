using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing
{
    [TestFixture]
    public class InstanceBuilderListTester
    {
        [Test]
        public void Do_not_add_a_new_InstanceBuilder_for_the_same_plugged_type_but_different_concrete_key()
        {
            InstanceBuilderList list = new InstanceBuilderList(typeof (IWidget));
            Plugin plugin = new Plugin(typeof (AWidget));
            list.Add(plugin);

            Plugin plugin2 = new Plugin(typeof (AWidget), "DifferentKey");
            list.Add(plugin2);

            Assert.AreEqual(1, list.BuilderCount);
        }

        [Test]
        public void Find_the_InstanceBuilder_by_concrete_key_with_different_aliased_concrete_keys()
        {
            InstanceBuilderList list = new InstanceBuilderList(typeof (IWidget));
            Plugin plugin = new Plugin(typeof (AWidget));
            list.Add(plugin);

            Plugin plugin2 = new Plugin(typeof (AWidget), "DifferentKey");
            list.Add(plugin2);

            InstanceBuilder builder1 = list.FindByConcreteKey(plugin.ConcreteKey);
            InstanceBuilder builder2 = list.FindByConcreteKey(plugin2.ConcreteKey);

            Assert.AreSame(builder1, builder2);
        }

        [Test]
        public void InstanceBuilderList_add_a_new_InstanceBuilder_if_the_new_Plugin_is_not_recognized()
        {
            InstanceBuilderList list = new InstanceBuilderList(typeof (IWidget));
            Assert.AreEqual(0, list.BuilderCount);

            Plugin plugin = new Plugin(typeof (AWidget));
            list.Add(plugin);

            Assert.AreEqual(1, list.BuilderCount);
            InstanceBuilder builder = list.FindByType(typeof (AWidget));

            Assert.IsNotNull(builder);
        }

        [Test]
        public void InstanceBuilderList_should_not_add_a_new_InstanceBuilder_if_the_same_one_exists()
        {
            InstanceBuilderList list = new InstanceBuilderList(typeof (IWidget));
            Plugin plugin = new Plugin(typeof (AWidget));
            list.Add(plugin);
            list.Add(plugin);

            Assert.AreEqual(1, list.BuilderCount);
        }
    }
}