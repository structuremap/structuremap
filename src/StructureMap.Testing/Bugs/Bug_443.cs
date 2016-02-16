using System.Diagnostics;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_443
    {
        [Test]
        public void should_call_OnCreation_for_regstered_inline_dependencies()
        {
            var container = Container.For<TheRegistry>();

            var plan = container.Model.For<IRootType>().Default.DescribeBuildPlan();
            Debug.WriteLine(plan);


            container.GetInstance<IRootType>();
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