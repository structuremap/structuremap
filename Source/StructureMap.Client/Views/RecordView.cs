using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    [Pluggable("Record")]
    public class RecordView : IViewPart
    {
        private readonly IColumn[] _columns;

        public RecordView(IColumn[] columns)
        {
            _columns = columns;
        }

        #region IViewPart Members

        public void WriteHTML(HTMLBuilder builder, GraphObject subject)
        {
            TableMaker table = builder.StartTable();

            foreach (IColumn column in _columns)
            {
                column.Initialize(subject.GetType());
                table.AddRow(false);
                table.AddHeader(column.HeaderText);
                column.CreateCell(table, subject);
            }
        }

        #endregion
    }
}