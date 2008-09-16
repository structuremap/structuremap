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

        public interface IDefault
        {
        }

        public class DefaultClass : IDefault
        {
        }

        [Test]
        public void Build_happy_path()
        {
            MockRepository mocks = new MockRepository();
            BuildSession buildSession =
                mocks.StrictMock<BuildSession>();

            DefaultClass theDefault = new DefaultClass();


            using (mocks.Record())
            {
                Expect.Call(buildSession.CreateInstance(typeof (IDefault))).Return(theDefault);
            }

            using (mocks.Playback())
            {
                DefaultInstance instance = new DefaultInstance();
                Assert.AreSame(theDefault, instance.Build(typeof (IDefault), buildSession));
            }
        }

        [Test]
        public void Get_description()
        {
            TestUtility.AssertDescriptionIs(new DefaultInstance(), "Default");
        }
    }
}