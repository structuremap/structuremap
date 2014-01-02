using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

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
            var theReferenceKey = "theReferenceKey";
            var instance = new ReferencedInstance(theReferenceKey);

            TestUtility.AssertDescriptionIs(instance, "\"theReferenceKey\"");
        }

        [Test]
        public void to_dependency_source()
        {
            var theReferenceKey = "theReferenceKey";
            var instance = new ReferencedInstance(theReferenceKey);

            instance.ToDependencySource(typeof (IGateway))
                .ShouldEqual(new ReferencedDependencySource(typeof (IGateway), theReferenceKey));
        }
    }
}