using StructureMap.Attributes;
using StructureMap.LegacyAttributeSupport;

namespace StructureMap.Testing.Widget5
{
    [Pluggable("Bad")]
    public class BadSetterClass
    {
        private readonly string _columnName;


        public BadSetterClass(string columnName)
        {
            _columnName = columnName;
        }

        [SetterProperty]
        public string ColumnName { get { return _columnName; } }
    }
}