using System.Data;

namespace StructureMap.DataAccess.Commands
{
    [Pluggable("StoredProcedure")]
    public class StoredProcedureCommand : CommandBase
    {
        private readonly string _commandText;

        [DefaultConstructor]
        public StoredProcedureCommand(string commandText)
        {
            _commandText = commandText;
        }

        public StoredProcedureCommand(string commandText, IDatabaseEngine engine)
            : this(commandText)
        {
            Initialize(engine);
        }

        public StoredProcedureCommand(string commandText, IDataSession session)
            : this(commandText)
        {
            session.Initialize(this);
            Attach(session);
        }

        public string CommandText
        {
            get { return _commandText; }
        }

        public override void Initialize(IDatabaseEngine engine)
        {
            IDbCommand innerCommand = engine.CreateStoredProcedureCommand(_commandText);
            var parameters = new ParameterCollection(innerCommand.Parameters);

            initializeMembers(parameters, innerCommand);
        }
    }
}