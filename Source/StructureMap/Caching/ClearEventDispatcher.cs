namespace StructureMap.Caching
{
	public class ClearEventDispatcher : EventDispatcher
	{
		public ClearEventDispatcher(string SubjectName) : base(SubjectName)
		{
		}

		protected override void dispatch(IManagedCache _cache)
		{
			_cache.Clear();
		}

	}
}