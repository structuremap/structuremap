using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using StructureMap.DataAccess;

namespace StructureMap.Testing.DataAccess
{
    public class StubbedReaderSource : IReaderSource, IInitializable
    {
        private readonly IDataReader _reader;
        private string _name;
        private Hashtable _parameters;
        private IDataSession _session;
        private bool _wasInitialized = false;

        public StubbedReaderSource(IDataReader reader)
        {
            _reader = reader;
            _parameters = new Hashtable();
        }

        public IDataSession Session
        {
            get { return _session; }
            set { _session = value; }
        }

        public bool WasInitialized
        {
            get { return _wasInitialized; }
        }

        #region IInitializable Members

        public void Initialize(IDatabaseEngine engine)
        {
            _wasInitialized = true;
        }

        #endregion

        #region IReaderSource Members

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public IDataReader ExecuteReader()
        {
            return _reader;
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
            _session = session;
        }

        public string ExecuteJSON()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}