using NUnit.Framework;
using Shouldly;
using StructureMap.Testing.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_416_missing_name_instance_in_try_get_instance
    {
        [Test]
        public void should_be_able_to_resolve_by_name()
        {
            var container = new Container(_ =>
            {
                _.For<ColorRule>().MissingNamedInstanceIs.ConstructedBy(c => new ColorRule(c.RequestedName));
            });

            container.GetInstance<ColorRule>("Red").Color.ShouldBe("Red");

            container.TryGetInstance<ColorRule>("Red").Color.ShouldBe("Red");
            container.TryGetInstance<ColorRule>("Blue").Color.ShouldBe("Blue");
            container.TryGetInstance<ColorRule>("Green").Color.ShouldBe("Green");
        }
    }
}