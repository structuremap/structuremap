using System;
using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class Lazy_and_Func_construction_strategy_Tester
    {
        [Test]
        public void FactoryTemplateTester()
        {
            var container =
                new Container();

            container.GetInstance<Func<ConcreteClass>>()().ShouldNotBeNull();
        }

        [Test]
        public void can_build_a_Lazy_of_T_automatically()
        {
            new Container().GetInstance<Lazy<ConcreteClass>>()
                .Value.ShouldBeOfType<ConcreteClass>().ShouldNotBeNull();
        }

        [Test]
        public void build_a_func_for_a_concrete_class()
        {
            var container = new Container();
            var func = container.GetInstance<Func<ConcreteClass>>();

            func().ShouldNotBeNull();
        }

        [Test]
        public void build_a_func_that_returns_a_transient()
        {
            var container =
                new Container(x => x.For<IWidget>().Use<ColorWidget>().Ctor<string>("color").Is("green"));

            var func = container.GetInstance<Func<IWidget>>();
            var w1 = func();
            var w2 = func();
            var w3 = func();

            w1.ShouldBeOfType<ColorWidget>().Color.ShouldEqual("green");

            w1.ShouldNotBeTheSameAs(w2);
            w1.ShouldNotBeTheSameAs(w3);
            w2.ShouldNotBeTheSameAs(w3);
        }

        [Test]
        public void build_a_func_that_returns_a_singleton()
        {
            var container =
                new Container(x => { x.ForSingletonOf<IWidget>().Use<ColorWidget>().Ctor<string>("color").Is("green"); });

            var func = container.GetInstance<Func<IWidget>>();
            var w1 = func();
            var w2 = func();
            var w3 = func();

            w1.ShouldBeOfType<ColorWidget>().Color.ShouldEqual("green");

            w1.ShouldBeTheSameAs(w2);
            w1.ShouldBeTheSameAs(w3);
            w2.ShouldBeTheSameAs(w3);
        }

        [Test]
        public void build_a_func_by_string()
        {
            var container = new Container(x => {
                x.For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("green").Named("green");
                x.For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("blue").Named("blue");
                x.For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("red").Named("red");
            });

            var func = container.GetInstance<Func<string, IWidget>>();
            func("green").ShouldBeOfType<ColorWidget>().Color.ShouldEqual("green");
        }

        public class ConcreteClass
        {
        }
    }
}