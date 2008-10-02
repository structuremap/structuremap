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

        #region ICommandFactory Members

        public ICommand BuildCommand(string commandName)
        {
            var command = (ICommand) ObjectFactory.GetNamedInstance(typeof (ICommand), commandName);
            initialize(command);
            command.Name = commandName;

            return command;
        }

        public IReaderSource BuildReaderSource(string name)
        {
            var source = (IReaderSource) ObjectFactory.GetNamedInstance(typeof (IReaderSource), name);
            initialize(source);
            source.Name = name;

            return source;
        }

        #endregion

        private void initialize(object target)
        {
            var initializable = target as IInitializable;
            if (initializable != null)
            {
                initializable.Initialize(_engine);
            }
        }
    }
}