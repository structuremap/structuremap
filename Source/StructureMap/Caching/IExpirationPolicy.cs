using System;

namespace StructureMap.Caching
{
	[PluginFamily]
	public interface IExpirationPolicy
	{
		bool HasExpired(ICacheItem item);
		void Calculate(DateTime now);
	}
}