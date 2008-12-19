using System;
using Moq;
using NUnit.Framework;
using StructureMap.AutoMocking;
using System.Linq.Expressions;

namespace StructureMap.Testing.AutoMocking
{
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
