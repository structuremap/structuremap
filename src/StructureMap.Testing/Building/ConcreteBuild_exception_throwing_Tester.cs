using StructureMap.Building;
using StructureMap.Testing.Pipeline;
using System;
using System.Diagnostics;
using Xunit;

namespace StructureMap.Testing.Building
{
    public class ConcreteBuild_exception_throwing_Tester
    {
        [Fact]
        public void happily_throws_build_exception_with_description_of_the_constructor()
        {
            var ex = Exception<StructureMapBuildException>.ShouldBeThrownBy(() =>
            {
                var build = new ConcreteBuild<ClassThatBlowsUp>();
                var session = new StubBuildSession();
                build.Build(session, session);
            });

            Debug.WriteLine(ex);
        }

        [Fact]
        public void exception_throwing_from_container()
        {
            var ex =
                Exception<StructureMapBuildException>.ShouldBeThrownBy(
                    () => { new Container().GetInstance<ClassThatBlowsUp>(); });

            Debug.WriteLine(ex);
        }
    }

    public class ClassThatBlowsUp
    {
        public ClassThatBlowsUp()
        {
            throw new DivideByZeroException("you cannot pass!");
        }
    }
}