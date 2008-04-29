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

        [Test]
        public void CanFindType()
        {
            TypePath path = new TypePath(GetType().Assembly.GetName().Name, GetType().FullName);
            Assert.IsTrue(path.CanFindType());
            Assert.IsNotNull(path.FindType());
        }

        [Test]
        public void Define_with_out_assembly_qualified_name_throws_exception()
        {
            try
            {
                new TypePath(this.GetType().FullName);
                Assert.Fail("Did not throw exception");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(107, ex.ErrorCode);
            }
        }
    }
}