using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ConcreteClassCreationTester
    {
        private Container container;

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.For<IWidget>().Use(new ColorWidget("red"));
            });
        }

        [Test]
        public void can_create_a_concrete_class_by_default()
        {
            container.GetInstance<ConcreteClass>().Widget1.ShouldBeOfType<ColorWidget>();
        }

        [Test]
        public void the_instance_count_is_zero()
        {
            container.GetAllInstances<ConcreteClass>().Count.ShouldEqual(0); 
        }

        public class ConcreteClass
        {
            private readonly IWidget _widget;

            public ConcreteClass(IWidget widget)
            {
                _widget = widget;
            }

            public IWidget Widget1 { get { return _widget; } }
        }
    }
}