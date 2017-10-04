using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.TypeRules;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class custom_registration_convention
    {
        // SAMPLE: custom-registration-composite
        public interface ISomething
        {
            IEnumerable<string> GetNames();
        }

        public class One : ISomething
        {
            public IEnumerable<string> GetNames() => new[] { "one" };
        }

        public class Two : ISomething
        {
            public IEnumerable<string> GetNames() => new[] { "two" };
        }

        public class SomethingComposite : ISomething
        {
            private readonly IEnumerable<ISomething> _others;

            public SomethingComposite(IEnumerable<ISomething> others)
            {
                _others = others;
            }

            public IEnumerable<string> GetNames() => _others.SelectMany(other => other.GetNames());
        }

        // Custom IRegistrationConvention
        public class CompositeDecorator<TComposite, TDependents> : IRegistrationConvention
            where TComposite : TDependents
        {
            public void ScanTypes(TypeSet types, Registry registry)
            {
                var dependents = types
                    .FindTypes(TypeClassification.Concretes)
                    .Where(t => t.CanBeCastTo<TDependents>() && t.HasConstructors())
                    .Where(t => t != typeof(TComposite))
                    .ToList();

                registry.For<TDependents>()
                    .Use<TComposite>()
                    .EnumerableOf<TDependents>().Contains(x => dependents.ForEach(t => x.Type(t)));
            }
        }

        [Fact]
        public void use_custom_registration_convention()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.AssemblyContainingType<ISomething>();
                    x.Convention<CompositeDecorator<SomethingComposite, ISomething>>();
                });
            });

            var composite = container.GetInstance<ISomething>();

            composite.ShouldBeOfType<SomethingComposite>();
            composite.GetNames().ShouldBe(new[] { "one", "two" }, ignoreOrder: true);
        }

        // ENDSAMPLE
    }
}
