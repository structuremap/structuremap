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
        public void FindMaster_Instance_happy_path()
        {
            var family = new PluginFamily(typeof (ISomething));
            ObjectInstance redInstance = new ObjectInstance(new SomethingOne()).Named("Red");
            family.AddInstance(redInstance);
            family.AddInstance(new ObjectInstance(new SomethingOne()).Named("Blue"));

            var instance = new ReferencedInstance("Red");
            Assert.AreSame(redInstance, ((IDiagnosticInstance) instance).FindInstanceForProfile(family, null, null));
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