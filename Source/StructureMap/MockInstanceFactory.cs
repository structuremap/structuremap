using System;
using NMock;
using StructureMap.Interceptors;

namespace StructureMap
{
	/// <summary>
	/// Implementation of IInstanceFactory that uses NMock internally to create instances of the 
	/// PluginType
	/// </summary>
	public class MockInstanceFactory : InstanceFactoryInterceptor
	{
		private IMock _mockType;

		/// <summary>
		/// Creates a MockInstanceFactory that wraps and intercepts calls to the innerFactory
		/// </summary>
		/// <param name="innerFactory"></param>
		public MockInstanceFactory(IInstanceFactory innerFactory)
		{
			this.InnerInstanceFactory = innerFactory;
			_mockType = new DynamicMock(innerFactory.PluginType);
		}

		/// <summary>
		/// Returns "new DyanamicProxy(this.PluginType) as IMock"
		/// </summary>
		/// <returns></returns>
		public IMock GetMock()
		{
			return _mockType;
		}

		/// <summary>
		/// See <cref>IInstanceFactory</cref>
		/// </summary>
		/// <param name="InstanceKey"></param>
		/// <returns></returns>
		public override object GetInstance(string InstanceKey)
		{
			return _mockType.MockInstance;
		}

		/// <summary>
		/// See <cref>IInstanceFactory</cref>
		/// </summary>
		/// <param name="Memento"></param>
		/// <returns></returns>
		public override object GetInstance(InstanceMemento Memento)
		{
			return _mockType.MockInstance;
		}

		/// <summary>
		/// See <cref>IInstanceFactory</cref>
		/// </summary>
		/// <returns></returns>
		public override object GetInstance()
		{
			return _mockType.MockInstance;
		}

		public override bool IsMockedOrStubbed
		{
			get { return true; }
		}

	    public override object Clone()
	    {
            return this.MemberwiseClone();
	    }
	}
}