using System;
using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class GenericConnectionScannerTester
    {
        [Test]
        public void can_find_the_closed_finders()
        {
            var container = new Container(x => x.Scan(o =>
            {
                o.TheCallingAssembly();
                o.ConnectImplementationsToTypesClosing(typeof(IFinder<>));
            }));
            container.GetInstance<IFinder<string>>().ShouldBeOfType<StringFinder>();
            container.GetInstance<IFinder<int>>().ShouldBeOfType<IntFinder>();
            container.GetInstance<IFinder<double>>().ShouldBeOfType<DoubleFinder>();
        }

        [Test, ExpectedException(typeof (ApplicationException))]
        public void fails_on_closed_type()
        {
            new GenericConnectionScanner(typeof (double));
        }

        [Test]
        public void can_configure_plugin_families_via_dsl()
        {
            var container = new Container(registry => registry.Scan(x =>
            {
                x.TheCallingAssembly();
                x.ConnectImplementationsToTypesClosing(typeof(IFinder<>)).OnAddedPluginTypes(t => t.Singleton());
            }));

            var firstStringFinder = container.GetInstance<IFinder<string>>().ShouldBeOfType<StringFinder>();
            var secondStringFinder = container.GetInstance<IFinder<string>>().ShouldBeOfType<StringFinder>();
            secondStringFinder.ShouldBeTheSameAs(firstStringFinder);

            var firstIntFinder = container.GetInstance<IFinder<int>>().ShouldBeOfType<IntFinder>();
            var secondIntFinder = container.GetInstance<IFinder<int>>().ShouldBeOfType<IntFinder>();
            secondIntFinder.ShouldBeTheSameAs(firstIntFinder);
        }
    }

    public interface IFinder<T>
    {
    }

    public class StringFinder : IFinder<string>
    {
    }

    public class IntFinder : IFinder<int>
    {
    }

    public class DoubleFinder : IFinder<double>
    {
    }
}