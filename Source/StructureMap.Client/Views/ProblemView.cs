using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
	[Pluggable("Problems")]
	public class ProblemView : IViewPart
	{
		private readonly bool _showPath;

		// Force StructureMap to create this class with this constructor
		[DefaultConstructor]
		public ProblemView() : this(false)
		{
			
		}

		public ProblemView(bool showPath)
		{
			_showPath = showPath;
		}

		public void WriteHTML(HTMLBuilder builder, GraphObject subject)
		{
			Problem[] problems = subject.Problems;
			WriteProblems(problems, builder);
		}

		public void WriteProblems(Problem[] problems, HTMLBuilder builder)
		{
			TableMaker table = createHeaders(problems, builder);
			foreach (Problem problem in problems)
			{
				writeRow(table, problem);
			}
		}

		private void writeRow(TableMaker table, Problem problem)
		{
			table.AddRow(false);
	
			if (_showPath)
			{
				table.AddReferenceLink(problem.Path, problem.ObjectId);
			}
	
			table.AddCell(problem.Title);
	
			CellMaker cell = table.CreateCell();
			cell.AddText(problem.Message);
			//cell.AddFormattedText(problem.Message);
		}

		private TableMaker createHeaders(Problem[] problems, HTMLBuilder builder)
		{
			if (problems.Length == 0)
			{
				builder.AddSubHeader("No Problems.");
			}
	
			builder.AddSubHeader("Problems");
	
			TableMaker table = builder.StartTable();
	
			table.AddRow(false);
			if (_showPath)
			{
				table.AddHeader("Object");
			}
	
			table.AddHeader("Title", 40);
			table.AddHeader("Message", 60);
			return table;
		}
	}
}
