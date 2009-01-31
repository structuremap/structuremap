using NUnit.Framework;
using StructureMap.Prism;

namespace StructureMap.Testing.Prism
{
    [TestFixture]
    public class StructureMapContainerFacadeTester
    {
        [Test]
        public void can_use_inner_container_to_resolve_a_type()
        {
            var container = new Container(x => { x.ForRequestedType<IView>().TheDefaultIsConcreteType<View>(); });

            var facade = new StructureMapContainerFacade(container);
            facade.Resolve(typeof (IView)).ShouldBeOfType<View>();
        }

        [Test]
        public void can_use_inner_container_to_try_resolve_successfully()
        {
            var container = new Container(x => { x.ForRequestedType<IView>().TheDefaultIsConcreteType<View>(); });

            var facade = new StructureMapContainerFacade(container);
            facade.TryResolve(typeof (IView)).ShouldBeOfType<View>();
        }

        [Test]
        public void can_use_inner_container_to_try_resolve_when_type_is_not_there()
        {
            var facade = new StructureMapContainerFacade(new Container());
            facade.TryResolve(typeof (IView)).ShouldBeNull();
        }
    }

    public interface IView
    {
    }

    public class View : IView
    {
    }
}