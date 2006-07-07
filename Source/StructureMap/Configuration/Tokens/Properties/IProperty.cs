namespace StructureMap.Configuration.Tokens.Properties
{
	public interface IProperty
	{
		string PropertyName { get; }

		string PropertyType { get; }

		void Validate(IInstanceValidator validator);
	}
}