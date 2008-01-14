namespace StructureMap.Interceptors
{
    public delegate T EnrichmentHandler<T>(T target);

    public delegate void StartupHandler<T>(T target);
}
