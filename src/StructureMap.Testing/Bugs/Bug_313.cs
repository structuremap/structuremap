using System.Linq;
using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_313
    {
        [Test]
        public void exclude_type_does_indeed_work()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf<IFoo>();
                    x.ExcludeType<Foo2>();
                });
            });

            container.GetAllInstances<IFoo>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof (Foo1), typeof (Foo3));
        }

        [Test, Explicit("This test is invalid and will fail, included to demonstrate a usage problem")]
        public void demo_of_problem()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf<IFoo>();
                    x.ExcludeType<Foo2>();
                });

                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf<IFoo>();
                    x.ExcludeType<Foo1>();
                });
            });

            // This will fail! 
            container.GetAllInstances<IFoo>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof (Foo3));
        }

        public interface IFoo
        {
        }

        public class Foo1 : IFoo
        {
        }

        public class Foo2 : IFoo
        {
        }

        public class Foo3 : IFoo
        {
        }
    }
}