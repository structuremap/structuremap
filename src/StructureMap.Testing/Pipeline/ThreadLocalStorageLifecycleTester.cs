using System.Threading;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ThreadLocalStorageLifecycleTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _lifecycle = new ThreadLocalStorageLifecycle();

            container =
                new Container(
                    x =>
                    {
                        x.For<Rule>(InstanceScope.ThreadLocal).Use(
                            () => new ColorRule("Red"));
                    });
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
            Assert.AreSame(_rule1, rule);
        }

        private void findRule2()
        {
            _rule2 = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();

            var rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            Assert.AreSame(_rule2, rule);
        }

        private void findRule3()
        {
            _rule3 = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();

            var rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            Assert.AreSame(_rule3, rule);

            rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            Assert.AreSame(_rule3, rule);

            rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            Assert.AreSame(_rule3, rule);

            rule = container.GetInstance<Rule>().ShouldBeOfType<ColorRule>();
            Assert.AreSame(_rule3, rule);
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

            Assert.AreNotSame(_rule1, _rule2);
            Assert.AreNotSame(_rule1, _rule3);
            Assert.AreNotSame(_rule2, _rule3);
            Assert.IsTrue(_rule1.ID != _rule2.ID);
            Assert.IsTrue(_rule1.ID != _rule3.ID);
            Assert.IsTrue(_rule2.ID != _rule3.ID);
        }
    }

	[TestFixture]
	public class ThreadLocalStorageLifecycleTester_with_nested_containers
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			lifecycle = new ThreadLocalStorageLifecycle();

			container =
				new Container(
					x =>
					{
						x.For<Rule>().HybridHttpOrThreadLocalScoped().Use(() => new ColorRule("Red"));
					});

			nestedContainer = container.GetNestedContainer();

			int count = 0;
			nestedContainer.Configure(x =>
			{
				x.For<Rule>().Transient().Use(() =>
				{
					count++;
					return new ColorRule("Red" + count);
				});
			});
		}

		#endregion

		private Container container;
		private IContainer nestedContainer;
		private ThreadLocalStorageLifecycle lifecycle;

		[Test]
		public void Overrides_Lifecycle()
		{
			var rule1 = container.GetInstance<Rule>();
			var rule2 = nestedContainer.GetInstance<Rule>();

			rule1.ShouldNotBeTheSameAs(rule2);
		}

		[Test]
		public void Does_not_cache_item()
		{
			int initial = lifecycle.FindCache().Count;

			nestedContainer.GetInstance<Rule>();

			int after = lifecycle.FindCache().Count;

			after.ShouldEqual(initial);
		}
	}

}