using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class AddTypesTester
    {
        [Test]
        public void A_concrete_type_is_available_when_it_is_added_by_the_shorthand_mechanism()
        {
            Registry registry = new Registry();
            registry.ForRequestedType<IAddTypes>()
                .AddConcreteType<RedAddTypes>()
                .AddConcreteType<GreenAddTypes>()
                .AddConcreteType<BlueAddTypes>()
                .AddConcreteType<PurpleAddTypes>();

            IList<IAddTypes> instances = registry.BuildInstanceManager().GetAllInstances<IAddTypes>();
            Assert.AreEqual(4, instances.Count);
        }

        [Test]
        public void A_concrete_type_is_available_by_name_when_it_is_added_by_the_shorthand_mechanism()
        {
            Registry registry = new Registry();
            registry.ForRequestedType<IAddTypes>()
                .AddConcreteType<RedAddTypes>("Red")
                .AddConcreteType<GreenAddTypes>("Green")
                .AddConcreteType<BlueAddTypes>("Blue")
                .AddConcreteType<PurpleAddTypes>();

            IInstanceManager manager = registry.BuildInstanceManager();
            Assert.IsInstanceOfType(typeof(RedAddTypes), manager.CreateInstance<IAddTypes>("Red"));
            Assert.IsInstanceOfType(typeof(GreenAddTypes), manager.CreateInstance<IAddTypes>("Green"));
            Assert.IsInstanceOfType(typeof(BlueAddTypes), manager.CreateInstance<IAddTypes>("Blue"));
        }

        [Test]
        public void Make_sure_that_we_dont_double_dip_instances_when_we_register_a_type_with_a_name()
        {
            Registry registry = new Registry();
            registry.ForRequestedType<IAddTypes>()
                .AddConcreteType<RedAddTypes>("Red")
                .AddConcreteType<GreenAddTypes>()
                .AddConcreteType<BlueAddTypes>("Blue")
                .AddConcreteType<PurpleAddTypes>();

            IList<IAddTypes> instances = registry.BuildInstanceManager().GetAllInstances<IAddTypes>();
            Assert.AreEqual(4, instances.Count);

        }



        public interface IAddTypes
        {
            
        }

        public class RedAddTypes : IAddTypes{}
        public class GreenAddTypes : IAddTypes{}
        public class BlueAddTypes : IAddTypes{}
        public class PurpleAddTypes : IAddTypes{}
    }
}
