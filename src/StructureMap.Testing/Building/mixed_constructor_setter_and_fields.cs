using Shouldly;
using StructureMap.Building;
using Xunit;

namespace StructureMap.Testing.Building
{
    public class mixed_constructor_setter_and_fields
    {
        [Fact]
        public void with_all_constants()
        {
            var build = new ConcreteBuild<MixedTarget>();
            build.Constructor.Add(Constant.For("Jeremy"));
            build.Constructor.Add(Constant.For("Red"));

            build.Set(x => x.Direction, "South");
            build.Set(x => x.Description, "Something");

            var target = build.Build<MixedTarget>(new FakeBuildSession());

            target.Name.ShouldBe("Jeremy");
            target.Color.ShouldBe("Red");
            target.Direction.ShouldBe("South");
            target.Description.ShouldBe("Something");
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