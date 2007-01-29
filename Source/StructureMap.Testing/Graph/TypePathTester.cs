using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class TypePathTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void CanFindType()
        {
            TypePath path = new TypePath(GetType().Assembly.GetName().Name, GetType().FullName);
            Assert.IsTrue(path.CanFindType());
            Assert.IsNotNull(path.FindType());
        }

        [Test]
        public void CanBuildTypeCreatedFromType()
        {
            TypePath path = new TypePath(GetType());
            path.FindType();
        }
    }
}