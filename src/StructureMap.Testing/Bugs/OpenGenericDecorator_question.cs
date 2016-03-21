using StructureMap.Building.Interception;
using StructureMap.Graph;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    // This came from SO: http://stackoverflow.com/questions/32722306/decorator-interception-with-open-generics-in-structuremap-3

    public class OpenGenericDecorator_question
    {
        [Fact]
        public void applies_the_decorator()
        {
            var container = new Container(_ =>
            {
                _.Policies.Interceptors(new CommandHandlerLoggingDecoration());
                _.Scan(scan =>
                {
                    scan.Exclude(type => type == typeof(CommandHandlerLoggingDecorator<>));
                    scan.TheCallingAssembly();
                    scan.ConnectImplementationsToTypesClosing(typeof(ICommandHandler<>));
                });
            });

            /*
            container.GetInstance<ICommandHandler<Command1>>()
                .ShouldBeOfType<CommandHandlerLoggingDecorator<Command1>>()
                .Inner.ShouldBeOfType<Command1Handler>();
             */

            container.GetAllInstances<ICommandHandler<Command1>>()
                .OfType<CommandHandlerLoggingDecorator<Command1>>()
                .Select(x => x.Inner.GetType()).OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(Command1Handler), typeof(OtherCommand1Handler), typeof(YetAnotherCommand1Handler));
        }
    }

    public interface ICommandHandler<T>
    {
    }

    public class CommandHandlerLoggingDecoration : DecoratorPolicy
    {
        public CommandHandlerLoggingDecoration()
            : base(typeof(ICommandHandler<>), typeof(CommandHandlerLoggingDecorator<>), type => true)
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

    public class OtherCommand1Handler : ICommandHandler<Command1>
    {
    }

    public class YetAnotherCommand1Handler : ICommandHandler<Command1>
    {
    }
}