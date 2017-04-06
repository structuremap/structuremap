using System;
using Shouldly;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_552_ProfileAndChild_SameHandlingOfSingleton
    {
        [Fact]
        public void ProfileAndChild_SameHandlingOfSingleton()
        {
            var container = new Container(c =>
            {
                c.For<I>().Use<C>().Singleton();
                c.For<IFoo>().Use<Foo>().Singleton();

                c.Profile("a", p =>
                {
                    p.For<I>().Use<C>().Singleton();
                    p.For<IFoo>().Use<AlternateFoo>().Singleton();
                });
            });

            //Requesting "C" (implementation)
            container.GetInstance<C>().M().ShouldBe(nameof(Foo)); //OK
            container.GetProfile("a").GetInstance<C>().M().ShouldBe(nameof(AlternateFoo)); //OK

            //Requesting "I" (abstraction)
            container.GetInstance<I>().M().ShouldBe(nameof(Foo)); //OK
            container.GetProfile("a").GetInstance<I>().M().ShouldBe(nameof(AlternateFoo)); //Fixed

            var childContainer = container.CreateChildContainer();
            childContainer.Configure(c =>
            {
                c.For<I>().Use<C>().Singleton();
                c.For<IFoo>().Use<AlternateFoo>().Singleton();
            });
            childContainer.GetInstance<I>().M().ShouldBe(nameof(AlternateFoo)); //OK
            childContainer.GetInstance<C>().M().ShouldBe(nameof(AlternateFoo)); //OK
        }

        interface I { string M(); }

        class C : I
        {
            private readonly IFoo foo;

            public C(IFoo foo) { this.foo = foo; }

            public string M() => this.foo.M();
        }

        interface IFoo { string M(); }

        class Foo : IFoo { public string M() => nameof(Foo); }

        class AlternateFoo : IFoo { public string M() => nameof(AlternateFoo); }
    }
}