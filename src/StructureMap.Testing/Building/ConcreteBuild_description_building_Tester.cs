using StructureMap.Attributes;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using System.Diagnostics;
using Xunit;

namespace StructureMap.Testing.Building
{
    public class ConcreteBuild_description_building_Tester
    {
        [Fact]
        public void see_the_description_of_class_with_ctors_and_setters()
        {
            var build = ConcreteType.BuildSource(typeof(GuyWithCtorAndArgs), null, new DependencyCollection(),
                Policies.Default());

            Debug.WriteLine(build.Description);
        }

        [Fact]
        public void see_the_description_of_class_with_ctors_and_setters_with_inline_depencencies()
        {
            var dependencies = new DependencyCollection();
            dependencies.Add(typeof(Rule), new ColorRule("Red"));
            dependencies.Add(typeof(IWidget), new AWidget());

            var build = ConcreteType.BuildSource(typeof(GuyWithCtorAndArgs), null, dependencies,
                Policies.Default());

            Debug.WriteLine(build.Description);
        }

        [Fact]
        public void see_the_description_of_class_with__only_ctors()
        {
            var build = ConcreteType.BuildSource(typeof(GuyWithOnlyCtor), null, new DependencyCollection(),
                Policies.Default());

            Debug.WriteLine(build.Description);
        }

        [Fact]
        public void see_the_description_of_class_with_only_ctor_with_inline_depencencies()
        {
            var dependencies = new DependencyCollection();
            dependencies.Add(typeof(Rule), new ColorRule("Red"));
            dependencies.Add(typeof(IWidget), new AWidget());

            var build = ConcreteType.BuildSource(typeof(GuyWithOnlyCtor), null, dependencies,
                Policies.Default());

            Debug.WriteLine(build.Description);
        }
    }

    public class GuyWithOnlyCtor
    {
        public GuyWithOnlyCtor(IGateway gateway, Rule rule)
        {
        }
    }

    public class GuyWithCtorAndArgs
    {
        public GuyWithCtorAndArgs(IGateway gateway, Rule rule)
        {
        }

        [SetterProperty]
        public IWidget Widget { get; set; }

        [SetterProperty]
        public IService Service { get; set; }
    }
}