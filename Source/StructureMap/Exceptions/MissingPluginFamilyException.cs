using System;

namespace StructureMap.Exceptions
{
	public class MissingPluginFamilyException : ApplicationException
	{
		private string _message;

		public MissingPluginFamilyException(string pluginTypeName) : base()
		{
			_message = string.Format("Type {0} is not a configured PluginFamily", pluginTypeName);
		}

		public override string Message
		{
			get { return _message; }
		}
	}
}
