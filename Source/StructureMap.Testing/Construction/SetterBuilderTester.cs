using NUnit.Framework;
using StructureMap.Construction;

namespace StructureMap.Testing.Construction
{
    [TestFixture]
    public class SetterBuilderTester
    {
        [Test]
        public void build_and_execute_a_mandatory_setter()
        {
            var builder = new SetterBuilder<SetterTarget>();
            var func = builder.BuildMandatorySetter("Name");

            var args = new StubArguments();
            args.Set("Name", "Max");

            var target = new SetterTarget();
            func(args, target);

            target.Name.ShouldEqual("Max");
        }

        [Test]
        public void build_and_execute_an_optional_setter()
        {
            var builder = new SetterBuilder<SetterTarget>();
            var func = builder.BuildOptionalSetter("Name");

            var args = new StubArguments();
            var target = new SetterTarget();
            func(args, target);

            target.Name.ShouldBeNull();


            args.Set("Name", "Max");
            func(args, target);

            target.Name.ShouldEqual("Max");

        }

        public class SetterTarget
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Active { get; set; }
        }
    }
}