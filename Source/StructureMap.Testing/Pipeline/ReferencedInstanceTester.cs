using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ReferencedInstanceTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Create_referenced_instance_happy_path()
        {
            MockRepository mocks = new MockRepository();
            StructureMap.Pipeline.IInstanceCreator instanceCreator = mocks.CreateMock<StructureMap.Pipeline.IInstanceCreator>();

            ConcreteReferenced returnedValue = new ConcreteReferenced();
            string theReferenceKey = "theReferenceKey";
            ReferencedInstance instance = new ReferencedInstance(theReferenceKey);

            using (mocks.Record())
            {
                Expect.Call(instanceCreator.CreateInstance(typeof(IReferenced), theReferenceKey)).Return(returnedValue);
            }

            using (mocks.Playback())
            {
                Assert.AreSame(returnedValue, instance.Build(typeof(IReferenced), instanceCreator));
            }
        }

        public interface IReferenced
        {
            
        }

        public class ConcreteReferenced : IReferenced{}
    }
}
