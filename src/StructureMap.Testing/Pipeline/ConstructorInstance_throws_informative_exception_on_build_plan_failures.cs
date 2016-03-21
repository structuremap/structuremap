using StructureMap.Pipeline;
using StructureMap.Testing.Building;
using System.Diagnostics;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class ConstructorInstance_throws_informative_exception_on_build_plan_failures
    {
        [Fact]
        public void try_the_exception_message()
        {
            var instance = new ConstructorInstance(typeof(GuyWithPrimitives));
            instance.Dependencies.Add("name", "Jeremy");

            var ex =
                Exception<StructureMapBuildPlanException>.ShouldBeThrownBy(
                    () => { instance.ToBuilder(typeof(GuyWithPrimitives), Policies.Default()); });

            ex.ToString()
                .ShouldContain(
                    "Unable to create a build plan for concrete type StructureMap.Testing.Building.GuyWithPrimitives");

            Debug.WriteLine(ex);
        }
    }
}