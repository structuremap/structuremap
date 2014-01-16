using System;
using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class LazyFuncTester
    {
        [Test]
        public void FactoryTemplateTester()
        {
            var container =
                new Container();

            container.GetInstance<Func<ConcreteClass>>()().ShouldNotBeNull();
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

        public class ConcreteClass
        {
        }
    }
}