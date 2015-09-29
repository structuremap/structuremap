using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing
{
    [TestFixture]
    public class using_default_values_on_constructor_function_if_no_explicit_value
    {
        [Test]
        public void uses_the_default_value_if_one_exists()
        {
            var container = new Container();

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

            container.GetInstance<GuyWithName>()
                .Name.ShouldBe("Jim Croce");
        }

        [Test]
        public void use_explicit_dependency_if_one_exists()
        {
            var container = new Container(_ =>
            {
                _.ForConcreteType<GuyWithName>().Configure.Ctor<string>("name").Is("Eric Clapton");
            });

            container.GetInstance<GuyWithName>()
                .Name.ShouldBe("Eric Clapton");
        }

        public class GuyWithName
        {
            public GuyWithName(string name = "Jim Croce")
            {
                Name = name;
            }

            public string Name { get; set; }
        }    
    }
}