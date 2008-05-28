using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class AddTypesTester
    {
        public interface IAddTypes
        {
        }

        public class RedAddTypes : IAddTypes
        {
        }

        public class GreenAddTypes : IAddTypes
        {
        }

        public class BlueAddTypes : IAddTypes
        {
        }

        public class PurpleAddTypes : IAddTypes
        {
        }

        [Test]
        public void A_concrete_type_is_available_by_name_when_it_is_added_by_the_shorthand_mechanism()
        {
            IContainer manager = new StructureMap.Container(delegate(Registry registry)
            {
                registry.ForRequestedType<IAddTypes>()
                    .AddConcreteType<RedAddTypes>("Red")
                    .AddConcreteType<GreenAddTypes>("Green")
                    .AddConcreteType<BlueAddTypes>("Blue")
                    .AddConcreteType<PurpleAddTypes>();
            });

            Assert.IsInstanceOfType(typeof (RedAddTypes), manager.GetInstance<IAddTypes>("Red"));
            Assert.IsInstanceOfType(typeof (GreenAddTypes), manager.GetInstance<IAddTypes>("Green"));
            Assert.IsInstanceOfType(typeof (BlueAddTypes), manager.GetInstance<IAddTypes>("Blue"));
        }

        [Test]
        public void A_concrete_type_is_available_when_it_is_added_by_the_shorthand_mechanism()
        {
            IContainer manager = new StructureMap.Container(delegate(Registry registry)
            {
                registry.ForRequestedType<IAddTypes>()
                    .AddConcreteType<RedAddTypes>()
                    .AddConcreteType<GreenAddTypes>()
                    .AddConcreteType<BlueAddTypes>()
                    .AddConcreteType<PurpleAddTypes>();
            });


            IList<IAddTypes> instances = manager.GetAllInstances<IAddTypes>();
            Assert.AreEqual(4, instances.Count);
        }

        [Test]
        public void Make_sure_that_we_dont_double_dip_instances_when_we_register_a_type_with_a_name()
        {
            IContainer manager = new StructureMap.Container(delegate(Registry registry)
            {
                registry.ForRequestedType<IAddTypes>()
                    .AddConcreteType<RedAddTypes>("Red")
                    .AddConcreteType<GreenAddTypes>()
                    .AddConcreteType<BlueAddTypes>("Blue")
                    .AddConcreteType<PurpleAddTypes>();
            });

            IList<IAddTypes> instances = manager.GetAllInstances<IAddTypes>();
            Assert.AreEqual(4, instances.Count);
        }
    }
}