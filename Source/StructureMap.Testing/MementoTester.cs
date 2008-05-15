using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Graph;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class MementoTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void If_a_Memento_does_not_know_its_PluggedType_or_concreteKey_select_the_DEFAULT_Plugin()
        {
            PluginFamily family = new PluginFamily(typeof(IGateway));
            Plugin plugin = family.AddPlugin(typeof(TheGateway), Plugin.DEFAULT);

            MemoryInstanceMemento memento = new MemoryInstanceMemento();
            Assert.AreSame(plugin, memento.FindPlugin(family));
        }

        public class TheGateway : IGateway
        {
            public string WhoAmI
            {
                get { throw new System.NotImplementedException(); }
            }

            public void DoSomething()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
