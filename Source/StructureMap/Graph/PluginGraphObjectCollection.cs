using System;
using System.Collections;

namespace StructureMap.Graph
{
    public abstract class PluginGraphObjectCollection : MarshalByRefObject, ICollection
    {
        private readonly PluginGraph _pluginGraph;

        public PluginGraphObjectCollection(PluginGraph pluginGraph) : base()
        {
            _pluginGraph = pluginGraph;
        }

        protected abstract ICollection innerCollection { get; }

        #region ICollection Members

        public IEnumerator GetEnumerator()
        {
            ArrayList list = new ArrayList(innerCollection);
            try
            {
                list.Sort();
            }
            catch (Exception)
            {
                // no-op.  Only happens in trouble-shooting instances anyway
            }

            return list.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            innerCollection.CopyTo(array, index);
        }

        public int Count
        {
            get { return innerCollection.Count; }
        }

        public object SyncRoot
        {
            get { return innerCollection.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return innerCollection.IsSynchronized; }
        }

        #endregion

        protected void verifySealed()
        {
            if (!_pluginGraph.IsSealed)
            {
                throw new InvalidOperationException("This PluginGraph is not Sealed!");
            }
        }

        protected void verifyNotSealed()
        {
            if (_pluginGraph.IsSealed)
            {
                throw new InvalidOperationException("This PluginGraph is Sealed!");
            }
        }
    }
}