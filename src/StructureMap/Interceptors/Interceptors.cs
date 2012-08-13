namespace StructureMap.Interceptors
{
    public delegate object EnrichmentHandler<T>(T target);

    public delegate object ContextEnrichmentHandler<T>(IContext context, T target);
}