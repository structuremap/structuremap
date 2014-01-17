using System;
using System.Linq;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class ejecting_instances
    {
        [Test]
        public void ejecting_all_from_a_family()
        {
            var container = new Container(x => {
                x.For<DisposedGuy>().Singleton().AddInstances(guys => {
                    guys.Type<DisposedGuy>().Named("A");
                    guys.Type<DisposedGuy>().Named("B");
                    guys.Type<DisposedGuy>().Named("C");
                });
            });

            // Fetch all the singleton objects
            var guyA = container.GetInstance<DisposedGuy>("A");
            var guyB = container.GetInstance<DisposedGuy>("B");
            var guyC = container.GetInstance<DisposedGuy>("C");
        
            container.Model.EjectAndRemove<DisposedGuy>();

            // All the singleton instances should be disposed
            // as they are removed from the Container
            guyA.WasDisposed.ShouldBeTrue();
            guyB.WasDisposed.ShouldBeTrue();
            guyC.WasDisposed.ShouldBeTrue();

            // And now the container no longer has any
            // registrations for DisposedGuy
            container.GetAllInstances<DisposedGuy>()
                .Any().ShouldBeFalse();
        }

        [Test]
        public void eject_and_remove_a_single_instance()
        {
            var container = new Container(x =>
            {
                x.For<DisposedGuy>().Singleton().AddInstances(guys =>
                {
                    guys.Type<DisposedGuy>().Named("A");
                    guys.Type<DisposedGuy>().Named("B");
                    guys.Type<DisposedGuy>().Named("C");
                });
            });

            // Fetch all the singleton objects
            var guyA = container.GetInstance<DisposedGuy>("A");
            var guyB = container.GetInstance<DisposedGuy>("B");
            var guyC = container.GetInstance<DisposedGuy>("C");

            // Eject *only* guyA
            container.Model.For<DisposedGuy>().EjectAndRemove("A");

            // Only guyA should be disposed
            guyA.WasDisposed.ShouldBeTrue();
            guyB.WasDisposed.ShouldBeFalse();
            guyC.WasDisposed.ShouldBeFalse();

            // Now, see that guyA is really gone
            container.GetAllInstances<DisposedGuy>()
                .ShouldHaveTheSameElementsAs(guyB, guyC);
        }

        [Test]
        public void eject_with_custom_lifecycle()
        {
            CustomLifecycle.Cache.DisposeAndClear();

            var customLifecycle = new CustomLifecycle();

            var container = new Container(x => {
                x.For<DisposedGuy>().LifecycleIs(customLifecycle).AddInstances(guys =>
                {
                    guys.Type<DisposedGuy>().Named("A");
                    guys.Type<DisposedGuy>().Named("B");
                    guys.Type<DisposedGuy>().Named("C");
                });
            });

            // Fetch all the singleton objects
            var guyA = container.GetInstance<DisposedGuy>("A");
            var guyB = container.GetInstance<DisposedGuy>("B");
            var guyC = container.GetInstance<DisposedGuy>("C");

            container.Model.For<DisposedGuy>().EjectAndRemove("A");

            guyA.WasDisposed.ShouldBeTrue();

            container.Model.EjectAndRemove<DisposedGuy>();

            guyB.WasDisposed.ShouldBeTrue();
            guyC.WasDisposed.ShouldBeTrue();

            // And now the container no longer has any
            // registrations for DisposedGuy
            container.GetAllInstances<DisposedGuy>()
                .Any().ShouldBeFalse();
        }

        [Test]
        public void eject_and_remove_will_eject_from_a_per_instance_lifecycle()
        {
            CustomLifecycle.Cache.DisposeAndClear();

            var container = new Container(x =>
            {
                x.For<DisposedGuy>().AddInstances(guys =>
                {
                    guys.Type<DisposedGuy>().Named("A").SetLifecycleTo(new CustomLifecycle());
                });
            });

            var guyA = container.GetInstance<DisposedGuy>("A");
            CustomLifecycle.Cache.Count.ShouldEqual(1);

            container.Model.For<DisposedGuy>().EjectAndRemove("A");

            guyA.WasDisposed.ShouldBeTrue();

            CustomLifecycle.Cache.Count.ShouldEqual(0);
        }
    }

    // SAMPLE: DisposedGuy
    public class DisposedGuy : IDisposable
    {
        public bool WasDisposed;

        public void Dispose()
        {
            WasDisposed = true;
        }
    }
    // ENDSAMPLE

    public class CustomLifecycle : ILifecycle
    {
        public static LifecycleObjectCache Cache = new LifecycleObjectCache();

        public string Description
        {
            get
            {
                return "Custom";
            }
        }

        public void EjectAll(ILifecycleContext context)
        {
            
        }

        public IObjectCache FindCache(ILifecycleContext context)
        {
            return Cache;
        }
    }
}