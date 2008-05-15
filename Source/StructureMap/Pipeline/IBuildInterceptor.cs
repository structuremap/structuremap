namespace StructureMap.Pipeline
{
    [PluginFamily]
    public interface IBuildInterceptor : IBuildPolicy
    {
        IBuildPolicy InnerPolicy { get; set; }
    }
}