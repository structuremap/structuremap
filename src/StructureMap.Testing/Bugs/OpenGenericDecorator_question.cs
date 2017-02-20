using System;
using StructureMap.Building.Interception;
using StructureMap.Graph;
using System.Linq;
using StructureMap.Pipeline;
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
                _.Policies.Add<StructureMapForClassPolicy>();
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

    public interface ILogger
    {
        
    }

    public class Logger : ILogger
    {
        public Type Type { get; }

        public static Logger For(Type type)
        {
            return new Logger(type);
        }

        private Logger(Type type)
        {
            Type = type;
        }
    }

    public class StructureMapForClassPolicy : ConfiguredInstancePolicy
    {
        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            // Try to inject an ILogger via constructor parameter.
            var param = instance.Constructor.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(ILogger));
            if (param != null)
            {
                var logger = Logger.For(pluginType);
                instance.Dependencies.AddForConstructorParameter(param, logger);
            }
            else
            {
                // Try to inject an ILogger via public-settable property.
                var prop = instance.SettableProperties().FirstOrDefault(x => x.PropertyType == typeof(ILogger));
                if (prop != null)
                {
                    var logger = Logger.For(pluginType);
                    instance.Dependencies.AddForProperty(prop, logger);
                }
            }
        }
    }

    public class CommandHandlerLoggingDecorator<T> : ICommandHandler<T>
    {
        public ICommandHandler<T> Inner { get; set; }
        public ILogger Logger { get; }

        public CommandHandlerLoggingDecorator(ICommandHandler<T> inner, ILogger logger)
        {
            Inner = inner;
            Logger = logger;
        }
    }

    public class Command1
    {
    }

    public class Command1Handler : ICommandHandler<Command1>
    {
        public Command1Handler(ILogger logger)
        {
        }
    }

    public class OtherCommand1Handler : ICommandHandler<Command1>
    {
    }

    public class YetAnotherCommand1Handler : ICommandHandler<Command1>
    {
    }
}