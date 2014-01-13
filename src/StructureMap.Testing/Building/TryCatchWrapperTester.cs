using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using NUnit.Framework;
using StructureMap.Building;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class TryCatchWrapperTester
    {
        private ConstantExpression good;
        private BlockExpression throwGeneral;
        private BlockExpression throwSM;
        private StructureMapException smEx;
        private NotImplementedException genericEx;

        [SetUp]
        public void SetUp()
        {
            good = Expression.Constant("I am good");

            genericEx = new NotImplementedException();
            throwGeneral = Expression.Block(Expression.Throw(Expression.Constant(genericEx)),
                Expression.Constant("bar"));

            smEx = new StructureMapException("you stink!");
            throwSM = Expression.Block(Expression.Throw(Expression.Constant(smEx)),
                Expression.Constant("bar"));
        }

        [Test]
        public void wrap_against_a_successful_expression_by_IDescribed()
        {
            var wrapped = TryCatchWrapper.Wrap<FakeStructureMapException>(typeof (string), good,
                new StubbedDescribed("my description"));


            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            func().ShouldEqual("I am good");
        }

        [Test]
        public void wrap_against_a_failed_generic_exception_by_IDescribed()
        {
            var wrapped = TryCatchWrapper.Wrap<FakeStructureMapException>(typeof(string), throwGeneral,
                new StubbedDescribed("my description"));


            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<FakeStructureMapException>.ShouldBeThrownBy(() => func());

            ex.Title.ShouldEqual("my description");
            ex.InnerException.ShouldBeTheSameAs(genericEx);
        }

        [Test]
        public void wrap_against_a_failed_StructureMapException_by_IDescribed()
        {
            var wrapped = TryCatchWrapper.Wrap<FakeStructureMapException>(typeof (string), throwSM,
                new StubbedDescribed("my description"));


            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<StructureMapException>.ShouldBeThrownBy(() => func());

            ex.ShouldBeOfType<StructureMapException>();
            ex.Message.ShouldContain("my description");
            ex.ShouldBeTheSameAs(smEx);
        }

        [Test]
        public void wrap_against_a_successful_expression_by_expression_description()
        {
            var wrapped = TryCatchWrapper.Wrap<FakeStructureMapException>(typeof(string), good,
                () => "some description");


            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            func().ShouldEqual("I am good");
        }

        [Test]
        public void wrap_against_a_failed_generic_exception_by_expression_description()
        {
            var wrapped = TryCatchWrapper.Wrap<FakeStructureMapException>(typeof(string), throwGeneral,
                () => "some description");


            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<FakeStructureMapException>.ShouldBeThrownBy(() => func());

            ex.Title.ShouldEqual("some description");
            ex.InnerException.ShouldBeTheSameAs(genericEx);
        }

        [Test]
        public void wrap_against_a_failed_StructureMapException_by_expression_description()
        {
            var wrapped = TryCatchWrapper.Wrap<FakeStructureMapException>(typeof(string), throwSM,
                () => "some description");


            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<StructureMapException>.ShouldBeThrownBy(() => func());

            ex.Message.ShouldContain("some description");
            ex.ShouldBeTheSameAs(smEx);
        }

        [Test]
        public void wrap_against_a_successful_expression_by_string_description()
        {
            var wrapped = TryCatchWrapper.Wrap<FakeStructureMapException>(typeof(string), good,
                "some description");


            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            func().ShouldEqual("I am good");
        }

        [Test]
        public void wrap_against_a_failed_generic_exception_by_string_description()
        {
            var wrapped = TryCatchWrapper.Wrap<FakeStructureMapException>(typeof(string), throwGeneral,
                "some description");


            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<FakeStructureMapException>.ShouldBeThrownBy(() => func());

            ex.Title.ShouldEqual("some description");
            ex.InnerException.ShouldBeTheSameAs(genericEx);
        }

        [Test]
        public void wrap_against_a_failed_StructureMapException_by_string_description()
        {
            var wrapped = TryCatchWrapper.Wrap<FakeStructureMapException>(typeof(string), throwSM,
               "some description");


            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<StructureMapException>.ShouldBeThrownBy(() => func());

            ex.Message.ShouldContain("some description");
            ex.ShouldBeTheSameAs(smEx);
        }
    }

    public class StubbedDescribed : IDescribed
    {
        public StubbedDescribed(string description)
        {
            Description = description;
        }

        public string Description { get; private set; }
    }

    public class FakeStructureMapException : StructureMapException
    {
        protected FakeStructureMapException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FakeStructureMapException(string message) : base(message)
        {
        }

        public FakeStructureMapException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FakeStructureMapException(int ErrorCode, params object[] args) : base(ErrorCode, args)
        {
        }

        public FakeStructureMapException(int ErrorCode, Exception InnerException, params object[] args) : base(ErrorCode, InnerException, args)
        {
        }
    }
}