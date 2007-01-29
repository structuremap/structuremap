using System;
using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    [Pluggable("Problems")]
    public class ProblemColumn : IColumn
    {
        public ProblemColumn()
        {
        }

        public void Initialize(Type subjectType)
        {
            // no-op;
        }

        public string HeaderText
        {
            get { return "Problems"; }
        }

        public void CreateCell(TableMaker maker, GraphObject subject)
        {
            int problemCount = subject.Problems.Length;
            if (problemCount > 0)
            {
                maker.AddCenteredCell(problemCount.ToString());
            }
            else
            {
                maker.AddCell("");
            }
        }
    }
}