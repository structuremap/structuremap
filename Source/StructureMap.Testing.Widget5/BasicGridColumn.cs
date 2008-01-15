using StructureMap.Attributes;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    [Pluggable("Basic")]
    public class BasicGridColumn : IGridColumn
    {
        private readonly string _headerText;
        private string _columnName;
        private bool _displayed;
        private FontStyleEnum _fontStyle;
        private Rule[] _rules;
        private int _size;
        private IWidget _widget;
        private bool _wrapLines;

        public BasicGridColumn(string headerText)
        {
            _headerText = headerText;
        }

        public string HeaderText
        {
            get { return _headerText; }
        }

        [SetterProperty]
        public IWidget Widget
        {
            get { return _widget; }
            set { _widget = value; }
        }

        [SetterProperty]
        public FontStyleEnum FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        [SetterProperty]
        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        [SetterProperty]
        public Rule[] Rules
        {
            get { return _rules; }
            set { _rules = value; }
        }

        [SetterProperty]
        public bool WrapLines
        {
            get { return _wrapLines; }
            set { _wrapLines = value; }
        }

        public bool Displayed
        {
            get { return _displayed; }
            set { _displayed = value; }
        }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }
    }
}