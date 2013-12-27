using System;
using NUnit.Framework;
using StructureMap.Building;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class mixed_constructor_setter_and_fields
    {
        [Test]
        public void with_all_constants()
        {
            var build = new ConcreteBuild<MixedTarget>();
            build.Constructor.Add(Constant.For("Jeremy"));
            build.Constructor.Add(Constant.For("Red"));

            build.Set(x => x.Direction, "South");
            build.Set(x => x.Description, "Something");

            var target = build.Build<MixedTarget>(new FakeBuildSession());

            target.Name.ShouldEqual("Jeremy");
            target.Color.ShouldEqual("Red");
            target.Direction.ShouldEqual("South");
            target.Description.ShouldEqual("Something");
        }
    }

    public class MixedTarget
    {
        private readonly string _name;
        private readonly string _color;

        public MixedTarget(string name, string color)
        {
            _name = name;
            _color = color;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Color
        {
            get { return _color; }
        }

        public string Direction;
        public string Description { get; set; }
    }
}