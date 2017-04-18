using System;
using System.Collections.Generic;
using StructureMap.Testing.Widget;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_562_injecting_list_with_explicit_args
    {

        [Fact]
        public void should_be_able_to_inject_list_in_as_explicit_variable()
        {
            var things = new List<Thing> {new Thing(), new Thing()};

            var container = new Container(_ => _.ForConcreteType<ThingHolder>().Configure.Singleton());

            var holder = container.With<IList<Thing>>(things).GetInstance<ThingHolder>();

            holder.Things.ShouldBeTheSameAs(things);
        }


        public class ThingHolder
        {
            public IList<Thing> Things { get; }

            public ThingHolder(IList<Thing> things)
            {
                Things = things;
            }
        }

        public class Thing
        {
            
        }
    }
}