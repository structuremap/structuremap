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
            var mocks = new MockRepository();
            var buildSession =
                mocks.StrictMock<BuildSession>();

            var theDefault = new DefaultClass();


            using (mocks.Record())
            {
                Expect.Call(buildSession.CreateInstance(typeof (IDefault))).Return(theDefault);
            }

            using (mocks.Playback())
            {
                var instance = new DefaultInstance();
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