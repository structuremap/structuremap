using System;
using System.Runtime.CompilerServices;
using StructureMap.DataAccess;

namespace StructureMap.Testing.DataAccess
{
    public class StubbedCommand : ICommand, IInitializable
    {
        private IDataSession _session;
        private bool _wasInitialized;

        public bool WasInitialized
        {
            get { return _wasInitialized; }
        }

        public IDataSession Session
        {
            get { return _session; }
        }

        #region ICommand Members

        public string Name { get; set; }

        public int Execute()
        {
            throw new NotImplementedException();
        }

        [IndexerName("Parameter")]
        public object this[string parameterName]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Attach(IDataSession session)
        {
            _session = session;
        }

        #endregion

        #region IInitializable Members

        public void Initialize(IDatabaseEngine engine)
        {
            _wasInitialized = true;
        }

        #endregion
    }
}