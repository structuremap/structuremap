using Shouldly;
using StructureMap.Building;
using StructureMap.Testing.Widget3;
using System;
using System.Linq.Expressions;
using Xunit;

namespace StructureMap.Testing.Building
{
    public class TryCatchWrapperTester
    {
        private ConstantExpression good;
        private BlockExpression throwGeneral;
        private BlockExpression throwSM;
        private StructureMapException smEx;
        private NotImplementedException genericEx;
        private BlockExpression goodVoid;
        private BlockExpression badVoid;
        private BlockExpression badSmVoid;

        public TryCatchWrapperTester()
        {
            good = Expression.Constant("I am good");

            genericEx = new NotImplementedException();
            throwGeneral = Expression.Block(Expression.Throw(Expression.Constant(genericEx)),
                Expression.Constant("bar"));

            smEx = new StructureMapException("you stink!");
            throwSM = Expression.Block(Expression.Throw(Expression.Constant(smEx)),
                Expression.Constant("bar"));

            var gateway = new StubbedGateway();

            Expression<Action> goodExpr = () => gateway.DoSomething();

            goodVoid = Expression.Block(goodExpr.Body);
            badVoid = Expression.Block(Expression.Throw(Expression.Constant(genericEx)));
            badSmVoid = Expression.Block(Expression.Throw(Expression.Constant(smEx)));
        }

        [Fact]
        public void no_return_happy_type()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", null, goodVoid, "okay");
            var action = Expression.Lambda<Action>(wrapped).Compile();

            action();
        }

        [Fact]
        public void no_return_sad_path_with_generic_exception()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", null, badVoid, "okay");
            var action = Expression.Lambda<Action>(wrapped).Compile();

            Exception<FakeStructureMapException>.ShouldBeThrownBy(() => { action(); })
                .InnerException.ShouldBeTheSameAs(genericEx);
        }

        [Fact]
        public void no_return_sad_path_with_structuremapexception()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", null, badSmVoid, "okay");
            var action = Expression.Lambda<Action>(wrapped).Compile();

            Exception<StructureMapException>.ShouldBeThrownBy(() => { action(); }).ShouldBeTheSameAs(smEx);
        }

        [Fact]
        public void wrap_against_a_successful_expression_by_IDescribed()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", typeof(string), good,
                new StubbedDescribed("my description"));

            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            func().ShouldBe("I am good");
        }

        [Fact]
        public void wrap_against_a_failed_generic_exception_by_IDescribed()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", typeof(string), throwGeneral,
                new StubbedDescribed("my description"));

            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<FakeStructureMapException>.ShouldBeThrownBy(() => func());

            ex.Title.ShouldBe("bad!");
            ex.Message.ShouldContain("my description");
            ex.InnerException.ShouldBeTheSameAs(genericEx);
        }

        [Fact]
        public void wrap_against_a_failed_StructureMapException_by_IDescribed()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", typeof(string), throwSM,
                new StubbedDescribed("my description"));

            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<StructureMapException>.ShouldBeThrownBy(() => func());

            ex.ShouldBeOfType<StructureMapException>();
            ex.Message.ShouldContain("my description");
            ex.ShouldBeTheSameAs(smEx);
        }

        [Fact]
        public void wrap_against_a_successful_expression_by_expression_description()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", typeof(string), good,
                () => "some description");

            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            func().ShouldBe("I am good");
        }

        [Fact]
        public void wrap_against_a_failed_generic_exception_by_expression_description()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", typeof(string), throwGeneral,
                () => "some description");

            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<FakeStructureMapException>.ShouldBeThrownBy(() => func());

            ex.Title.ShouldBe("bad!");
            ex.Message.ShouldContain("some description");
            ex.InnerException.ShouldBeTheSameAs(genericEx);
        }

        [Fact]
        public void wrap_against_a_failed_StructureMapException_by_expression_description()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", typeof(string), throwSM,
                () => "some description");

            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<StructureMapException>.ShouldBeThrownBy(() => func());

            ex.Message.ShouldContain("some description");
            ex.ShouldBeTheSameAs(smEx);
        }

        [Fact]
        public void wrap_against_a_successful_expression_by_string_description()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", typeof(string), good,
                "some description");

            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            func().ShouldBe("I am good");
        }

        [Fact]
        public void wrap_against_a_failed_generic_exception_by_string_description()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", typeof(string), throwGeneral,
                "some description");

            var func = Expression.Lambda<Func<string>>(wrapped).Compile();

            var ex = Exception<FakeStructureMapException>.ShouldBeThrownBy(() => func());

            ex.Title.ShouldBe("bad!");
            ex.Message.ShouldContain("some description");
            ex.InnerException.ShouldBeTheSameAs(genericEx);
        }

        [Fact]
        public void wrap_against_a_failed_StructureMapException_by_string_description()
        {
            var wrapped = TryCatchWrapper.WrapFunc<FakeStructureMapException>("bad!", typeof(string), throwSM,
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
        public FakeStructureMapException(string message) : base(message)
        {
        }

        public FakeStructureMapException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}