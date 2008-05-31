using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class TypePathTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void CanBuildTypeCreatedFromType()
        {
            TypePath path = new TypePath(GetType());
            path.FindType();
        }
    }
}