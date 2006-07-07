using System;
using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
	[PluginFamily]
	public interface IColumn
	{
		void Initialize(Type subjectType);

		string HeaderText { get; }

		void CreateCell(TableMaker maker, GraphObject subject);
	}
}