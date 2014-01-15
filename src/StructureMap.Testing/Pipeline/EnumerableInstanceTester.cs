using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

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
        public void to_dependency_source_with_all_possible_values()
        {
            new EnumerableInstance(new Instance[0])
                .ToDependencySource(typeof (IList<IGateway>))
                .ShouldEqual(new AllPossibleValuesDependencySource(typeof (IList<IGateway>)));
        }

        [Test]
        public void to_dependency_source_as_array_with_explicit_values()
        {
            var i1 = new FakeInstance();
            var i2 = new FakeInstance();
            var i3 = new FakeInstance();
            var enumerableType = typeof (IGateway[]);
            var source = new EnumerableInstance(new Instance[] {i1, i2, i3})
                .ToDependencySource(enumerableType)
                .ShouldBeOfType<ArrayDependencySource>();

            source.ItemType.ShouldEqual(typeof (IGateway));
            source.Items.ShouldHaveTheSameElementsAs(i1.DependencySource, i2.DependencySource, i3.DependencySource);
        }

        [Test]
        public void to_dependency_source_as_ilist_with_explicit_values()
        {
            var i1 = new FakeInstance();
            var i2 = new FakeInstance();
            var i3 = new FakeInstance();
            var enumerableType = typeof (IList<IGateway>);
            var source = new EnumerableInstance(new Instance[] {i1, i2, i3})
                .ToDependencySource(enumerableType)
                .ShouldBeOfType<ArrayDependencySource>();

            source.ItemType.ShouldEqual(typeof (IGateway));
            source.Items.ShouldHaveTheSameElementsAs(i1.DependencySource, i2.DependencySource, i3.DependencySource);
        }

        [Test]
        public void to_dependency_source_as_ienumerable_with_explicit_values()
        {
            var i1 = new FakeInstance();
            var i2 = new FakeInstance();
            var i3 = new FakeInstance();
            var enumerableType = typeof (IEnumerable<IGateway>);
            var source = new EnumerableInstance(new Instance[] {i1, i2, i3})
                .ToDependencySource(enumerableType)
                .ShouldBeOfType<ArrayDependencySource>();

            source.ItemType.ShouldEqual(typeof (IGateway));
            source.Items.ShouldHaveTheSameElementsAs(i1.DependencySource, i2.DependencySource, i3.DependencySource);
        }

        [Test]
        public void to_dependency_source_as_list_with_explicit_values()
        {
            var i1 = new FakeInstance();
            var i2 = new FakeInstance();
            var i3 = new FakeInstance();
            var enumerableType = typeof (List<IGateway>);
            var source = new EnumerableInstance(new Instance[] {i1, i2, i3})
                .ToDependencySource(enumerableType)
                .ShouldBeOfType<ListDependencySource>();

            source.ItemType.ShouldEqual(typeof (IGateway));
            source.Items.ShouldHaveTheSameElementsAs(i1.DependencySource, i2.DependencySource, i3.DependencySource);
        }

        [Test]
        public void build_children_to_a_list()
        {
            var children = new Instance[]
            {
                new SmartInstance<ColorWidget>().Ctor<string>("color").Is("red"),
                new SmartInstance<ColorWidget>().Ctor<string>("color").Is("green"),
                new ObjectInstance(new AWidget())
            };

            var theInstance = new EnumerableInstance(children);
            Assert.Fail("NWO");
//            var list =
//                theInstance.Build(typeof (IList<IWidget>), new StubBuildSession()).ShouldBeOfType<List<IWidget>>();
//
//            list.Count.ShouldEqual(3);
//
//            list[0].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("red");
//            list[2].ShouldBeOfType<AWidget>();
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

            var theInstance = new EnumerableInstance(children);
            Assert.Fail("NWO");
//            var list = theInstance.Build(typeof (IWidget[]), new StubBuildSession()).ShouldBeOfType<IWidget[]>();
//
//            list.Length.ShouldEqual(3);
//
//            list[0].ShouldBeOfType<ColorWidget>().Color.ShouldEqual("red");
//            list[2].ShouldBeOfType<AWidget>();
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