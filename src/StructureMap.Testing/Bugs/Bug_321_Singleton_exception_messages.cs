﻿using NUnit.Framework;
using StructureMap.Building;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_321_Singleton_exception_messages
    {
        [Test]
        public void make_it_happen()
        {
            var container = new Container(_ =>
            {
                _.ForSingletonOf<SingleGuy>().Use<SingleGuy>();
            });

            var ex = Exception<StructureMapBuildException>.ShouldBeThrownBy(() =>
            {
                container.GetInstance<SingleGuy>();
            });

            ex.Message.ShouldContain("new SingleGuyDependency(*Default of SingleGuy*)");
            ex.Message.ShouldContain("new SingleGuy(*Default of SingleGuyDependency*)");
        }

        public class SingleGuy
        {
            private readonly SingleGuyDependency _dependency;

            public SingleGuy(SingleGuyDependency dependency)
            {
                _dependency = dependency;
            }
        }

        public class SingleGuyDependency
        {
            public SingleGuyDependency(SingleGuy guy)
            {
            }
        }
    }
}