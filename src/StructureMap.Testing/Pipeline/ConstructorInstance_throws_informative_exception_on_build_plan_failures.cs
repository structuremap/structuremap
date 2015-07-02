using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Building;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ConstructorInstance_throws_informative_exception_on_build_plan_failures
    {
        [Test]
        public void try_the_exception_message()
        {
            var instance = new ConstructorInstance(typeof (GuyWithPrimitives));
            instance.Dependencies.Add("name", "Jeremy");

            var ex =
                Exception<StructureMapBuildPlanException>.ShouldBeThrownBy(
                    () => { instance.ToBuilder(typeof (GuyWithPrimitives), new Policies()); });

            ex.ToString()
                .ShouldContain(
                    "Unable to create a build plan for concrete type StructureMap.Testing.Building.GuyWithPrimitives");

            Debug.WriteLine(ex);
        }
    }
}