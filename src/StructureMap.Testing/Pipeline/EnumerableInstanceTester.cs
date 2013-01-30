using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class EnumerableInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void build_children_to_a_list()
        {
            var children = new Instance[]
            {
                new SmartInstance<ColorWidget>().Ctor<string>("color").Is("red"),
                new SmartInstance<ColorWidget>().Ctor<string>("color").Is("green"),
                new ObjectInstance(new AWidget())
            };

            var theInstance = new EnumerableInstance(typeof (IList<IWidget>), children);

            var list =
                theInstance.Build(typeof (IList<IWidget>), new StubBuildSession()).ShouldBeOfType<List<IWidget>>();

            list.Count.ShouldEqual(3);

            list[0].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("red");
            list[2].ShouldBeOfType<AWidget>();
        }

        [Test]
        public void build_children_to_an_array()
        {
            var children = new Instance[]
            {
                new SmartInstance<ColorWidget>().Ctor<string>("color").Is("red"),
                new SmartInstance<ColorWidget>().Ctor<string>("color").Is("green"),
                new ObjectInstance(new AWidget())
            };

            var theInstance = new EnumerableInstance(typeof (IWidget[]), children);

            var list = theInstance.Build(typeof (IWidget[]), new StubBuildSession()).ShouldBeOfType<IWidget[]>();

            list.Length.ShouldEqual(3);

            list[0].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("red");
            list[2].ShouldBeOfType<AWidget>();
        }

        [Test]
        public void build_coercion_strategy_for_a_list()
        {
            EnumerableInstance.DetermineCoercion(typeof (IList<IWidget>)).ShouldBeOfType<ListCoercion<IWidget>>();
        }

        [Test]
        public void build_coercion_strategy_for_a_plain_enumerable()
        {
            EnumerableInstance.DetermineCoercion(typeof (IEnumerable<IWidget>)).ShouldBeOfType<ListCoercion<IWidget>>();
        }

        [Test]
        public void build_coercion_strategy_for_an_array()
        {
            EnumerableInstance.DetermineCoercion(typeof (IWidget[])).ShouldBeOfType<ArrayCoercion<IWidget>>();
        }

        [Test]
        public void is_enumerable()
        {
            EnumerableInstance.IsEnumerable(typeof (IWidget[])).ShouldBeTrue();
            EnumerableInstance.IsEnumerable(typeof (IList<IWidget>)).ShouldBeTrue();
            EnumerableInstance.IsEnumerable(typeof (IEnumerable<IWidget>)).ShouldBeTrue();
            EnumerableInstance.IsEnumerable(typeof (List<IWidget>)).ShouldBeTrue();
            EnumerableInstance.IsEnumerable(typeof (IWidget)).ShouldBeFalse();
        }
    }
}