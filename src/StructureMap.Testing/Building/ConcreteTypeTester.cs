using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Graph;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class ConcreteTypeTester
    {
        [Test]
        public void no_value_for_non_simple_resolves_to_default_source()
        {
            ConcreteType.SourceFor(typeof (IGateway), null)
                .ShouldBeOfType<DefaultDependencySource>()
                .DependencyType.ShouldEqual(typeof(IGateway));
        }

        [Test]
        public void value_is_instance_for_non_simple_resolves_to_lifecycle_source()
        {
            var instance = new FakeInstance();
            instance.SetScopeTo(new SingletonLifecycle());

            var source = ConcreteType.SourceFor(typeof (IGateway), instance)
                .ShouldBeOfType<LifecycleDependencySource>();

            source.Instance.ShouldBeTheSameAs(instance);
            source.PluginType.ShouldEqual(typeof (IGateway));
        }

        [Test]
        public void if_value_exists_and_it_is_the_right_type_return_constant()
        {
            ConcreteType.SourceFor(typeof (string), "foo")
                .ShouldEqual(Constant.For("foo"));

            ConcreteType.SourceFor(typeof(int), 42)
                .ShouldEqual(Constant.For<int>(42));

            // My dad raises registered Beefmasters and he'd be disappointed
            // if the default here was anything else
            ConcreteType.SourceFor(typeof (BreedEnum), BreedEnum.Beefmaster)
                .ShouldEqual(Constant.For(BreedEnum.Beefmaster));

            var gateway = new StubbedGateway();
            ConcreteType.SourceFor(typeof (IGateway), gateway)
                .ShouldEqual(Constant.For<IGateway>(gateway));


            
        }

        [Test]
        public void if_list_value_exists_use_that()
        {
            var list = new List<IGateway> { new StubbedGateway(), new StubbedGateway() };
            ConcreteType.SourceFor(typeof (List<IGateway>), list)
                .ShouldEqual(Constant.For(list));

            ConcreteType.SourceFor(typeof(IList<IGateway>), list)
                .ShouldEqual(Constant.For<IList<IGateway>>(list));
        }

        [Test, Ignore("next step")]
        public void array_can_be_coerced_to_concrete_list()
        {
            var list = new IGateway[] { new StubbedGateway(), new StubbedGateway() };
            var constant = ConcreteType.SourceFor(typeof (List<IGateway>), list)
                .ShouldBeOfType<Constant>();

            constant.ArgumentType.ShouldEqual(typeof (List<IGateway>));
            constant.Value.As<List<IGateway>>()
                .ShouldHaveTheSameElementsAs(list);


        }


    }
}