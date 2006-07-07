namespace StructureMap.Caching
{
	public interface IValueSource
	{
		object GetValue(object Key);
	}
}