using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ArgumentTester
    {
        [Test]
        public void close_an_open_type()
        {
            var argument = new Argument
            {
                Type = typeof (IService<>),
                Dependency = new ConfiguredInstance(typeof (Service<>)),
                Name = "foo"
            };

            var closed = argument.CloseType(typeof (int));

            closed.Type.ShouldBe(typeof (IService<int>));
            closed.Dependency.ShouldBeOfType<ConstructorInstance>()
                .PluggedType.ShouldBe(typeof (Service<int>));
            closed.Name.ShouldBe("foo");
        }

        [Test]
        public void close_an_already_closed_type_just_clones()
        {
            var argument = new Argument
            {
                Type = typeof (IGateway),
                Dependency = new StubbedGateway(),
                Name = "Foo"
            };

            var closed = argument.CloseType(typeof (int));
            closed.ShouldNotBeTheSameAs(argument);

            closed.Type.ShouldBe(argument.Type);
            closed.Dependency.ShouldBe(argument.Dependency);
            closed.Name.ShouldBe(argument.Name);
        }

        [Test]
        public void close_an_already_closed_type_just_clones_2()
        {
            var argument = new Argument
            {
                Type = typeof (IGateway),
                Dependency = new ConstructorInstance(typeof (StubbedGateway)),
                Name = "Foo"
            };

            var closed = argument.CloseType(typeof (int));
            closed.ShouldNotBeTheSameAs(argument);

            closed.Type.ShouldBe(argument.Type);
            closed.Dependency.ShouldBe(argument.Dependency);
            closed.Name.ShouldBe(argument.Name);
        }
    }
}