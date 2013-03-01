using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Testing;

namespace StructureMap.AutoMocking.Testing
{
    [TestFixture]
    public class RhinoMockRepositoryProxyTester
    {
        [Test]
        public void can_make_dynamic_mocks()
        {
            var mockRepository = new RhinoMockRepositoryProxy();
            object fooMock = mockRepository.DynamicMock(typeof (ITestMocks));

            fooMock.ShouldNotBeNull();
        }

        [Test]
        public void can_make_partial_mocks()
        {
            var mockRepository = new RhinoMockRepositoryProxy();
            var testPartials = (TestPartials) mockRepository.PartialMock(typeof (TestPartials), new object[0]);

            testPartials.ShouldNotBeNull();
            mockRepository.Replay(testPartials);
            testPartials.Concrete().ShouldEqual("Concrete");
            testPartials.Virtual().ShouldEqual("Virtual");

            testPartials.Stub(t => t.Virtual()).Return("MOCKED!");
            testPartials.Virtual().ShouldEqual("MOCKED!");
        }

        [Test]
        public void can_put_mock_in_replay_mode()
        {
            var mockRepository = new RhinoMockRepositoryProxy();
            var test = (ITestMocks) mockRepository.DynamicMock(typeof (ITestMocks));

            mockRepository.Replay(test);

            test.Stub(t => t.Answer()).Return("YES");
            test.ShouldNotBeNull();
            test.Answer().ShouldEqual("YES");
        }
    }

    public interface ITestMocks
    {
        string Answer();
    }

    public class TestPartials
    {
        public string Concrete()
        {
            return "Concrete";
        }

        public virtual string Virtual()
        {
            return "Virtual";
        }
    }
}