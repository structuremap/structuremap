using System.Collections;
using StructureMap.Interceptors;

namespace StructureMap.Graph
{
	/// <summary>
	/// Manages a list of InstanceFactoryInterceptor's.  Design-time model of an array
	/// of decorators to alter the InstanceFactory behavior for a PluginType.
	/// </summary>
	public class InterceptionChain
	{
		private ArrayList _interceptorList;

		public InterceptionChain()
		{
			_interceptorList = new ArrayList();
		}

		public IInstanceFactory WrapInstanceFactory(IInstanceFactory factory)
		{
			IInstanceFactory outerFactory = factory;

			for (int i = _interceptorList.Count - 1; i >= 0; i--)
			{
				InstanceFactoryInterceptor interceptor = (InstanceFactoryInterceptor) _interceptorList[i];
				interceptor.InnerInstanceFactory = outerFactory;
				outerFactory = interceptor;
			}

			return outerFactory;
		}

		public void AddInterceptor(InstanceFactoryInterceptor interceptor)
		{
			_interceptorList.Add(interceptor);
		}

		public int Count
		{
			get { return _interceptorList.Count; }
		}

		public InstanceFactoryInterceptor this[int index]
		{
			get { return (InstanceFactoryInterceptor) _interceptorList[index]; }
		}

	}
}