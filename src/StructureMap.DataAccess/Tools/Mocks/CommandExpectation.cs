using System;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class CommandExpectation : ICloneable
    {
        private readonly ParameterList _inputs;
        private readonly ParameterList _outputs;
        private readonly int _rowsAffected;
        private bool _wasExecuted;

        public CommandExpectation() : this(1)
        {
        }

        public CommandExpectation(int rowsAffected)
        {
            _rowsAffected = rowsAffected;
            _inputs = new ParameterList();
            _outputs = new ParameterList();
        }

        public CommandExpectation(ParameterList inputs, ParameterList outputs, int rowsAffected)
        {
            _inputs = inputs;
            _outputs = outputs;
            _rowsAffected = rowsAffected;
        }

        #region ICloneable Members

        public object Clone()
        {
            return
                new CommandExpectation((ParameterList) _inputs.Clone(), (ParameterList) _outputs.Clone(), _rowsAffected);
        }

        #endregion

        public void SetInput(string parameterName, object parameterValue)
        {
            _inputs[parameterName] = parameterValue;
        }

        public object GetInput(string parameterName)
        {
            return _inputs[parameterName];
        }

        public void SetOutput(string parameterName, object parameterValue)
        {
            _outputs[parameterName] = parameterValue;
        }

        public object GetOutput(string parameterName)
        {
            if (!_wasExecuted)
            {
                throw new NotExecutedCommandException(parameterName);
            }

            return _outputs[parameterName];
        }


        public bool IsOutput(string parameterName)
        {
            return _outputs.Contains(parameterName);
        }

        public int VerifyExecution(ParameterList list)
        {
            _inputs.Verify(list);
            _wasExecuted = true;

            return _rowsAffected;
        }
    }
}