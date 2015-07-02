using System.Threading;
using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    // SAMPLE: thread-local-storage
    [TestFixture]
    public class ThreadLocalStorageLifecycleTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _lifecycle = new ThreadLocalStorageLifecycle();

            container = new Container(x => x.For<Rule>(Lifecycles.ThreadLocal).Use(() => new ColorRule("Red")));
        }

        #endregion

        private ThreadLocalStorageLifecycle _lifecycle;
        private ColorRule _rule1;
        private ColorRule _rule2;
        private ColorRule _rule3;
        private Container container;


        private void findRule1()
        {
            _rule1 = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();

            var rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            _rule1.ShouldBeTheSameAs(rule);
        }

        private void findRule2()
        {
            _rule2 = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();

            var rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            _rule2.ShouldBeTheSameAs(rule);
        }

        private void findRule3()
        {
            _rule3 = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();

            var rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            _rule3.ShouldBeTheSameAs(rule);

            rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            _rule3.ShouldBeTheSameAs(rule);

            rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            _rule3.ShouldBeTheSameAs(rule);

            rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            _rule3.ShouldBeTheSameAs(rule);
        }

        [Test]
        public void object_has_been_created()
        {
            container.Model.For<Rule>().Default.ObjectHasBeenCreated().ShouldBeFalse();
            var r1 = container.GetInstance<Rule>();
            container.Model.For<Rule>().Default.ObjectHasBeenCreated().ShouldBeTrue();
        }

        [Test]
        public void FindUniqueInstancePerThread()
        {
            var t1 = new Thread(findRule1);
            var t2 = new Thread(findRule2);
            var t3 = new Thread(findRule3);

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();

            _rule1.ShouldNotBeTheSameAs(_rule2);
            _rule1.ShouldNotBeTheSameAs(_rule3);
            _rule2.ShouldNotBeTheSameAs(_rule3);
            (_rule1.ID != _rule2.ID).ShouldBeTrue();
            (_rule1.ID != _rule3.ID).ShouldBeTrue();
            (_rule2.ID != _rule3.ID).ShouldBeTrue();
        }
    }

    // ENDSAMPLE
}