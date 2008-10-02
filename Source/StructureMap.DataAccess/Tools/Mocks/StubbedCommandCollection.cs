using System.Collections;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class StubbedCommandCollection : ICommandCollection
    {
        private readonly Hashtable _commands;

        public StubbedCommandCollection()
        {
            _commands = new Hashtable();
        }

        #region ICommandCollection Members

        [IndexerName("Command")]
        public ICommand this[string commandName]
        {
            get
            {
                if (!_commands.ContainsKey(commandName))
                {
                    var command = new MockCommand(commandName);
                    _commands.Add(commandName, command);
                }

                return (ICommand) _commands[commandName];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _commands.Values.GetEnumerator();
        }

        #endregion
    }
}