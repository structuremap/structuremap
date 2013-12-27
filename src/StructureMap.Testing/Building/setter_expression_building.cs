using System;
using System.Diagnostics.Eventing.Reader;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class setter_expression_building
    {
        [Test]
        public void simple_creation_of_properties_with_only_constants()
        {
            var step = new ConcreteBuild<SetterTarget>();
            step.Set(x => x.Color, "Red");
            step.Set(x => x.Direction, "North");

            SetterTarget target = step.Build<SetterTarget>(new FakeBuildSession());

            target.Color.ShouldEqual("Red");
            target.Direction.ShouldEqual("North");
        }

        [Test]
        public void simple_creation_of_fields_with_only_constants()
        {
            var step = new ConcreteBuild<FieldTarget>();
            step.Set(x => x.Color, "Red");
            step.Set(x => x.Direction, "North");

            FieldTarget target = step.Build<FieldTarget>(new FakeBuildSession());

            target.Color.ShouldEqual("Red");
            target.Direction.ShouldEqual("North");
        }
    }

    public class SetterTarget
    {
        public string Color { get; set; }
        public string Direction { get; set; }

        public IGateway Gateway { get; set; }
    }

    public class FieldTarget
    {
        public string Color;
        public string Direction;
    }
}