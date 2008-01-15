using System.Collections;
using System.Data;
using System.Text;
using StructureMap.DataAccess.Parameters;

namespace StructureMap.DataAccess.Commands
{
    [Pluggable("TemplatedQuery")]
    public class TemplatedQuery : CommandBase
    {
        private readonly IQueryFilter[] _filters;
        private readonly string _selectAndFromClause;
        private ArrayList _templatedParameters = new ArrayList();

        public TemplatedQuery(string selectAndFromClause, IQueryFilter[] filters) : base()
        {
            _selectAndFromClause = selectAndFromClause;
            _filters = filters;
        }

        public override void Initialize(IDatabaseEngine engine)
        {
            ParameterCollection parameters = new ParameterCollection(_filters);
            IDbCommand command = engine.GetCommand();

            TemplateParser parser = new TemplateParser(_selectAndFromClause);
            string[] substitutions = parser.Parse();
            foreach (string substitution in substitutions)
            {
                TemplateParameter parameter = new TemplateParameter(substitution);
                _templatedParameters.Add(parameter);
                parameters.AddParameter(parameter);
            }

            foreach (IQueryFilter filter in _filters)
            {
                filter.Initialize(engine, command);
            }

            initializeMembers(parameters, command);
        }

        protected override void prepareCommand(IDbCommand command)
        {
            command.Parameters.Clear();

            StringBuilder sb = new StringBuilder(_selectAndFromClause);
            foreach (TemplateParameter parameter in _templatedParameters)
            {
                parameter.Substitute(sb);
            }

            ArrayList whereList = new ArrayList();
            foreach (IQueryFilter filter in _filters)
            {
                if (filter.IsActive())
                {
                    whereList.Add(filter.GetWhereClause());
                    filter.AttachParameters(command);
                }
            }

            if (whereList.Count > 0)
            {
                sb.Append(" where ");
                string[] filterStrings = (string[]) whereList.ToArray(typeof (string));
                string whereClause = string.Join(" and ", filterStrings);
                sb.Append(whereClause);
            }

            command.CommandText = sb.ToString();
        }

        /// <summary>
        /// Testing method
        /// </summary>
        /// <returns></returns>
        public IDbCommand ConfigureInnerCommand()
        {
            prepareCommand(innerCommand);
            return innerCommand;
        }
    }
}