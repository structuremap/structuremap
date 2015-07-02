using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class TryGetInstanceWithOpenGenericsBugTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void can_get_closing_type_if_starting_from_a_base_type()
        {
            typeof (ClosedClass<string>).FindFirstInterfaceThatCloses(typeof (IOpenClass<>)).ShouldBe(
                typeof (IOpenClass<string>));
        }

        [Test]
        public void try_get_instance_fills_from_open_generic()
        {
            var container = new Container(x => { x.For(typeof (IOpenClass<>)).Add(typeof (ClosedClass<>)); });

            container.TryGetInstance<IOpenClass<string>>().ShouldBeOfType<ClosedClass<string>>();
        }

        [Test]
        public void try_get_instance_fills_from_open_generic_on_conventions()
        {
            var container = new Container(x =>
            {
                x.Scan(o =>
                {
                    o.TheCallingAssembly();
                    o.ConnectImplementationsToTypesClosing(typeof (IOpenClass<>));
                });
            });

            container.GetInstance<IOpenClass<string>>().ShouldBeOfType<ClosedStringClass>();
            container.TryGetInstance<IOpenClass<string>>().ShouldBeOfType<ClosedStringClass>();
        }
    }

    public class IOpenClass<T>
    {
    }

    public class ClosedClass<T> : IOpenClass<T>
    {
    }

    public class ClosedStringClass : IOpenClass<string>
    {
    }
}