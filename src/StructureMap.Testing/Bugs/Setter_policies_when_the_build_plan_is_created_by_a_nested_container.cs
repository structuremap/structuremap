using System;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Setter_policies_when_the_build_plan_is_created_by_a_nested_container
    {
        [Test]
        public void setter_policies_should_be_applied_in_nested_container()
        {
            var container = new Container(cfg => { cfg.Policies.SetAllProperties(x => x.OfType<Injected>()); });


            var product = container.GetNestedContainer().GetInstance<Product>();
            product.Inject.ShouldNotBeNull();
        }

        [Test]
        public void setter_policies_should_be_applied_in_profile_container()
        {
            var container = new Container(cfg => { cfg.Policies.SetAllProperties(x => x.OfType<Injected>()); });


            var product = container.GetProfile("Foo").GetInstance<Product>();
            product.Inject.ShouldNotBeNull();
        }
    }

    public class Product
    {
        public Injected Inject { get; set; }
    }

    public class Injected
    {
        public Injected()
        {
            Name = Guid.NewGuid().ToString("N");
        }

        public string Name { get; set; }
    }
}