using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class ContainerIsInTheContainerTester
    {
        [Fact]
        public void build_a_class_that_needs_a_container_and_inject_the_current_container()
        {
            var container = new Container();
            container.GetInstance<ClassThatNeedsContainer>().Container.ShouldBeTheSameAs(container);
        }

        [Fact]
        public void build_a_container_and_retrieve_the_container_itself_through_service_location()
        {
            var container = new Container();
            container.GetInstance<IContainer>().ShouldBeTheSameAs(container);
        }
    }

    public class ClassThatNeedsContainer
    {
        private readonly IContainer _container;

        public ClassThatNeedsContainer(IContainer container)
        {
            _container = container;
        }

        public IContainer Container
        {
            get { return _container; }
        }
    }
}