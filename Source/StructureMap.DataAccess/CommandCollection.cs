using System.Collections;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess
{
	public class CommandCollection : ICommandCollection
	{
		private readonly ICommandFactory _commandFactory;
		private readonly DataSession _parent;
		private Hashtable _commands;

		public CommandCollection(DataSession parent, ICommandFactory commandFactory)
		{
			_commandFactory = commandFactory;
			_parent = parent;
			_commands = new Hashtable();
		}

		[IndexerName("Command")]
		public ICommand this[string commandName]
		{
			get
			{
				if (!_commands.ContainsKey(commandName))
				{
					buildCommand(commandName);
				}

				return (ICommand) _commands[commandName];
			}
		}

		private void buildCommand(string commandName)
		{
			lock (this)
			{
				if (!_commands.ContainsKey(commandName))
				{
					ICommand command = _commandFactory.BuildCommand(commandName);
					command.Attach(_parent);

					_commands.Add(commandName, command);
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return _commands.Values.GetEnumerator();
		}


	}
}
