using StructureMap.Attributes;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    public class BasicGridColumn : IGridColumn
    {
        public BasicGridColumn(string headerText)
        {
            HeaderText = headerText;
        }

        public string HeaderText { get; }

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