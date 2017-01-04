namespace StructureMap.AutoFactory
{
    public enum AutoFactoryMethodType
    {
        /// <summary>
        /// Method is used to create an instance i.e. factory method.
        /// </summary>
        GetInstance = 0,

        /// <summary>
        /// Special method to return the list of names of registered implementations.
        /// </summary>
        GetNames = 1
    }
}