namespace StructureMap.Pipeline
{
    public enum TransientTracking
    {
        /// <summary>
        /// The classic default StructureMap behavior where NO transient objects
        /// created by the root or child containers will be tracked by the Container.
        /// </summary>
        DefaultNotTrackedAtRoot,

        /// <summary>
        /// The root Container tracks any Transient objects that implement IDisposable created *outside* of 
        /// nested containers. Users are responsible for explicitly calling IContainer.Release(object).
        /// USE WITH CAUTION AS NAIVE USAGE OF THIS FEATURE CAN LEAD TO MEMORY LEAK PROBLEMS
        /// </summary>
        ExplicitReleaseMode
    }
}