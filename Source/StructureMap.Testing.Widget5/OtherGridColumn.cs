using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    public class OtherGridColumn : IGridColumn
    {
        public IWidget Widget { get; set; }

        public string ReadOnly
        {
            get { return "whatever"; }
        }

        public FontStyleEnum FontStyle { get; set; }
        public string ColumnName { get; set; }
        public Rule[] Rules { get; set; }
        public bool WrapLines { get; set; }
        public bool Displayed { get; set; }
        public int Size { get; set; }
    }
}