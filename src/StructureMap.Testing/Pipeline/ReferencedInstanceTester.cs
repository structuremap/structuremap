using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Graph;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ReferencedInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public interface IReferenced
        {
        }

        public class ConcreteReferenced : IReferenced
        {
        }


        [Test]
        public void GetDescription()
        {
            string theReferenceKey = "theReferenceKey";
            var instance = new ReferencedInstance(theReferenceKey);

            TestUtility.AssertDescriptionIs(instance, "\"theReferenceKey\"");
        }
    }
}