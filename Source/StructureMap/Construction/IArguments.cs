namespace StructureMap.Construction
{
    public interface IArguments
    {
        T Get<T>(string propertyName);
        bool Has(string propertyName);
    }
}