using System;

namespace StructureMap.Interceptors
{
	[Pluggable("Hybrid")]
	public class HybridCacheInterceptor : InstanceFactoryInterceptor
	{
		private InstanceFactoryInterceptor _innerInterceptor;

		public HybridCacheInterceptor() : base()
		{
			if (HttpContextItemInterceptor.HasContext())
			{
				_innerInterceptor = new HttpContextItemInterceptor();
			}
		}

		public override IInstanceFactory InnerInstanceFactory
		{
			get { return _innerInterceptor.InnerInstanceFactory; }
			set { _innerInterceptor.InnerInstanceFactory = value; }
		}

		public override object GetInstance(string instanceKey)
		{
			return _innerInterceptor.GetInstance(instanceKey);
		}

		public override object GetInstance(InstanceMemento Memento)
		{
			return _innerInterceptor.GetInstance(Memento);
		}

		public override object GetInstance()
		{
			return _innerInterceptor.GetInstance();
		}
	}
}
