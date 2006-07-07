using System;

namespace StructureMap.Caching
{
	public class PruneEventDispatcher : EventDispatcher
	{
		public PruneEventDispatcher(string SubjectName) : base(SubjectName)
		{
		}

		protected override void dispatch(IManagedCache _cache)
		{
			_cache.Prune(DateTime.Now);
		}

	}
}