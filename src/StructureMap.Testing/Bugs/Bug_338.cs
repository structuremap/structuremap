using Shouldly;
using StructureMap.Graph;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_338
    {
        public abstract class ParentClass
        {
        }

        public class ChildClass : ParentClass
        {
        }

        public abstract class OtherChildClass : ParentClass { }

        [Fact]
        public void be_smart_and_do_not_add_abstract_types()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf<ParentClass>();
                });
            });

            container.GetAllInstances<ParentClass>()
                .Single().ShouldBeOfType<ChildClass>();
        }
    }
}