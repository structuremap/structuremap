using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;
using System.Collections.Generic;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class DependencyCollectionTester
    {
        private readonly IGateway gateway1 = new StubbedGateway();
        private readonly IGateway gateway2 = new StubbedGateway();
        private readonly IGateway gateway3 = new StubbedGateway();
        private readonly IService service1 = new ColorService("red");

        [Fact]
        public void if_an_enumerable_type_and_there_is_no_exact_match_by_type_try_ienumerable_of_the_element_type()
        {
            var collection = new DependencyCollection();
            var instance = new EnumerableInstance(new Instance[0]);

            collection.Add(typeof(IEnumerable<IGateway>), instance);

            collection.FindByTypeOrName(typeof(IList<IGateway>), null).ShouldBeTheSameAs(instance);
            collection.FindByTypeOrName(typeof(List<IGateway>), null).ShouldBeTheSameAs(instance);
            collection.FindByTypeOrName(typeof(IGateway[]), null).ShouldBeTheSameAs(instance);
        }

        [Fact]
        public void get_by_name_with_nothing_returns_null()
        {
            var collection = new DependencyCollection();
            collection.FindByTypeOrName(typeof(IGateway), "foo")
                .ShouldBeNull();
        }

        [Fact]
        public void add_and_retrieve_by_name_only()
        {
            var collection = new DependencyCollection();
            collection.Add("foo", gateway1);
            collection.Add("bar", gateway2);

            collection.FindByTypeOrName(typeof(IGateway), "foo")
                .ShouldBeTheSameAs(gateway1);

            collection.FindByTypeOrName(typeof(IGateway), "bar")
                .ShouldBeTheSameAs(gateway2);
        }

        [Fact]
        public void add_and_retrieve_by_type_only()
        {
            var collection = new DependencyCollection();
            collection.Add(typeof(IService), service1);
            collection.Add(typeof(IGateway), gateway3);

            collection.FindByTypeOrName(typeof(IGateway), "foo")
                .ShouldBe(gateway3);

            collection.FindByTypeOrName(typeof(IService), "anything")
                .ShouldBe(service1);
        }

        [Fact]
        public void use_most_specific_criteria_if_possible()
        {
            var collection = new DependencyCollection();
            collection.Add("foo", typeof(IGateway), gateway1);
            collection.Add("bar", typeof(IGateway), gateway2);
            collection.Add("bar", typeof(IService), service1);

            collection.FindByTypeOrName(typeof(IGateway), "foo")
                .ShouldBeTheSameAs(gateway1);

            collection.FindByTypeOrName(typeof(IGateway), "bar")
                .ShouldBeTheSameAs(gateway2);

            collection.FindByTypeOrName(typeof(IService), "bar")
                .ShouldBeTheSameAs(service1);
        }

        [Fact]
        public void can_override_and_last_one_in_wins()
        {
            var collection = new DependencyCollection();
            collection.Add("foo", typeof(IGateway), gateway1);
            collection.Add("foo", typeof(IGateway), gateway2);

            collection.FindByTypeOrName(typeof(IGateway), "foo")
                .ShouldBeTheSameAs(gateway2);
        }
    }
}