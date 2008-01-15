using StructureMap.Client.Controllers;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;

namespace StructureMap.Client.Views
{
    [Pluggable(ViewConstants.SUMMARY)]
    public class SummaryView : IHTMLSource
    {
        private HTMLBuilder _builder = new HTMLBuilder();

        public SummaryView()
        {
            _builder.AddHeader("Summary");
        }

        #region IHTMLSource Members

        public string BuildHTML(GraphObject subject)
        {
            PluginGraphReport report = (PluginGraphReport) subject;

            buildSummaryReport(report);

            Problem[] problems = ProblemFinder.FindProblems(report);
            ProblemView view = new ProblemView(true);
            view.WriteProblems(problems, _builder);

            return _builder.HTML;
        }

        #endregion

        private void buildSummaryReport(PluginGraphReport report)
        {
            TableMaker maker = _builder.StartTable();
            addSummaryRow(maker, ViewConstants.ASSEMBLIES, report.Assemblies.Length);
            addSummaryRow(maker, ViewConstants.PLUGINFAMILIES, report.Families.Length);

            int pluginCount = 0;
            int instanceCount = 0;
            foreach (FamilyToken family in report.Families)
            {
                pluginCount += family.Plugins.Length;
                instanceCount += family.Instances.Length;
            }

            addSummaryRow(maker, ViewConstants.PLUGINS, pluginCount);
            addSummaryRow(maker, ViewConstants.INSTANCES, instanceCount);
        }

        private void addSummaryRow(TableMaker maker, string header, int count)
        {
            maker.AddRow(false);
            maker.AddHeader(header);
            maker.AddCell(count.ToString());
        }
    }
}