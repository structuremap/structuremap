using System.Data;
using System.Text;
using StructureMap.DataAccess.Parameters;

namespace StructureMap.DataAccess.Commands
{
    [Pluggable("Templated")]
    public class TemplatedCommand : CommandBase
    {
        private readonly string[] _substitutions;
        private readonly string _template;

        [DefaultConstructor]
        public TemplatedCommand(string template)
        {
            _template = template;
            var parser = new TemplateParser(template);
            _substitutions = parser.Parse();
        }

        public TemplatedCommand(string template, IDatabaseEngine engine)
            : this(template)
        {
            Initialize(engine);
        }

        public string[] Substitutions
        {
            get { return _substitutions; }
        }

        public override void Initialize(IDatabaseEngine engine)
        {
            var parameters = new ParameterCollection();
            foreach (string substitution in _substitutions)
            {
                var parameter = new TemplateParameter(substitution);
                parameters.AddParameter(parameter);
            }

            IDbCommand command = engine.GetCommand();
            initializeMembers(parameters, command);
        }

        public string GetSql()
        {
            var sb = new StringBuilder(_template);
            foreach (TemplateParameter parameter in Parameters)
            {
                parameter.Substitute(sb);
            }

            return sb.ToString();
        }

        protected override void prepareCommand(IDbCommand command)
        {
            command.CommandText = GetSql();
        }
    }
}