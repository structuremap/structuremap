using System;

namespace StructureMap.Caching
{
	public class CloneCacheItem : CacheItem
	{
		private ICloneable _prototype;

		public CloneCacheItem(object Key) : base(Key)
		{
		}


		protected override void setValue(object Value)
		{
			if (Value is ICloneable)
			{
				_prototype = ((ICloneable) Value).Clone() as ICloneable;
			}
			else
			{
				string msg = string.Format("Type {0} is does not implement the ICloneable interface", Value.GetType().FullName);
				throw new ApplicationException(msg);
			}
		}

		protected override object getValue()
		{
			return _prototype.Clone();
		}
	}
}