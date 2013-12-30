using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ConstructorSelectorTester
    {
        [Test]
        public void get_the_first_constructor_marked_with_the_attribute_if_it_exists()
        {
            var selector = new ConstructorSelector();

            var constructor = selector.Select(typeof (ComplexRule));

            constructor.GetParameters().Length
                .ShouldEqual(7);
        }

        [Test]
        public void should_get_the_greediest_constructor_if_there_is_more_than_one()
        {
            var selector = new ConstructorSelector();
            var constructor = selector.Select(typeof (GreaterThanRule));

            constructor.GetParameters().Select(x => x.ParameterType)
                .ShouldHaveTheSameElementsAs(typeof(string), typeof(int));
        }
    }
}