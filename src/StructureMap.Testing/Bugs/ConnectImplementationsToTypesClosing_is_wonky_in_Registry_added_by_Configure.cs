using Shouldly;
using StructureMap.Graph;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class ConnectImplementationsToTypesClosing_is_wonky_in_Registry_added_by_Configure
    {
        [Fact]
        public void has_the_correct_number_by_initialize()
        {
            var container = Container.For<BookRegistry>();
            container.GetAllInstances<IBook<SciFi>>().Count().ShouldBe(1);
        }

        [Fact]
        public void has_the_correct_number_by_configure()
        {
            var container = new Container();
            container.Configure(x => x.AddRegistry<BookRegistry>());
            container.GetAllInstances<IBook<SciFi>>().Count().ShouldBe(1);
        }

        [Fact]
        public void Bug_525_interception_plus_connect_implementations()
        {
            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.Exclude(type => type == typeof(DustCover<>));
                    x.ConnectImplementationsToTypesClosing(typeof(IBook<>));
                });

                _.For(typeof(IBook<>)).DecorateAllWith(typeof(DustCover<>));
            });

            container.GetInstance<IBook<SciFi>>().ShouldBeOfType<DustCover<SciFi>>()
                .Book.ShouldBeOfType<SciFiBook>();
        }
    }

    public class BookRegistry : Registry
    {
        public BookRegistry()
        {
            Scan(x =>
            {
                x.Exclude(type => type == typeof(DustCover<>));
                x.TheCallingAssembly();
                x.ConnectImplementationsToTypesClosing(typeof(IBook<>));
            });
        }
    }


    public class DustCover<T> : IBook<T>
    {
        public IBook<T> Book { get; }

        public DustCover(IBook<T> book)
        {
            Book = book;
        }
    }

    public interface IBook<T>
    {
    }

    public class SciFi
    {
    }

    public class SciFiBook : IBook<SciFi>
    {
    }

    public class Fantasy
    {
    }

    public class FantasyBook : IBook<Fantasy>
    {
    }
}