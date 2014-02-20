using System;
using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class Func_Typefactory_construction_strategy_Tester
    {
        [Test]
        public void FactoryTemplateTester()
        {
            var container = new Container();
            var c1 = new ConcreteClass1();
            container.GetInstance<Func<ConcreteClass1,ConcreteClass2>>()(c1).ShouldNotBeNull();
        }

        [Test]
        public void dependency_was_properly_injected()
        {
            var c1 = new ConcreteClass1();
            var c2 = new Container().GetInstance<Func<ConcreteClass1, ConcreteClass2>>()(c1);
            c2.ShouldBeOfType<ConcreteClass2>();
            c2.C1.ShouldBeTheSameAs(c1);
        }

        [Test]
        public void configuration_is_honored()
        {
            var c = new Container(ce => ce.For<IFoo>().Use<ConcreteClass1>());
            var f = c.GetInstance<Func<IFoo, ConcreteClass3>>();
            f.ShouldNotBeNull();

            var c1 = new ConcreteClass1 {SomeGuid = Guid.NewGuid()};
            var c3 = f(c1);
            c3.C1.ShouldBeTheSameAs(c1);
        }

        [Test]
        public void nothing_special_about_a_string_input()
        {
            var c4 = new Container().GetInstance<Func<string, ConcreteClass4>>()("Hello");
            c4.ShouldNotBeNull();
            c4.S.ShouldEqual("Hello");
        }

        public class ConcreteClass1 : IFoo
        {
            public Guid SomeGuid { get; set; }
        }

        public class ConcreteClass2
        {
            public ConcreteClass2(ConcreteClass1 c1)
            {
                C1 = c1;
            }

            public ConcreteClass1 C1 { get; private set; }
        }

        public class ConcreteClass3
        {
            public ConcreteClass3(IFoo c1)
            {
                C1 = c1;
            }

            public IFoo C1 { get; private set; }
        }

        public class ConcreteClass4
        {
            public ConcreteClass4(string s)
            {
                S = s;
            }

            public string S { get; private set; }
        }
    }
}