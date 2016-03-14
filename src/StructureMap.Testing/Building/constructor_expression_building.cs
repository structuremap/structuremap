using Shouldly;
using StructureMap.Building;
using Xunit;

namespace StructureMap.Testing.Building
{
    public class constructor_expression_building
    {
        [Fact]
        public void try_a_simple_constructor_with_all_constants()
        {
            var step = ConcreteBuild<CtorTarget>.For(() => new CtorTarget("", 0));
            step.Constructor.Add(Constant.For("Jeremy"));
            step.Constructor.Add(Constant.For(39));

            var context = new FakeBuildSession();

            var target = step.Build<CtorTarget>(context);

            target.Name.ShouldBe("Jeremy");
            target.Age.ShouldBe(39);
        }
    }

    public class CtorTarget
    {
        private readonly string _name;
        private readonly int _age;

        public CtorTarget(string name, int age)
        {
            _name = name;
            _age = age;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Age
        {
            get { return _age; }
        }
    }
}