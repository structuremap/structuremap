using System;
using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    [PluginFamily]
    public interface IColumn
    {
        string HeaderText { get; }
        void Initialize(Type subjectType);

        void CreateCell(TableMaker maker, GraphObject subject);
    }
}