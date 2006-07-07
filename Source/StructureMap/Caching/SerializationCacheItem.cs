using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace StructureMap.Caching
{
	public class SerializationCacheItem : CacheItem
	{
		private byte[] _binaryArray;

		public SerializationCacheItem(object Key) : base(Key)
		{
		}


		protected override object getValue()
		{
			object returnValue = null;

			if (_binaryArray != null)
			{
				using (MemoryStream memStream =
					new MemoryStream(_binaryArray))
				{
					returnValue =
						new BinaryFormatter().Deserialize(memStream);
					memStream.Close();
				}
			}

			return returnValue;
		}

		protected override void setValue(object Value)
		{
			using (MemoryStream memStream = new MemoryStream())
			{
				try
				{
					new BinaryFormatter().Serialize(memStream, Value);
					_binaryArray = memStream.ToArray();
				}
				catch (SerializationException ex)
				{
					string msg = string.Format("Exception when trying to serialize type {0}", Value.GetType().FullName);
					throw new ApplicationException(msg, ex);
				}
			}
		}

	}
}