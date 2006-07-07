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

		public void CopyTo(Array array, int index)
		{
			this.innerCollection.CopyTo(array, index);
		}

		public int Count
		{
			get { return this.innerCollection.Count; }
		}

		public object SyncRoot
		{
			get { return this.innerCollection.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return this.innerCollection.IsSynchronized; }
		}


	}
}