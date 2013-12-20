using StructureMap.Attributes;
using StructureMap.LegacyAttributeSupport;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    [Pluggable("Basic")]
    public class BasicGridColumn : IGridColumn
    {
        private readonly string _headerText;

        public BasicGridColumn(string headerText)
        {
            _headerText = headerText;
        }

        public string HeaderText { get { return _headerText; } }

        [SetterProperty]
        public IWidget Widget { get; set; }

        [SetterProperty]
        public FontStyleEnum FontStyle { get; set; }

        [SetterProperty]
        public string ColumnName { get; set; }

        [SetterProperty]
        public Rule[] Rules { get; set; }

        [SetterProperty]
        public bool WrapLines { get; set; }

        public bool Displayed { get; set; }

        public int Size { get; set; }
    }
}