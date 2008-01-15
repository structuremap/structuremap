using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    [Pluggable("ChildHeader")]
    public class ChildHeader : IViewPart
    {
        private readonly string _headerText;

        public ChildHeader(string headerText)
        {
            _headerText = headerText;
        }

        #region IViewPart Members

        public void WriteHTML(HTMLBuilder builder, GraphObject subject)
        {
            builder.AddSubHeader(_headerText);
        }

        #endregion
    }
}