using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug456_AlwaysUnique_Tests
    {
        public class StructureMapAlwaysUniqueBug
        {
            [Test]
            public void Should_resolve()
            {
                using (var container = new Container(_ =>
                {
                    _.Scan(scan =>
                    {
                        scan.TheCallingAssembly();
                        scan.WithDefaultConventions();
                        scan.Convention<AlwaysUniqueConvention>();
                    });
                    _.For<IPalette>().Use<Palette>()
                        .AlwaysUnique()
                        .EnumerableOf<IColor>()
                        .Contains(x =>
                        {
                            x.Type<Red>();
                            x.Type<Blue>();
                        });
                }))
                {
                    Debug.WriteLine(container.Model.For<IPalette>().Default.DescribeBuildPlan());

                    var palette = container.GetInstance<IPalette>();
                    palette.ShouldNotBeNull();
                }
            }

            public interface IPalette
            {
                IEnumerable<IColor> Colors { get; }
            }

            public class Palette : IPalette
            {
                public IEnumerable<IColor> Colors { get; }

                public Palette(IEnumerable<IColor> colors)
                {
                    Colors = colors;
                }
            }

            public interface IColor { }

            public class Blue : IColor { }
            public class Red : IColor { }

            public class AlwaysUniqueConvention : IRegistrationConvention
            {
                public void ScanTypes(TypeSet types, Registry registry)
                {
                    foreach (var type in types.AllTypes())
                    {
                        registry.For(type).LifecycleIs(new UniquePerRequestLifecycle());
                    }
                }
            }

        }
    }
}