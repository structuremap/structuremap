using System.Data;

namespace StructureMap.DataAccess.Tools.Mocks
{
	public class ReaderExpectation
	{
		private readonly ParameterList _parameters;
		private readonly IDataReader _result;

		public ReaderExpectation(IDataReader result)
		{
			_result = result;
			_parameters = new ParameterList();
		}

		public ReaderExpectation(ParameterList parameters, IDataReader result)
		{
			_parameters = parameters;
			_result = result;
		}

		public IDataReader VerifyAndGetReader(ParameterList actualParameters)
		{
			_parameters.Verify(actualParameters);

			return _result;
		}

		public ParameterList Parameters
		{
			get { return _parameters; }
		}
	}
}
