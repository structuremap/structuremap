using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    [Pluggable("Reference")]
    public class ReferenceColumn : Column
    {
        public ReferenceColumn(string propertyName, string headerText) : base(propertyName, headerText)
        {
        }

        protected override void createCellContents(TableMaker maker, string cellText, GraphObject subject)
        {
            maker.AddReferenceLink(cellText, subject.Id);
        }
    }
}