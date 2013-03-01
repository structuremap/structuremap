using Moq;
using NUnit.Framework;
using StructureMap.Testing;

namespace StructureMap.AutoMocking.Moq.Testing
{
    [TestFixture]
    public class MoqFactoryTester
    {
        [Test]
        public void can_make_dynamic_mocks()
        {
            var moqFactory = new MoqFactory();
            object fooMock = moqFactory.CreateMock(typeof (ITestMocks));

            fooMock.ShouldNotBeNull();
        }

        [Test]
        public void can_make_partial_mocks()
        {
            var moqFactory = new MoqFactory();
            var testPartials = (TestPartials) moqFactory.CreateMockThatCallsBase(typeof (TestPartials), new object[0]);

            testPartials.ShouldNotBeNull();
            testPartials.Concrete().ShouldEqual("Concrete");
            testPartials.Virtual().ShouldEqual("Virtual");

            var mock = Mock.Get(testPartials);
            mock.Expect(t => t.Virtual()).Returns("MOQed!");
            testPartials.Virtual().ShouldEqual("MOQed!");
        }

        [Test]
        public void sample_moq_usage()
        {
            var mock = new Mock<ITestMocks>();
            mock.Expect(t => t.Answer()).Returns("Moq");
            mock.Object.Answer().ShouldEqual("Moq");
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