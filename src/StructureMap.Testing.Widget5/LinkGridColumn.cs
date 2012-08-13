using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    public class LinkGridColumn : IGridColumn
    {
        private readonly string _columnName;

        public LinkGridColumn(string columnName)
        {
            _columnName = columnName;
        }

        public string ColumnName { get { return _columnName; } }

        public IWidget Widget { get; set; }

        public int Width { get; set; }

        public bool Displayed { get; set; }
    }
}