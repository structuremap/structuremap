using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class MemoryLeak_bug_289
    {
        [Test]
        public void no_longer_tracks_nested_containers_in_parent_scope()
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

            var task1 = Task.Factory.StartNew(() => Thread.Sleep(10));
            var task2 = Task.Factory.StartNew(() => Thread.Sleep(10));
            var task3 = Task.Factory.StartNew(() => Thread.Sleep(10));
            var task4 = Task.Factory.StartNew(() => Thread.Sleep(10));

            for (var i = 0; i < 5; i++)
            {
                task1 = task1.ContinueWith(t =>
                {
                    using (var nested = container.GetNestedContainer())
                    {
                        Thread.Sleep(10);

                        var lazy = nested.GetInstance<Lazy<IContainer>>();

                        lazy.Value.ShouldBeTheSameAs(nested);

                        var guy = nested.GetInstance<GuyWithContainer>();
                        guy.Container.ShouldBeTheSameAs(nested);
                        container.Model.Pipeline.Singletons.Count.ShouldBe(0);
                    }
                });

                task2 = task2.ContinueWith(t =>
                {
                    using (var nested = container.GetNestedContainer())
                    {
                        Thread.Sleep(10);

                        var lazy = nested.GetInstance<Lazy<IContainer>>();

                        lazy.Value.ShouldBeTheSameAs(nested);

                        var guy = nested.GetInstance<GuyWithContainer>();
                        guy.Container.ShouldBeTheSameAs(nested);
                        container.Model.Pipeline.Singletons.Count.ShouldBe(0);
                    }
                });

                task3 = task3.ContinueWith(t =>
                {
                    using (var nested = container.GetNestedContainer())
                    {
                        var lazy = nested.GetInstance<Lazy<IContainer>>();

                        lazy.Value.ShouldBeTheSameAs(nested);

                        var guy = nested.GetInstance<GuyWithContainer>();
                        guy.Container.ShouldBeTheSameAs(nested);

                        container.Model.Pipeline.Singletons.Count.ShouldBe(0);
                    }
                });

                task4 = task4.ContinueWith(t =>
                {
                    using (var nested = container.GetNestedContainer())
                    {
                        var lazy = nested.GetInstance<Lazy<IContainer>>();

                        lazy.Value.ShouldBeTheSameAs(nested);

                        var guy = nested.GetInstance<GuyWithContainer>();
                        guy.Container.ShouldBeTheSameAs(nested);
                        container.Model.Pipeline.Singletons.Count.ShouldBe(0);
                    }
                });
            }

            Task.WaitAll(task1, task2, task3, task4);

            if (task1.IsFaulted) throw task1.Exception;
            if (task2.IsFaulted) throw task2.Exception;
            if (task3.IsFaulted) throw task3.Exception;
            if (task4.IsFaulted) throw task4.Exception;
        }

        public class GuyWithContainer
        {
            private readonly IContainer _container;

            public GuyWithContainer(IContainer container)
            {
                _container = container;
            }

            public IContainer Container
            {
                get { return _container; }
            }
        }
    }
}