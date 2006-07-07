using System;

namespace StructureMap.Caching.Expirations
{
	[Pluggable("SlidingTime")]
	public class SlidingTimeExpirationPolicy : IExpirationPolicy
	{
		private long _ticks;
		private DateTime _currentTime;

		public SlidingTimeExpirationPolicy(int minutes)
		{
			_ticks = TimeSpan.TicksPerMinute*minutes;
		}

		public long Ticks
		{
			get { return _ticks; }
		}

		#region IExpirationPolicy Members

		public bool HasExpired(ICacheItem item)
		{
			TimeSpan span = _currentTime.Subtract(item.LastAccessed);
			return (span.Ticks >= _ticks);
		}

		public void Calculate(DateTime now)
		{
			_currentTime = now;
		}

		#endregion
	}
}