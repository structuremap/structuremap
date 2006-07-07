using System;
using System.Web;

namespace StructureMap.Interceptors
{
	[Pluggable("HttpContext")]
	public class HttpContextItemInterceptor : CacheInterceptor
	{
		public static bool HasContext()
		{
			return HttpContext.Current != null;
		}

		public HttpContextItemInterceptor() : base()
		{

		}

		private string getKey(string instanceKey)
		{
			return string.Format("{0}:{1}", this.InnerInstanceFactory.PluginType.FullName, instanceKey);
		}

		protected override void cache(string instanceKey, object instance)
		{
			string key = getKey(instanceKey);
			HttpContext.Current.Items.Add(key, instance);
		}

		protected override bool isCached(string instanceKey)
		{
			string key = getKey(instanceKey);
			return HttpContext.Current.Items.Contains(key);
		}

		protected override object getInstance(string instanceKey)
		{
			string key = getKey(instanceKey);
			return HttpContext.Current.Items[key];
		}
	}
}
