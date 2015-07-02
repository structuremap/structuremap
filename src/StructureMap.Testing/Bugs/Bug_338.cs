using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_338
    {
        public abstract class ParentClass
        {
        }

        public class ChildClass : ParentClass
        {
        }

        public abstract class OtherChildClass : ParentClass { }

        [Test]
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