namespace StructureMap.Pipeline
{
    internal interface IOverridableInstance
    {
        Instance Override(ExplicitArguments arguments);
    }
}