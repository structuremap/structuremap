using System;
using Moq;
using NUnit.Framework;
using StructureMap.AutoMocking;
using System.Linq.Expressions;

namespace StructureMap.Testing.AutoMocking
{
    [TestFixture]
    public class example_MoqAutoMocker_usage
    {
        [Test]
        public void verify_an_expected_calls()
        {
            MoqAutoMocker<AutoMockerTester.ConcreteClass> autoMocker = new MoqAutoMocker<AutoMockerTester.ConcreteClass>();
            AutoMockerTester.IMockedService mockedService = autoMocker.Get<AutoMockerTester.IMockedService>();
            autoMocker.ClassUnderTest.CallService();

            IMock<AutoMockerTester.IMockedService> mockedServiceWrapper = Mock.Get(mockedService);
            mockedServiceWrapper.Verify(x => x.Go());
        }
    }

    [TestFixture]
    public class MoqAutoMockerTester : AutoMockerTester
    {
        protected override AutoMocker<T> createAutoMocker<T>()
        {
            return new MoqAutoMocker<T>();
        }

        protected override void setExpectation<T, TResult>(T mock, Expression<Func<T, TResult>> functionCall, TResult expectedResult)
        {
            Mock.Get(mock).Expect(functionCall).Returns(expectedResult);
        }
    }
}
