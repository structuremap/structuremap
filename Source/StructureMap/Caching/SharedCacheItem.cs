namespace StructureMap.Caching
{
    public class SharedCacheItem : CacheItem
    {
        private object _value;

        public SharedCacheItem(object Key) : base(Key)
        {
        }

        protected override object getValue()
        {
            return _value;
        }

        protected override void setValue(object Value)
        {
            _value = Value;
        }
    }
}