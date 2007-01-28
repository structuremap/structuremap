namespace StructureMap.Graph
{
    /// <summary>
    /// Specifies whether a PluginGraphObject is defined Explicitly in the configuration file,
    /// or implicitly through the [PluginFamily] or [Pluggable] attributes
    /// </summary>
    public enum DefinitionSource
    {
        Implicit,
        Explicit
    }
}