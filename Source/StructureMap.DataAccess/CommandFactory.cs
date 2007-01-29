namespace StructureMap.DataAccess
{
    [Pluggable("Default")]
    public class CommandFactory : ICommandFactory
    {
        private readonly IDatabaseEngine _engine;

        public CommandFactory(IDatabaseEngine engine)
        {
            _engine = engine;
        }

        public ICommand BuildCommand(string commandName)
        {
            ICommand command = (ICommand) ObjectFactory.GetNamedInstance(typeof (ICommand), commandName);
            initialize(command);
            command.Name = commandName;

            return command;
        }

        private void initialize(object target)
        {
            IInitializable initializable = target as IInitializable;
            if (initializable != null)
            {
                initializable.Initialize(_engine);
            }
        }

        public IReaderSource BuildReaderSource(string name)
        {
            IReaderSource source = (IReaderSource) ObjectFactory.GetNamedInstance(typeof (IReaderSource), name);
            initialize(source);
            source.Name = name;

            return source;
        }
    }
}