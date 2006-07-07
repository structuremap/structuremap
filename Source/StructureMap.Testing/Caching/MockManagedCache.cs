using System;
using System.Threading;
using NUnit.Framework;
using StructureMap.Caching;

namespace StructureMap.Testing.Caching
{
	public class MockManagedCache : IManagedCache
	{
		private ManualResetEvent _event;
		private bool _expectPrune;
		private bool _expectClear;
		private bool _pruned = false;
		private bool _cleared = false;

		public MockManagedCache(ManualResetEvent Event, bool ExpectPrune, bool ExpectClear)
		{
			_event = Event;
			_expectPrune = ExpectPrune;
			_expectClear = ExpectClear;
		}

		#region IManagedCache Members

		public void AddWatches(CacheManager Manager)
		{
			// TODO:  Add MockManagedCache.AddWatches implementation
		}

		public void Prune(DateTime currentTime)
		{
			if (!_expectPrune)
			{
				Assert.Fail("Unexpected Prune() call");
			}

			_pruned = true;
			_event.Set();
		}

		public void Clear()
		{
			if (!_expectClear)
			{
				Assert.Fail("Unexpected Clear() call");
			}

			_cleared = true;
			_event.Set();
		}

		public void Verify()
		{
			if (_expectClear && !_cleared)
			{
				Assert.Fail("Clear() was not called!");
			}

			if (_expectPrune && !_pruned)
			{
				Assert.Fail("Prune() was not called!");
			}
		}

		public string CacheName
		{
			get { return "MOCK"; }
		}

		#endregion
	}
}