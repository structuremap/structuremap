using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class DefaultInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void Build_happy_path()
        {
            MockRepository mocks = new MockRepository();
            StructureMap.Pipeline.IInstanceCreator instanceCreator =
                mocks.CreateMock<StructureMap.Pipeline.IInstanceCreator>();

            DefaultClass theDefault = new DefaultClass();


            using (mocks.Record())
            {
                Expect.Call(instanceCreator.CreateInstance(typeof(IDefault))).Return(theDefault);
            }

            using (mocks.Playback())
            {
                DefaultInstance instance = new DefaultInstance();
                Assert.AreSame(theDefault, instance.Build(typeof(IDefault), instanceCreator));
            }
        }

        public interface IDefault {}
        public class DefaultClass : IDefault {}
    }
}