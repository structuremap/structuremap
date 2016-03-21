using Shouldly;
using StructureMap.Graph;
using System;
using System.Diagnostics;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class GenericConnectionScannerTester
    {
        [Fact]
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

        [Fact]
        public void single_class_can_close_multiple_open_interfaces()
        {
            var container = new Container(x => x.Scan(o =>
            {
                o.TheCallingAssembly();
                o.ConnectImplementationsToTypesClosing(typeof(IFinder<>));
                o.ConnectImplementationsToTypesClosing(typeof(IFindHandler<>));
            }));
            container.GetInstance<IFinder<decimal>>().ShouldBeOfType<SrpViolation>();
            container.GetInstance<IFindHandler<DateTime>>().ShouldBeOfType<SrpViolation>();
        }

        [Fact]
        public void single_class_can_close_the_same_open_interface_multiple_times()
        {
            var container = new Container(x => x.Scan(o =>
            {
                o.TheCallingAssembly();
                o.ConnectImplementationsToTypesClosing(typeof(IFinder<>));
            }));
            container.GetInstance<IFinder<byte>>().ShouldBeOfType<SuperFinder>();
            container.GetInstance<IFinder<char>>().ShouldBeOfType<SuperFinder>();
            container.GetInstance<IFinder<uint>>().ShouldBeOfType<SuperFinder>();
        }

        [Fact]
        public void fails_on_closed_type()
        {
            Exception<InvalidOperationException>.ShouldBeThrownBy(
                () => { new GenericConnectionScanner(typeof(double)); });
        }

        [Fact]
        public void can_configure_plugin_families_via_dsl()
        {
            var container = new Container(registry => registry.Scan(x =>
            {
                x.TheCallingAssembly();
                x.ConnectImplementationsToTypesClosing(typeof(IFinder<>)).OnAddedPluginTypes(t => t.Singleton());
            }));

            Debug.WriteLine(container.WhatDoIHave());

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

    public interface IFindHandler<T>
    {
    }

    public class SrpViolation : IFinder<decimal>, IFindHandler<DateTime>
    {
    }

    public class SuperFinder : IFinder<byte>, IFinder<char>, IFinder<uint>
    {
    }
}