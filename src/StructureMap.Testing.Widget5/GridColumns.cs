using StructureMap.Attributes;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    public class StringGridColumn : IGridColumn
    {
        [SetterProperty]
        public string Name { get; set; }
    }

    public class LongGridColumn : IGridColumn
    {
        [SetterProperty]
        public long Count { get; set; }
    }

    public class EnumGridColumn : IGridColumn
    {
        [SetterProperty]
        public FontStyleEnum FontStyle { get; set; }
    }

    public class WidgetGridColumn : IGridColumn
    {
        [SetterProperty]
        public IWidget Widget { get; set; }
    }

    public class WidgetArrayGridColumn : IGridColumn
    {
        [SetterProperty]
        public IWidget[] Widgets { get; set; }
    }
}