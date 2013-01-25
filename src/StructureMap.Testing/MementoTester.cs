using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class MementoTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public class TheGateway : IGateway
        {
            #region IGateway Members

            public string WhoAmI { get { throw new NotImplementedException(); } }

            public void DoSomething()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [Test]
        public void If_a_Memento_does_not_know_its_TPluggedType_or_concreteKey_select_the_DEFAULT_Plugin()
        {
            var family = new PluginFamily(typeof (IGateway));
            Plugin plugin = family.AddPlugin(typeof (TheGateway), Plugin.DEFAULT);

            var memento = new MemoryInstanceMemento();
            Assert.AreSame(plugin, memento.FindPlugin(family));
        }
    }
}