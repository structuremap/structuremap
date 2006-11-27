using System.Data;

namespace StructureMap.DataAccess
{
	public interface IParameter
	{
		void SetProperty(object propertyValue);
		object GetProperty();
		string ParameterName {get;}
		void OverrideParameterType (DbType dbtype);
	}
}
