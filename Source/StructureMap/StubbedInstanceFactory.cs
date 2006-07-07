using StructureMap.Interceptors;

namespace StructureMap
{
	/// <summary>
	/// Decorator that intercepts a call to create a PluginType and returns a 
	/// static mock or stub.  Used when ObjectFactory.InjectStub(Type, object) is called.
	/// </summary>
	public class StubbedInstanceFactory : InstanceFactoryInterceptor
	{
		private readonly object _stub;
		private readonly IInstanceFactory _innerFactory;

		public StubbedInstanceFactory(IInstanceFactory innerFactory, object stub) : base()
		{
			this.InnerInstanceFactory = innerFactory;
			_stub = stub;
		}

		/// <summary>
		/// Creates an object instance for the InstanceKey
		/// </summary>
		/// <param name="InstanceKey">The named instance</param>
		/// <returns></returns>
		public override object GetInstance(string InstanceKey)
		{
			return _stub;
		}

		/// <summary>
		/// Creates an object instance directly from the Memento
		/// </summary>
		/// <param name="Memento">A representation of an object instance</param>
		/// <returns></returns>
		public override object GetInstance(InstanceMemento Memento)
		{
			return _stub;
		}

		/// <summary>
		/// Creates a new object instance of the default instance memento
		/// </summary>
		/// <returns></returns>
		public override object GetInstance()
		{
			return _stub;
		}


		public override bool IsMockedOrStubbed
		{
			get { return true; }
		}

	}
}