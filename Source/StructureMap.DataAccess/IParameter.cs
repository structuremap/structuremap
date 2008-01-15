using System.Data;

namespace StructureMap.DataAccess
{
    public interface IParameter
    {
        string ParameterName { get; }
        void SetProperty(object propertyValue);
        object GetProperty();
        void OverrideParameterType(DbType dbtype);
    }
}