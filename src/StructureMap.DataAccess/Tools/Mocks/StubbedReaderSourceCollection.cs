using System.Collections;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class StubbedReaderSourceCollection : IReaderSourceCollection
    {
        private readonly Hashtable _sources;

        public StubbedReaderSourceCollection()
        {
            _sources = new Hashtable();
        }

        #region IReaderSourceCollection Members

        public IEnumerator GetEnumerator()
        {
            return _sources.Values.GetEnumerator();
        }

        public int Count
        {
            get { return _sources.Count; }
        }

        [IndexerName("ReaderSource")]
        public IReaderSource this[string name]
        {
            get
            {
                if (!_sources.ContainsKey(name))
                {
                    var source = new MockReaderSource(name);
                    _sources.Add(name, source);
                }

                return (IReaderSource) _sources[name];
            }
            set { _sources[name] = value; }
        }

        #endregion
    }
}