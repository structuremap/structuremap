using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_354_bidirectional_dep_bug_under_heavy_threading
    {
        private const String PearCode = "P";

        [Test]
        public void do_not_blow_up()
        {
            var container = new Container(_ =>
            {
                _.For<IFruit>().Use<Pear>().Named(PearCode).AlwaysUnique();
                _.For<FruitProvider>().Use<FruitProvider>();
                _.For<ISomething>().Use<Something>().AlwaysUnique();
            });
            var tasks = new List<Task>(2000);
            var fruitProvider = container.GetInstance<FruitProvider>();

            for (var i = 0; i < tasks.Capacity; i++)
            {
                var t = Task.Factory.StartNew(() => { fruitProvider.Get(PearCode); });
                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
        }
    }

    public class FruitProvider
    {
        private readonly Func<String, IFruit> _factory;

        public FruitProvider(Func<String, IFruit> factory)
        {
            _factory = factory;
        }

        public IFruit Get(String name)
        {
            return _factory(name);
        }
    }

    public interface IFruit
    {
        String Name { get; }
    }

    public class Pear : IFruit
    {
        private readonly ISomething _something;

        public Pear(ISomething something)
        {
            // Please note that this is NOT needed to cause the issue, but will increase the number of times the issue occurs
            _something = something;
        }

        public String Name
        {
            get { return "Pear"; }
        }
    }

    public interface ISomething
    {
        Object Something2 { get; }
    }

    public class Something : ISomething
    {
        public Object Something2
        {
            get { return DateTime.UtcNow; }
        }
    }
}

