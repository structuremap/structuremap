using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class PluggableAttributeTester
    {
        public PluggableAttributeTester()
        {
        }


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