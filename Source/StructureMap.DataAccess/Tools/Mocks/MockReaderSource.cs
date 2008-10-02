using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class MockReaderSource : IReaderSource
    {
        private readonly Queue _expectations;
        private readonly ParameterList _parameters;


        public MockReaderSource(string name)
        {
            Name = name;
            _parameters = new ParameterList();
            _expectations = new Queue();
        }

        #region IReaderSource Members

        public string Name { get; set; }

        public IDataReader ExecuteReader()
        {
            if (_expectations.Count == 0)
            {
                throw new UnExpectedCallException(Name);
            }

            var expectation = (ReaderExpectation) _expectations.Dequeue();

            return expectation.VerifyAndGetReader(_parameters);
        }

        public DataSet ExecuteDataSet()
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        [IndexerName("Parameter")]
        public object this[string parameterName]
        {
            get { return _parameters[parameterName]; }
            set { _parameters[parameterName] = value; }
        }

        public void Attach(IDataSession session)
        {
            // no-op;
        }

        public string ExecuteJSON()
        {
            throw new NotImplementedException();
        }

        #endregion

        public void AddExpectation(ReaderExpectation expectation)
        {
            _expectations.Enqueue(expectation);
        }
    }
}