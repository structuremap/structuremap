using System;
using NUnit.Framework;
using StructureMap.Construction;
using StructureMap.Graph;

namespace StructureMap.Testing.Construction
{
    [TestFixture]
    public class BuilderCompilerTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void compile_and_exercise_a_builder()
        {
            var func = BuilderCompiler.CompileCreator(new Plugin(typeof (ConstructorTarget)));
            var args = new StubArguments();
            args.Set("name", "Jeremy");
            args.Set("age", 35);
            args.Set("birthDay", new DateTime(2009, 1, 1));

            args.Set("Color", "blue");

            var target = func(args).ShouldBeOfType<ConstructorTarget>();

            target.Name.ShouldEqual("Jeremy");
            target.Age.ShouldEqual(35);
            target.Color.ShouldEqual("blue");

            // Optional wasn't filled in
            target.Direction.ShouldBeNull();
        }

        [Test]
        public void compile_and_exercise_build_up()
        {
            var args = new StubArguments();
            args.Set("Color", "blue");

            var target = new ConstructorTarget(null, 5, DateTime.Today);

            var action = BuilderCompiler.CompileBuildUp(new Plugin(typeof (ConstructorTarget)));
            action(args, target);

            target.Color.ShouldEqual("blue");
        }

        public class ConstructorTarget
        {
            private readonly string _name;
            private readonly int _age;
            private readonly DateTime _birthDay;

            public ConstructorTarget(string name, int age, DateTime birthDay)
            {
                _name = name;
                _age = age;
                _birthDay = birthDay;
            }

            public string Name { get { return _name; } }
            public int Age { get { return _age; } }
            public DateTime BirthDay { get { return _birthDay; } }


            public string Color { get; set; }
            public string Direction { get; set; }
            public int Number { get; set; }
        }
    }
}