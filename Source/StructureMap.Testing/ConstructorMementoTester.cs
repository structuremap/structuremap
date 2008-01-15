using NUnit.Framework;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ConstructorMementoTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

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