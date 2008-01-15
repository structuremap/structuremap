using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    public class LinkGridColumn : IGridColumn
    {
        private readonly string _columnName;
        private bool _displayed;
        private IWidget _widget;
        private int _width;

        public LinkGridColumn(string columnName)
        {
            _columnName = columnName;
        }

        public string ColumnName
        {
            get { return _columnName; }
        }

        public IWidget Widget
        {
            get { return _widget; }
            set { _widget = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public bool Displayed
        {
            get { return _displayed; }
            set { _displayed = value; }
        }
    }
}