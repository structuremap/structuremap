using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class PluggableAttributeTester
    {
        [Test]
        public void MarkedAsPluggable()
        {
            Assert.AreEqual(true, PluggableAttribute.MarkedAsPluggable(typeof (ColorWidget)), "ColorWidget is marked");
        }

        [Test]
        public void NotMarkedAsPluggable()
        {
            Assert.AreEqual(false, PluggableAttribute.MarkedAsPluggable(typeof (NotPluggableWidget)),
                            "ColorWidget is marked");
        }
    }
}