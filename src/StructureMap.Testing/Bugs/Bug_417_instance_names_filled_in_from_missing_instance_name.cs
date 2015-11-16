using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_417_instance_names_filled_in_from_missing_instance_name
    {
        [Test]
        public void should_add_the_instance_name_in_missing_name_usage()
        {
            var container = new Container(_ =>
            {
                _.For<ColorRule>().MissingNamedInstanceIs.ConstructedBy(c => new ColorRule(c.RequestedName));

                _.For<ColorRule>().OnCreationForAll("Just for testing", r => Debug.WriteLine(r.Color), i =>
                {
                    if (i.Name != "Red" && i.Name != "Green" && i.Name != "Blue")
                    {
                        Assert.Fail("Still using a Guid for the Id! --> " + i.Name);
                    }

                    return true;
                });
            });


            container.GetInstance<ColorRule>("Red").ShouldNotBeNull();
            container.GetInstance<ColorRule>("Green").ShouldNotBeNull();
            container.GetInstance<ColorRule>("Blue").ShouldNotBeNull();

            
        }
    }
}