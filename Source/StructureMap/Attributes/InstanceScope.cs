using System;

namespace StructureMap.Attributes
{
	public enum InstanceScope
	{
		PerRequest,
		Singleton,
		ThreadLocal,
		HttpContext,
		Hybrid
	}
}
