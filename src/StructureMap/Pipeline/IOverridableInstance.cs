namespace StructureMap.Pipeline
{
    internal interface IOverridableInstance
    {
        ConstructorInstance Override(ExplicitArguments arguments);
    }
}