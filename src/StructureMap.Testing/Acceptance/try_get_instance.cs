using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class try_get_instance
    {
        [Test]
        public void complete_miss()
        {
            var container = new Container();
            container.TryGetInstance<IFancy>()
                .ShouldBeNull();
        }

        [Test]
        public void force_the_cached_miss_behavior()
        {
            var container = new Container();
            container.TryGetInstance<IFancy>().ShouldBeNull();
            container.TryGetInstance<IFancy>().ShouldBeNull();
            container.TryGetInstance<IFancy>().ShouldBeNull();
            container.TryGetInstance<IFancy>().ShouldBeNull();
            container.TryGetInstance<IFancy>().ShouldBeNull();
        }

        [Test]
        public void miss_then_hit_after_configure_with_policy_change()
        {
            var container = new Container();
            container.TryGetInstance<IFancy>().ShouldBeNull();

            container.Configure(_ => _.Policies.OnMissingFamily<FancyFamily>());

            container.TryGetInstance<IFancy>()
                .ShouldBeOfType<Very>();
        }

        [Test]
        public void miss_then_hit_after_configure_adds_it()
        {
            var container = new Container();
            container.TryGetInstance<IFancy>().ShouldBeNull();

            container.Configure(_ =>
            {
                _.For<IFancy>().Use(new NotReally());
            });

            container.TryGetInstance<IFancy>()
                .ShouldBeOfType<NotReally>();
        }

        
    }

    public interface IFancy
    {
        
    }

    public class Very : IFancy { }
    public class NotReally : IFancy { }

    public class FancyFamily : IFamilyPolicy
    {
        public PluginFamily Build(Type type)
        {
            if (type != typeof (IFancy)) return null;

            var family = new PluginFamily(type);
            family.SetDefault(new SmartInstance<Very>());

            return family;
        }

        public bool AppliesToHasFamilyChecks
        {
            get { return true; }
        }
    }
}