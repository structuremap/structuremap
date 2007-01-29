using StructureMap.Attributes;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    [Pluggable("String")]
    public class StringGridColumn : IGridColumn
    {
        private string _name;

        [SetterProperty]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

    [Pluggable("Long")]
    public class LongGridColumn : IGridColumn
    {
        private long _count;

        [SetterProperty]
        public long Count
        {
            get { return _count; }
            set { _count = value; }
        }
    }

    [Pluggable("Enum")]
    public class EnumGridColumn : IGridColumn
    {
        private FontStyleEnum _fontStyle;

        [SetterProperty]
        public FontStyleEnum FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }
    }

    [Pluggable("Widget")]
    public class WidgetGridColumn : IGridColumn
    {
        private IWidget _widget;

        [SetterProperty]
        public IWidget Widget
        {
            get { return _widget; }
            set { _widget = value; }
        }
    }

    [Pluggable("WidgetArray")]
    public class WidgetArrayGridColumn : IGridColumn
    {
        private IWidget[] _widgets;

        [SetterProperty]
        public IWidget[] Widgets
        {
            get { return _widgets; }
            set { _widgets = value; }
        }
    }
}