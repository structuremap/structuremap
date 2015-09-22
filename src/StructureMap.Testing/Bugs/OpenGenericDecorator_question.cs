using NUnit.Framework;
using Shouldly;
using StructureMap.Building.Interception;
using StructureMap.Graph;

namespace StructureMap.Testing.Bugs
{
    // This came from SO: http://stackoverflow.com/questions/32722306/decorator-interception-with-open-generics-in-structuremap-3

    [TestFixture]
    public class OpenGenericDecorator_question
    {
        [Test]
        public void applies_the_decorator()
        {
            var container = new Container(_ =>
            {
                _.Policies.Interceptors(new CommandHandlerLoggingDecoration());
                _.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.ConnectImplementationsToTypesClosing(typeof (ICommandHandler<>));
                });
            });

            container.GetInstance<ICommandHandler<Command1>>()
                .ShouldBeOfType<CommandHandlerLoggingDecorator<Command1>>()
                .Inner.ShouldBeOfType<Command1Handler>();
        }
    }

    public interface ICommandHandler<T>
    {
    }

    public class CommandHandlerLoggingDecoration : DecoratorPolicy
    {
        public CommandHandlerLoggingDecoration()
            : base(typeof (ICommandHandler<>), typeof (CommandHandlerLoggingDecorator<>), type => true)
        {
        }
    }

    public class CommandHandlerLoggingDecorator<T> : ICommandHandler<T>
    {
        public ICommandHandler<T> Inner { get; set; }

        public CommandHandlerLoggingDecorator(ICommandHandler<T> inner)
        {
            Inner = inner;
        }
    }

    public class Command1
    {
    }

    public class Command1Handler : ICommandHandler<Command1>
    {
    }


}