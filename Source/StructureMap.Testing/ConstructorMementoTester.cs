using NUnit.Framework;
using Rhino.Mocks;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ConstructorMementoTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Construct()
        {
            ConstructorMemento<string> memento = new ConstructorMemento<string>("A", delegate { return "Hello"; });
            Assert.AreEqual("A", memento.InstanceKey);
            string actual = (string) memento.Build(null);
            Assert.AreEqual("Hello", actual);
        }
    }
}
