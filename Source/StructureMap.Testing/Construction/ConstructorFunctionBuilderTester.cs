using System;
using NUnit.Framework;
using StructureMap.Construction;

namespace StructureMap.Testing.Construction
{
    [TestFixture]
    public class ConstructorFunctionBuilderTester
    {
        private Func<IArguments, ConstructorTarget> func;

        [SetUp]
        public void SetUp()
        {
            var builder = new ConstructorFunctionBuilder<ConstructorTarget>();
            func = builder.CreateBuilder();
        }

        [Test]
        public void build_an_object()
        {
            var args = new StubArguments();
            args.Set("name", "Jeremy");
            args.Set("age", 35);

            // That's actually correct, you know, just in case you want to buy me
            // a birthday present
            args.Set("birthDay", new DateTime(1974, 1, 1));

            var target = func(args);

            target.Name.ShouldEqual("Jeremy");
            target.Age.ShouldEqual(35);
            target.BirthDay.ShouldEqual(new DateTime(1974, 1, 1));
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
            public int Number { get; set; }
        }
    }
}