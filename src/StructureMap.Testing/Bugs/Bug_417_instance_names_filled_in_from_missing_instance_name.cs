using Shouldly;
using StructureMap.Testing.Widget;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_417_instance_names_filled_in_from_missing_instance_name
    {
        [Fact]
        public void should_add_the_instance_name_in_missing_name_usage()
        {
            var container = new Container(_ =>
            {
                _.For<ColorRule>().MissingNamedInstanceIs.ConstructedBy(c => new ColorRule(c.RequestedName));

                _.For<ColorRule>().OnCreationForAll("Just for testing", r => Debug.WriteLine(r.Color), i =>
                {
                    Assert.True(i.Name == "Red" || i.Name == "Green" || i.Name == "Blue",
                        "Still using a Guid for the Id! --> " + i.Name);

                    return true;
                });
            });

            container.GetInstance<ColorRule>("Red").ShouldNotBeNull();
            container.GetInstance<ColorRule>("Green").ShouldNotBeNull();
            container.GetInstance<ColorRule>("Blue").ShouldNotBeNull();
        }
    }
}