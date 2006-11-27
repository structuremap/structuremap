using System;
using StructureMap.DataAccess.Commands;

namespace StructureMap.DataAccess.Parameterization
{
	[Pluggable("Parameterized")]
	public class ParameterizedCommand : CommandBase
	{
		private readonly string _commandText;

		public ParameterizedCommand(string template)
		{
			_commandText = template;
		}

		public override void Initialize(IDatabaseEngine engine)
		{
			ParameterizedCommandBuilder builder = new ParameterizedCommandBuilder(engine, _commandText);
			builder.Build();

			this.initializeMembers(builder.Parameters, builder.Command);
		}
	}
}
