using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using StructureMap.AutoMocking;

namespace StructureMap.Testing.AutoMocking
{
    [TestFixture]
    public class example_MoqAutoMocker_usage
    {
        [Test]
        public void verify_an_expected_calls()
        {
            var autoMocker = new MoqAutoMocker<AutoMockerTester.ConcreteClass>();
            var mockedService = autoMocker.Get<AutoMockerTester.IMockedService>();
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

        protected override void setExpectation<T, TResult>(T mock, Expression<Func<T, TResult>> functionCall,
                                                           TResult expectedResult)
        {
            Mock.Get(mock).Expect(functionCall).Returns(expectedResult);
        }
    }
}