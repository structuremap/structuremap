using System;
using System.Data;

namespace StructureMap.DataAccess.Parameterization
{
	[Pluggable("Basic")]
	public class BasicParameterTemplate : IParameterTemplate
	{
		private readonly string _parameterName;
		private readonly DbType _dbType;
		private readonly bool _isNullable;

		public BasicParameterTemplate(string parameterName, DbType dbType, bool isNullable)
		{
			_parameterName = parameterName;
			_dbType = dbType;
			_isNullable = isNullable;
		}

		public BasicParameterTemplate(string parameterName)
			: this(parameterName, DbType.Object, true)
		{
			_parameterName = parameterName;
		}

		public IDataParameter ConfigureParameter(IDatabaseEngine database)
		{
			return database.CreateParameter(_parameterName, _dbType, _isNullable);
		}

		public string ParameterName
		{
			get { return _parameterName; }
		}
	}
}
