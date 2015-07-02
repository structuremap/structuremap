using System;
using NUnit.Framework;
using StructureMap.Building;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Exception_Within_Exception_Bug
    {
        [Test]
        public void setter_failure_description_doesnt_kill_format()
        {
            var c = new Container(ce => { ce.Policies.SetAllProperties(sc => sc.OfType<PropertyType>()); });

            Exception<StructureMapBuildException>.ShouldBeThrownBy(() => { c.GetInstance<Root>(); })
                .InnerException.ShouldNotBeOfType<FormatException>();
        }
    }

    public class Root
    {
        public PropertyType Prop { get; set; }
    }

    public class PropertyType
    {
        public PropertyType()
        {
            throw new InvalidOperationException();
        }
    }
}