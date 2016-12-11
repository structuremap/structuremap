using NUnit.Framework;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Bugs
{
    public class OnCreation_doesnt_work_for_inline_dependencies
    {
        [Test]
        public void should_call_OnCreation_for_regstered_inline_dependencies()
        {
            Container.For<TheRegistry>().GetInstance<IRootType>();
            Dependency.StuffWasDone.ShouldBeTrue();
        }

        public class TheRegistry : Registry
        {
            public TheRegistry()
            {
                For<IRootType>().Use<RootType>()
                    .Ctor<IDependency>().Is<Dependency>(x => x.OnCreation(y => y.DoStuff()));
            }
        }

        public interface IRootType { }
        public class RootType : IRootType
        {
            public RootType(IDependency dependency)
            {
            }
        }

        public interface IDependency { }
        public class Dependency : IDependency
        {
            public static bool StuffWasDone;

            public void DoStuff()
            {
                StuffWasDone = true;
            }
        }
    }
}
