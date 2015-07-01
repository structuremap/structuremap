using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing.Examples
{
    public class ClassThatHasConnection
    {
        public string ConnectionString { get; set; }
    }

    [TestFixture]
    public class demo_the_BuildUp
    {
        [Test]
        public void push_in_a_string_property()
        {
            // There is a limitation to this.  As of StructureMap 2.5.2,
            // you can only use the .WithProperty().Is() syntax
            // for BuildUp()
            // SetProperty() will not work at this time.
            var container = new Container(x => {
                x.ForConcreteType<ClassThatHasConnection>().Configure
                    .Setter(o => o.ConnectionString).Is("connect1");
            });

            var @class = new ClassThatHasConnection();
            container.BuildUp(@class);

            @class.ConnectionString.ShouldBe("connect1");
        }
    }
}