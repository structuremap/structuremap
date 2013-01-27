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
        public void Create_referenced_instance_happy_path()
        {
            var mocks = new MockRepository();
            var buildSession = mocks.StrictMock<BuildSession>();

            var returnedValue = new ConcreteReferenced();
            string theReferenceKey = "theReferenceKey";
            var instance = new ReferencedInstance(theReferenceKey);

            using (mocks.Record())
            {
                Expect.Call(buildSession.CreateInstance(typeof (IReferenced), theReferenceKey)).Return(returnedValue);
            }

            using (mocks.Playback())
            {
                Assert.AreSame(returnedValue, instance.Build(typeof (IReferenced), buildSession));
            }
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