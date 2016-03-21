using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;
using Xunit;

namespace StructureMap.Testing.AutoMocking
{
    public class RhinoMockRepositoryProxyTester
    {
        [Fact]
        public void can_make_dynamic_mocks()
        {
            var mockRepository = new RhinoMockRepositoryProxy();
            var fooMock = mockRepository.DynamicMock(typeof(ITestMocks));

            fooMock.ShouldNotBeNull();
        }

        [Fact]
        public void can_make_partial_mocks()
        {
            var mockRepository = new RhinoMockRepositoryProxy();
            var testPartials = (TestPartials)mockRepository.PartialMock(typeof(TestPartials), new object[0]);

            testPartials.ShouldNotBeNull();
            mockRepository.Replay(testPartials);
            testPartials.Concrete().ShouldBe("Concrete");
            testPartials.Virtual().ShouldBe("Virtual");

            testPartials.Stub(t => t.Virtual()).Return("MOCKED!");
            testPartials.Virtual().ShouldBe("MOCKED!");
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