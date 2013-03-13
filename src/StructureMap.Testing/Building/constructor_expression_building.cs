using System;
using NUnit.Framework;
using StructureMap.Building;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class constructor_expression_building
    {
        [Test]
        public void try_a_simple_constructor_with_all_constants()
        {
            var step = ConstructorAndSetterStep<CtorTarget>.For(() => new CtorTarget("", 0));
            step.Constructor.Add(Constant.For("Jeremy"));
            step.Constructor.Add(Constant.For<int>(39));

            var context = new FakeContext();

            var builder = (Func<IContext, CtorTarget>)step.ToDelegate();
            var target = builder(context);

            target.Name.ShouldEqual("Jeremy");
            target.Age.ShouldEqual(39);
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