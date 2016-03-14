using Shouldly;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class StructureMapTests
    {
        [Fact]
        public void Test()
        {
            var container = new Container(x =>
            {
                x.ForConcreteType<SomeDbRepository>().Configure.
                    Ctor<string>("connectionString").Is("some connection string");

                //x.ForConcreteType<SomeWebPage>().Configure.
                //  SetterDependency<SomeDbRepository>().Is<SomeDbRepository>();

                x.Policies.SetAllProperties(o => o.OfType<SomeDbRepository>());
            });

            var webPage = new SomeWebPage();

            container.BuildUp(webPage);

            webPage.DbRepository.ConnectionString.ShouldBe("some connection string");
        }
    }

    public class SomeDbRepository
    {
        public SomeDbRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        // ...
    }

    public class SomeWebPage
    {
        public SomeDbRepository DbRepository { get; set; }

        // ...
    }
}