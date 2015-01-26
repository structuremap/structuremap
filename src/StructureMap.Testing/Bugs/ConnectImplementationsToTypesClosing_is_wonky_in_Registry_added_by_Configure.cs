using System.Linq;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class ConnectImplementationsToTypesClosing_is_wonky_in_Registry_added_by_Configure
    {
        [Test]
        public void has_the_correct_number_by_initialize()
        {
            var container = Container.For<BookRegistry>();
            container.GetAllInstances<IBook<SciFi>>().Count().ShouldEqual(1);
        }

        [Test]
        public void has_the_correct_number_by_configure()
        {
            var container = new Container();
            container.Configure(x => x.AddRegistry<BookRegistry>());
            container.GetAllInstances<IBook<SciFi>>().Count().ShouldEqual(1);
        }
    }

    public class BookRegistry : Registry
    {
        public BookRegistry()
        {
            Scan(x => {
                x.TheCallingAssembly();
                x.ConnectImplementationsToTypesClosing(typeof (IBook<>));
            });
        }
    }

    public interface IBook<T>{}


    public class SciFi{}
    public class SciFiBook : IBook<SciFi>{}

    public class Fantasy {}
    public class FantasyBook : IBook<Fantasy>{}
}