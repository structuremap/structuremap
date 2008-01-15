using System.Collections;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class MockCommand : ICommand
    {
        private CommandExpectation _currentExpectation;
        private Queue _expectations;
        private ParameterList _inputs;
        private string _name;

        public MockCommand(string name)
        {
            _name = name;
            _expectations = new Queue();
            _inputs = new ParameterList();
        }

        #region ICommand Members

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public int Execute()
        {
            if (_expectations.Count == 0)
            {
                throw new UnExpectedCallException(Name);
            }

            _currentExpectation = (CommandExpectation) _expectations.Dequeue();
            return _currentExpectation.VerifyExecution(_inputs);
        }

        [IndexerName("Parameter")]
        public object this[string parameterName]
        {
            get
            {
                if (_currentExpectation != null)
                {
                    if (_currentExpectation.IsOutput(parameterName))
                    {
                        return _currentExpectation.GetOutput(parameterName);
                    }
                }

                if (_inputs.Contains(parameterName))
                {
                    return _inputs[parameterName];
                }

                if (_expectations.Count > 0)
                {
                    CommandExpectation nextExpectation = (CommandExpectation) _expectations.Peek();
                    if (nextExpectation.IsOutput(parameterName))
                    {
                        throw new NotExecutedCommandException(parameterName, Name);
                    }
                }

                throw new UnKnownOrNotSetParameterException(parameterName);
            }
            set { _inputs[parameterName] = value; }
        }

        public void Attach(IDataSession session)
        {
            // no-op;
        }

        #endregion

        public void AddExpectation(CommandExpectation expectation)
        {
            _expectations.Enqueue(expectation);
        }
    }
}