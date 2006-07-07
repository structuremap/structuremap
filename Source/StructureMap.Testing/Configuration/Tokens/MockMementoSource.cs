using System;

namespace StructureMap.Testing.Configuration.Tokens
{
	[Pluggable("Mock")]
	public class MockMementoSource : MementoSource
	{
		public static InstanceMemento CreateFailureMemento()
		{
			MemoryInstanceMemento memento = new MemoryInstanceMemento("Mock", "bad");
			memento.SetProperty("valid", false.ToString());

			return memento;
		}

		public static InstanceMemento CreateSuccessMemento()
		{
			MemoryInstanceMemento memento = new MemoryInstanceMemento("Mock", "bad");
			memento.SetProperty("valid", true.ToString());

			return memento;			
		}


		private readonly bool _valid;

		public MockMementoSource(bool valid)
		{
			if (!valid)
			{
				throw new ApplicationException("Bad");
			}
			_valid = valid;
		}

		protected override InstanceMemento[] fetchInternalMementos()
		{
			if (_valid)
			{
				return new InstanceMemento[0];
			}
			else
			{
				throw new ApplicationException("Bad");
			}
		}

		protected override bool containsKey(string instanceKey)
		{
			throw new NotImplementedException();
		}

		protected override InstanceMemento retrieveMemento(string instanceKey)
		{
			throw new NotImplementedException();
		}

		public override string Description
		{
			get { throw new NotImplementedException(); }
		}
	}
}
