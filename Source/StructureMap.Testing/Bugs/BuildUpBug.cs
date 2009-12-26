using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class StructureMapTests
    {
        [Test]
        public void Test()
        {
            ObjectFactory.Initialize(x =>
            {
                x.UseDefaultStructureMapConfigFile = false;

                x.ForConcreteType<SomeDbRepository>().Configure.
                  WithCtorArg("connectionString").EqualTo("some connection string");

                //x.ForConcreteType<SomeWebPage>().Configure.
                //  SetterDependency<SomeDbRepository>().Is<SomeDbRepository>();

                x.SetAllProperties(o => o.OfType<SomeDbRepository>());
            });

            SomeDbRepository dbRepository =
              ObjectFactory.GetInstance<SomeDbRepository>();

            SomeWebPage webPage = new SomeWebPage();

            ObjectFactory.BuildUp(webPage);

            webPage.DbRepository.ConnectionString.ShouldEqual("some connection string");
        }
    }

    public class SomeDbRepository
    {
        public SomeDbRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
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
