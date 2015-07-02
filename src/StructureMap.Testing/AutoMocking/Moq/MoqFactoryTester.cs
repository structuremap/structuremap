using Moq;
using NUnit.Framework;
using Shouldly;
using StructureMap.AutoMocking.Moq;

namespace StructureMap.Testing.AutoMocking.Moq
{
    [TestFixture]
    public class MoqFactoryTester
    {
        [Test]
        public void can_make_dynamic_mocks()
        {
            var moqFactory = new MoqFactory();
            var fooMock = moqFactory.CreateMock(typeof (ITestMocks));

            fooMock.ShouldNotBeNull();
        }

        [Test]
        public void can_make_partial_mocks()
        {
            var moqFactory = new MoqFactory();
            var testPartials = (TestPartials) moqFactory.CreateMockThatCallsBase(typeof (TestPartials), new object[0]);

            testPartials.ShouldNotBeNull();
            testPartials.Concrete().ShouldBe("Concrete");
            testPartials.Virtual().ShouldBe("Virtual");

            var mock = Mock.Get(testPartials);
            mock.Setup(t => t.Virtual()).Returns("MOQed!");
            testPartials.Virtual().ShouldBe("MOQed!");
        }

        [Test]
        public void sample_moq_usage()
        {
            var mock = new Mock<ITestMocks>();
            mock.Setup(t => t.Answer()).Returns("Moq");
            mock.Object.Answer().ShouldBe("Moq");
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