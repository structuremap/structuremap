using System;
using System.Data;
using System.Text;
using StructureMap.DataAccess.Parameters;

namespace StructureMap.DataAccess.Commands
{
	[Pluggable("Templated")]
	public class TemplatedCommand : CommandBase
	{
		private string[] _substitutions;
		private string _template;

		[DefaultConstructor]
		public TemplatedCommand(string template) : base()
		{
			_template = template;
			TemplateParser parser = new TemplateParser(template);
			_substitutions = parser.Parse();
		}

		public TemplatedCommand(string template, IDatabaseEngine engine)
			: this(template)
		{
			this.Initialize(engine);			
		}

		public override void Initialize(IDatabaseEngine engine)
		{
			ParameterCollection parameters = new ParameterCollection();
			foreach (string substitution in _substitutions)
			{
				TemplateParameter parameter = new TemplateParameter(substitution);
				parameters.AddParameter(parameter);
			}

			IDbCommand command = engine.GetCommand();
			this.initializeMembers(parameters, command);
		}

		public string[] Substitutions
		{
			get { return _substitutions; }
		}

		public string GetSql()
		{
			StringBuilder sb = new StringBuilder(_template);
			foreach (TemplateParameter parameter in this.Parameters)
			{
				parameter.Substitute(sb);
			}

			return sb.ToString();
		}

		protected override void prepareCommand(IDbCommand command)
		{
			command.CommandText = this.GetSql();
		}



	}
}
