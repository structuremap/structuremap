using System;

namespace StructureMap.Caching.Expirations
{
    [Pluggable("AbsoluteTime")]
    public class AbsoluteTimeExpirationPolicy : IExpirationPolicy
    {
        private long _ticks;
        private DateTime _currentTime;

        public AbsoluteTimeExpirationPolicy(int minutes)
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
            TimeSpan span = _currentTime.Subtract(item.Created);
            return (span.Ticks >= _ticks);
        }

        public void Calculate(DateTime now)
        {
            _currentTime = now;
        }

        #endregion
    }
}