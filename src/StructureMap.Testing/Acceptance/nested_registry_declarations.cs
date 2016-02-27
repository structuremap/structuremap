using System.Linq;

using NUnit.Framework;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class nested_registry_declarations
    {
        [Test]
        public void get_registrations_from_all_included_registries()
        {
            var container = new Container(new ARegistry());

            container.GetAllInstances<IWidget>()
                .OrderBy(x => x.GetType().Name)
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof (AWidget), typeof (BWidget), typeof (CWidget),
                    typeof (DefaultWidget));
        }

        [Test]
        public void get_registrations_from_all_included_registries_through_configure()
        {
            var container = new Container();
            container.Configure(x => x.IncludeRegistry<ARegistry>());

            container.GetAllInstances<IWidget>()
                .OrderBy(x => x.GetType().Name)
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof (AWidget), typeof (BWidget), typeof (CWidget),
                    typeof (DefaultWidget));
        }

        [Test]
        public void get_instance_gets_most_recently_registered_type()
        {
            using (var container = new Container(new SecondRegistry()))
            {
                container.GetInstance<IWidget>()
                    .IsType<BWidget>();
            }
        }
    }

    public class FirstRegistry : Registry
    {
        public FirstRegistry()
        {
            For<IWidget>()
                .Use<AWidget>();
        }
    }

    public class SecondRegistry : Registry
    {
        public SecondRegistry()
        {
            IncludeRegistry<FirstRegistry>();

            For<IWidget>()
                .Use<BWidget>();
        }
    }

    public class ARegistry : Registry
    {
        public ARegistry()
        {
            For<IWidget>().Add<AWidget>();

            IncludeRegistry<BRegistry>();
            IncludeRegistry<CRegistry>();
        }
    }

    public class BRegistry : Registry
    {
        public BRegistry()
        {
            For<IWidget>().Add<BWidget>();

            IncludeRegistry<DefaultRegistry>();
        }
    }

    public class CRegistry : Registry
    {
        public CRegistry()
        {
            For<IWidget>().Add<CWidget>();
        }
    }

    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            For<IWidget>().Add<DefaultWidget>();
        }
    }
}