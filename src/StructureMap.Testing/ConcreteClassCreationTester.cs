using Shouldly;
using StructureMap.Testing.Widget;
using System.Linq;
using Xunit;

namespace StructureMap.Testing
{
    public class ConcreteClassCreationTester
    {
        public ConcreteClassCreationTester()
        {
            container = new Container(_ => { _.For<IWidget>().Use(new ColorWidget("red")); });
        }

        private Container container;

        public class ConcreteClass
        {
            private readonly IWidget _widget;

            public ConcreteClass(IWidget widget)
            {
                _widget = widget;
            }

            public IWidget Widget1
            {
                get { return _widget; }
            }
        }

        [Fact]
        public void can_create_a_concrete_class_by_default()
        {
            container.GetInstance<ConcreteClass>()
                .Widget1.ShouldBeOfType<ColorWidget>();
        }

        [Fact]
        public void the_instance_count_is_zero()
        {
            container.GetAllInstances<ConcreteClass>().Any().ShouldBeFalse();
        }
    }
}