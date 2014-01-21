using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace StructureMap.Testing
{
    [TestFixture]
    public class WhatDoIHave_Smoke_Tester
    {
        [Test]
        public void display_one_service_for_an_interface()
        {
            var container = new Container(x => {
                x.For<IEngine>().Use<Hemi>().Named("The Hemi");

                x.For<IEngine>().Add<VEight>().Singleton().Named("V8");
                x.For<IEngine>().Add<FourFiftyFour>().AlwaysUnique();
                x.For<IEngine>().Add<StraightSix>().LifecycleIs<ThreadLocalStorageLifecycle>();

                x.For<IEngine>().Add(() => new Rotary()).Named("Rotary");
                x.For<IEngine>().Add(c => c.GetInstance<PluginElectric>());

                x.For<IEngine>().Add(new InlineFour());
            });

            Debug.WriteLine(container.WhatDoIHave());
        }
    }

    public interface IAutomobile
    {
    }

    public interface IEngine
    {
    }

    public class VEight : IEngine{}
    public class StraightSix : IEngine{}
    public class Hemi : IEngine{}
    public class FourFiftyFour : IEngine{}

    public class Rotary : IEngine{}
    public class PluginElectric : IEngine{}

    public class InlineFour : IEngine
    {
        public override string ToString()
        {
            return "I'm an inline 4!";
        }
    }
}