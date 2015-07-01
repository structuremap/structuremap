using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Testing.Acceptance;
using AWidget = StructureMap.Testing.Widget.AWidget;
using IWidget = StructureMap.Testing.Widget.IWidget;

namespace StructureMap.Testing.Examples.Resolving
{
    [TestFixture]
    public class SimpleScenarios
    {
        // SAMPLE: GetInstance
        [Test]
        public void get_the_default_instance()
        {
            var container = new Container(x => {
                x.For<IWidget>().Use<AWidget>();
            });

            container.GetInstance<IWidget>()
                .ShouldBeOfType<AWidget>();

            // or

            container.GetInstance(typeof(IWidget))
                .ShouldBeOfType<AWidget>();
        }
        // ENDSAMPLE

        // SAMPLE: GetInstance-by-name
        [Test]
        public void get_a_named_instance()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Add<AWidget>().Named("A");
                x.For<IWidget>().Add<BWidget>().Named("B");
                x.For<IWidget>().Add<CWidget>().Named("C");
            });

            container.GetInstance<IWidget>("A").ShouldBeOfType<AWidget>();
            container.GetInstance<IWidget>("B").ShouldBeOfType<BWidget>();
            container.GetInstance<IWidget>("C").ShouldBeOfType<CWidget>();

            // or

            container.GetInstance(typeof(IWidget), "A").ShouldBeOfType<AWidget>();
            container.GetInstance(typeof(IWidget), "B").ShouldBeOfType<BWidget>();
            container.GetInstance(typeof(IWidget), "C").ShouldBeOfType<CWidget>();
        }
        // ENDSAMPLE

        // SAMPLE: get-all-instances
        [Test]
        public void get_all_instances()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Add<AWidget>().Named("A");
                x.For<IWidget>().Add<BWidget>().Named("B");
                x.For<IWidget>().Add<CWidget>().Named("C");
            });

            container.GetAllInstances<IWidget>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));

            // or

            container.GetAllInstances(typeof(IWidget))
                .OfType<IWidget>() // returns an IEnumerable, so I'm casting here
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));
        }
        // ENDSAMPLE


        public interface IWidget{}
        public class AWidget : IWidget{}
        public class BWidget : IWidget{}
        public class CWidget : IWidget{}
    }
}