using StructureMap.Attributes;
using StructureMap.LegacyAttributeSupport;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    [Pluggable("String")]
    public class StringGridColumn : IGridColumn
    {
        [SetterProperty]
        public string Name { get; set; }
    }

    [Pluggable("Long")]
    public class LongGridColumn : IGridColumn
    {
        [SetterProperty]
        public long Count { get; set; }
    }

    [Pluggable("Enum")]
    public class EnumGridColumn : IGridColumn
    {
        [SetterProperty]
        public FontStyleEnum FontStyle { get; set; }
    }

    [Pluggable("Widget")]
    public class WidgetGridColumn : IGridColumn
    {
        [SetterProperty]
        public IWidget Widget { get; set; }
    }

    [Pluggable("WidgetArray")]
    public class WidgetArrayGridColumn : IGridColumn
    {
        [SetterProperty]
        public IWidget[] Widgets { get; set; }
    }
}