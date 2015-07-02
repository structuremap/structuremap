using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing
{
    [TestFixture]
    public class WhatDoIHave_Smoke_Tester
    {
        [Test]
        public void empty_container()
        {
            // SAMPLE: whatdoihave-simple
            var container = new Container();
            var report = container.WhatDoIHave();

            Debug.WriteLine(report);
            // ENDSAMPLE
        }

        [Test]
        public void display_one_service_for_an_interface()
        {
            // SAMPLE: what_do_i_have_container
            var container = new Container(x =>
            {
                x.For<IEngine>().Use<Hemi>().Named("The Hemi");

                x.For<IEngine>().Add<VEight>().Singleton().Named("V8");
                x.For<IEngine>().Add<FourFiftyFour>().AlwaysUnique();
                x.For<IEngine>().Add<StraightSix>().LifecycleIs<ThreadLocalStorageLifecycle>();

                x.For<IEngine>().Add(() => new Rotary()).Named("Rotary");
                x.For<IEngine>().Add(c => c.GetInstance<PluginElectric>());

                x.For<IEngine>().Add(new InlineFour());

                x.For<IEngine>().UseIfNone<VTwelve>();
                x.For<IEngine>().MissingNamedInstanceIs.ConstructedBy(c => new NamedEngine(c.RequestedName));
            });
            // ENDSAMPLE

            // SAMPLE: whatdoihave_everything
            Debug.WriteLine(container.WhatDoIHave());
            // ENDSAMPLE
        }


        [Test]
        public void render_the_fallback_instance_if_it_exists()
        {
            var container = new Container(x =>
            {
                x.For<IEngine>().Use<Hemi>().Named("The Hemi");

                x.For<IEngine>().Add<VEight>().Singleton().Named("V8");
                x.For<IEngine>().Add<FourFiftyFour>().AlwaysUnique();
                x.For<IEngine>().Add<StraightSix>().LifecycleIs<ThreadLocalStorageLifecycle>();

                x.For<IEngine>().Add(() => new Rotary()).Named("Rotary");
                x.For<IEngine>().Add(c => c.GetInstance<PluginElectric>());

                x.For<IEngine>().Add(new InlineFour());

                x.For<IEngine>().UseIfNone<VTwelve>();
                x.For<IEngine>().MissingNamedInstanceIs.ConstructedBy(c => new NamedEngine(c.RequestedName));
            });

            var description = container.WhatDoIHave();

            description.ShouldContain("*Fallback*");
            description.ShouldContain("StructureMap.Testing.VTwelve");
        }

        [Test]
        public void render_the_missing_named_instance_if_it_exists()
        {
            var container =
                new Container(
                    x =>
                    {
                        x.For<IEngine>().MissingNamedInstanceIs.ConstructedBy(c => new NamedEngine(c.RequestedName));
                    });

            var description = container.WhatDoIHave();
            description.ShouldContain("*Missing Named Instance*");
            description.ShouldContain("Lambda: new NamedEngine(IContext.RequestedName)");
        }

        [Test]
        public void display_one_service_for__a_nested_container()
        {
            var container = new Container(x =>
            {
                x.For<IEngine>().Use<Hemi>().Named("The Hemi");

                x.For<IEngine>().Add<VEight>().Singleton().Named("V8");
                x.For<IEngine>().Add<FourFiftyFour>().AlwaysUnique();
                x.For<IEngine>().Add<StraightSix>().LifecycleIs<ThreadLocalStorageLifecycle>();

                x.For<IEngine>().Add(() => new Rotary()).Named("Rotary");
                x.For<IEngine>().Add(c => c.GetInstance<PluginElectric>());

                x.For<IEngine>().Add(new InlineFour());
            });

            Debug.WriteLine(container.GetNestedContainer().WhatDoIHave());
        }

        [Test]
        public void display_one_service_for__a_profile_container()
        {
            var container = new Container(x =>
            {
                x.For<IEngine>().Use<Hemi>().Named("The Hemi");

                x.For<IEngine>().Add<VEight>().Singleton().Named("V8");
                x.For<IEngine>().Add<FourFiftyFour>().AlwaysUnique();
                x.For<IEngine>().Add<StraightSix>().LifecycleIs<ThreadLocalStorageLifecycle>();

                x.For<IEngine>().Add(() => new Rotary()).Named("Rotary");
                x.For<IEngine>().Add(c => c.GetInstance<PluginElectric>());

                x.For<IEngine>().Add(new InlineFour());

                x.Profile("Blue", blue => { blue.For<IEngine>().Use<FourFiftyFour>().Named("Gas Guzzler"); });
            });

            Debug.WriteLine(container.GetProfile("Blue").WhatDoIHave());
        }

        [Test]
        public void filter_by_assembly()
        {
            var container = new Container(x =>
            {
                x.For<IEngine>().Use<Hemi>().Named("The Hemi");

                x.For<IEngine>().Add<VEight>().Singleton().Named("V8");
                x.For<IEngine>().Add<FourFiftyFour>().AlwaysUnique();
                x.For<IEngine>().Add<StraightSix>().LifecycleIs<ThreadLocalStorageLifecycle>();

                x.For<IEngine>().Add(() => new Rotary()).Named("Rotary");
                x.For<IEngine>().Add(c => c.GetInstance<PluginElectric>());

                x.For<IEngine>().Add(new InlineFour());

                x.For<IWidget>().Use<AWidget>();
            });

            // SAMPLE: whatdoihave-assembly
            Debug.WriteLine(container.WhatDoIHave(assembly: typeof (IWidget).Assembly));
            // ENDSAMPLE
        }

        [Test]
        public void filtering_examples()
        {
            // SAMPLE: whatdoihave-filtering
            var container = new Container();

            // Filter by the Assembly of the Plugin Type
            var byAssembly = container.WhatDoIHave(assembly: typeof (IWidget).Assembly);

            // Only report on the specified Plugin Type
            var byPluginType = container.WhatDoIHave(typeof (IWidget));

            // Filter to Plugin Type's in the named namespace
            // The 'IsInNamespace' test will include child namespaces
            var byNamespace = container.WhatDoIHave(@namespace: "StructureMap.Testing.Widget");

            // Filter by a case insensitive string.Contains() match
            // against the Plugin Type name
            var byType = container.WhatDoIHave(typeName: "Widget");
            // ENDSAMPLE
        }

        [Test]
        public void filter_by_plugin_type()
        {
            var container = new Container(x =>
            {
                x.For<IEngine>().Use<Hemi>().Named("The Hemi");

                x.For<IEngine>().Add<VEight>().Singleton().Named("V8");
                x.For<IEngine>().Add<FourFiftyFour>().AlwaysUnique();
                x.For<IEngine>().Add<StraightSix>().LifecycleIs<ThreadLocalStorageLifecycle>();

                x.For<IEngine>().Add(() => new Rotary()).Named("Rotary");
                x.For<IEngine>().Add(c => c.GetInstance<PluginElectric>());

                x.For<IEngine>().Add(new InlineFour());

                x.For<IWidget>().Use<AWidget>();
            });

            // SAMPLE: whatdoihave-plugintype
            Debug.WriteLine(container.WhatDoIHave(typeof (IWidget)));
            // ENDSAMPLE
        }

        [Test]
        public void filter_by_type_name()
        {
            var container = new Container(x =>
            {
                x.For<IEngine>().Use<Hemi>().Named("The Hemi");

                x.For<IEngine>().Add<VEight>().Singleton().Named("V8");
                x.For<IEngine>().Add<FourFiftyFour>().AlwaysUnique();
                x.For<IEngine>().Add<StraightSix>().LifecycleIs<ThreadLocalStorageLifecycle>();

                x.For<IEngine>().Add(() => new Rotary()).Named("Rotary");
                x.For<IEngine>().Add(c => c.GetInstance<PluginElectric>());

                x.For<IEngine>().Add(new InlineFour());

                x.For<IWidget>().Use<AWidget>();

                x.For<AWidget>().Use<AWidget>();
            });

            // SAMPLE: whatdoihave-type
            Debug.WriteLine(container.WhatDoIHave(typeName: "Widget"));
            // ENDSAMPLE
        }

        [Test]
        public void filter_by_namespace()
        {
            var container = new Container(x =>
            {
                x.For<IEngine>().Use<Hemi>().Named("The Hemi");

                x.For<IEngine>().Add<VEight>().Singleton().Named("V8");
                x.For<IEngine>().Add<FourFiftyFour>().AlwaysUnique();
                x.For<IEngine>().Add<StraightSix>().LifecycleIs<ThreadLocalStorageLifecycle>();

                x.For<IEngine>().Add(() => new Rotary()).Named("Rotary");
                x.For<IEngine>().Add(c => c.GetInstance<PluginElectric>());

                x.For<IEngine>().Add(new InlineFour());

                x.For<IWidget>().Use<AWidget>();

                x.For<AWidget>().Use<AWidget>();
            });

            // SAMPLE: whatdoihave-namespace
            Debug.WriteLine(container.WhatDoIHave(@namespace: "System"));
            // ENDSAMPLE
        }
    }

    public interface IAutomobile
    {
    }

    public interface IEngine
    {
    }

    public class NamedEngine : IEngine
    {
        private readonly string _name;

        public NamedEngine(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    public class VEight : IEngine
    {
    }

    public class StraightSix : IEngine
    {
    }

    public class Hemi : IEngine
    {
    }

    public class FourFiftyFour : IEngine
    {
    }

    public class VTwelve : IEngine
    {
    }

    public class Rotary : IEngine
    {
    }

    public class PluginElectric : IEngine
    {
    }

    public class InlineFour : IEngine
    {
        public override string ToString()
        {
            return "I'm an inline 4!";
        }
    }
}