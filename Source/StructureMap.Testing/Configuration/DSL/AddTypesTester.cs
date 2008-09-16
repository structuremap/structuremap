using System.Collections.Generic;
using NUnit.Framework;

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
            IContainer container = new Container(r => r.ForRequestedType<IAddTypes>().AddInstances(x =>
            {
                x.OfConcreteType<RedAddTypes>().WithName("Red");
                x.OfConcreteType<GreenAddTypes>().WithName("Green");
                x.OfConcreteType<BlueAddTypes>().WithName("Blue");
                x.OfConcreteType<PurpleAddTypes>();
            }));

            container.GetInstance<IAddTypes>("Red").IsType<RedAddTypes>();
            container.GetInstance<IAddTypes>("Green").IsType<GreenAddTypes>();
            container.GetInstance<IAddTypes>("Blue").IsType<BlueAddTypes>();
        }

        [Test]
        public void A_concrete_type_is_available_when_it_is_added_by_the_shorthand_mechanism()
        {
            IContainer manager = new Container(registry => registry.ForRequestedType<IAddTypes>()
                                                               .AddConcreteType<RedAddTypes>()
                                                               .AddConcreteType<GreenAddTypes>()
                                                               .AddConcreteType<BlueAddTypes>()
                                                               .AddConcreteType<PurpleAddTypes>());


            IList<IAddTypes> instances = manager.GetAllInstances<IAddTypes>();
            Assert.AreEqual(4, instances.Count);
        }

        [Test]
        public void Make_sure_that_we_dont_double_dip_instances_when_we_register_a_type_with_a_name()
        {
            IContainer manager = new Container(r =>
                                               r.ForRequestedType<IAddTypes>().AddInstances(x =>
                                               {
                                                   x.OfConcreteType<GreenAddTypes>();
                                                   x.OfConcreteType<BlueAddTypes>();
                                                   x.OfConcreteType<PurpleAddTypes>();
                                                   x.OfConcreteType<PurpleAddTypes>().WithName("Purple");
                                               })
                );

            IList<IAddTypes> instances = manager.GetAllInstances<IAddTypes>();
            Assert.AreEqual(4, instances.Count);
        }
    }
}