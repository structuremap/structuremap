using StructureMap.Graph;
using StructureMap.Testing.xUnit;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_313
    {
        [Fact]
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
                .ShouldHaveTheSameElementsAs(typeof(Foo1), typeof(Foo3));
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