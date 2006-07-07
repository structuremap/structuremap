using System;
using System.Reflection;
using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
	[Pluggable("Grid")]
	public class GridView : IViewPart
	{
		private readonly string _memberName;
		private readonly IColumn[] _columns;

		public GridView(string memberName, IColumn[] columns)
		{
			_memberName = memberName;
			_columns = columns;
		}

		public void WriteHTML(HTMLBuilder builder, GraphObject subject)
		{
			TableMaker table = builder.StartTable();
			table.AddRow(false);

			createHeaderRow(subject, table);

			PropertyInfo property = subject.GetType().GetProperty(_memberName);
			GraphObject[] displayValues = (GraphObject[]) property.GetValue(subject, null);
			Array.Sort(displayValues);

			Type memberType = property.PropertyType.GetElementType();
			foreach (IColumn column in _columns)
			{
				column.Initialize(memberType);
			}

			foreach (GraphObject displayValue in displayValues)
			{
				writeRow(table, displayValue);
			}
		}

		private void writeRow(TableMaker table, GraphObject displayValue)
		{
			bool hasErrors = (displayValue.Problems.Length > 0);

			table.AddRow(hasErrors);
			foreach (IColumn column in _columns)
			{
				column.CreateCell(table, displayValue);
			}
		}

		private void createHeaderRow(GraphObject subject, TableMaker table)
		{
			foreach (IColumn column in _columns)
			{
				table.AddHeader(column.HeaderText);
			}
		}
	}
}
