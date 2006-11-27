using System;

namespace StructureMap.DataAccess.Tools.Mocks
{
	public class UnKnownOrNotSetParameterException : ApplicationException
	{
		public UnKnownOrNotSetParameterException(string parameterName)
			: base("The parameter '" + parameterName + "' is unknown or has not been set")
		{

		}
	}
}
