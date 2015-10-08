using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing
{
    [TestFixture]
    public class using_default_values_on_constructor_function_if_no_explicit_value
    {
        // SAMPLE: GuyWithName-defaults
        // I was listening to Jim Croce's "I've got a Name" song
        // when I wrote this feature;)
        public class GuyWithName
        {
            public GuyWithName(string name = "Jim Croce")
            {
                Name = name;
            }

            public string Name { get; set; }
        }

        [Test]
        public void uses_the_default_value_if_one_exists()
        {
            var container = new Container();

            // Should happily build with the default
            // value of 'name'
            container.GetInstance<GuyWithName>()
                .Name.ShouldBe("Jim Croce");
        }

        [Test]
        public void uses_the_default_value_if_one_exists_2()
        {
            var container = new Container(_ =>
            {
                _.ForConcreteType<GuyWithName>();
            });

            // Should happily build with the default
            // value of 'name'
            container.GetInstance<GuyWithName>()
                .Name.ShouldBe("Jim Croce");
        }

        [Test]
        public void use_explicit_dependency_if_one_exists()
        {
            var container = new Container(_ =>
            {
                _.ForConcreteType<GuyWithName>()
                    .Configure.Ctor<string>("name").Is("Eric Clapton");
            });

            container.GetInstance<GuyWithName>()
                .Name.ShouldBe("Eric Clapton");
        }
        // ENDSAMPLE




        // SAMPLE: GuyWithNoDefaultName
        public class GuyWithNoDefaultName
        {
            // StructureMap will not use any kind of auto-wiring
            // on name
            public GuyWithNoDefaultName(string name)
            {
            }
        }

        [Test]
        public void cannot_build_simple_arguments()
        {
            var container = new Container();

            Exception<StructureMapBuildPlanException>.ShouldBeThrownBy(() =>
            {
                container.GetInstance<GuyWithNoDefaultName>();
            });
        }
        // ENDSAMPLE

        // SAMPLE: GuyWithNoDefaultName-explicit-argument
        [Test]
        public void can_build_with_explicit_argument()
        {
            var container = new Container(_ =>
            {
                _.ForConcreteType<GuyWithNoDefaultName>()
                    .Configure.Ctor<string>("name").Is("Steve Winwood");
            });

            container.GetInstance<GuyWithNoDefaultName>()
                .ShouldNotBeNull();
        }
        // ENDSAMPLE
        
    }
}