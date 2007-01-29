using System.Collections;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess
{
    public class ReaderSourceCollection : IReaderSourceCollection
    {
        private readonly DataSession _parent;
        private readonly ICommandFactory _commandFactory;
        private Hashtable _sources;

        public ReaderSourceCollection(DataSession session, ICommandFactory commandFactory)
        {
            _parent = session;
            _commandFactory = commandFactory;
            _sources = new Hashtable();
        }

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
                    buildSource(name);
                }

                return (IReaderSource) _sources[name];
            }
        }

        private void buildSource(string name)
        {
            lock (this)
            {
                if (!_sources.ContainsKey(name))
                {
                    IReaderSource source = _commandFactory.BuildReaderSource(name);
                    source.Attach(_parent);

                    _sources.Add(name, source);
                }
            }
        }
    }
}