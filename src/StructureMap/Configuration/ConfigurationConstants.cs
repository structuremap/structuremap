namespace StructureMap.Configuration
{
    public static class ConfigurationConstants
    {
        public const string CONFIGURED_DEFAULT_KEY_CANNOT_BE_FOUND =
            "The default instance key configured for this PluginFamily cannot be found";

        public const string COULD_NOT_CREATE_INSTANCE = "Cannot create the configured InstanceMemento";
        public const string COULD_NOT_CREATE_MEMENTO_SOURCE = "Could not create the externally configured MementoSource";

        public const string COULD_NOT_LOAD_ASSEMBLY = "Could not load Assembly into target AppDomain";
        public const string COULD_NOT_LOAD_TYPE = "Could not load Type into target AppDomain";

        public const string FATAL_ERROR =
            "A fatal error in configuration is preventing StructureMap from functioning correctly";

        public const string INVALID_ENUMERATION_VALUE = "Property value is not a valid name for this Enumeration type";

        public const string INVALID_PLUGIN = "Requested ConcreteKey for this PluginType cannot be found";
        public const string INVALID_PLUGIN_FAMILY = "The PluginFamily is not configured in StructureMap";

        public const string INVALID_PROPERTY_CAST =
            "Property value in the configured InstanceMemento could not be cast to the target type";

        public const string INVALID_SETTER = "Requested Setter property does not exist";
        public const string MEMENTO_PROPERTY_IS_MISSING = "Property is missing from the InstanceMemento configuration";

        public const string MEMENTO_SOURCE_CANNOT_RETRIEVE =
            "The configured MementoSource cannot retrieve InstanceMemento objects";

        public const string MISSING_CHILD = "Child memento is not defined";
        public const string MISSING_INSTANCE_KEY = "InstanceKey is required";

        public const string NO_DEFAULT_INSTANCE_CONFIGURED = "No default instance is configured for this PluginFamily";
        public const string NO_MATCHING_INSTANCE_CONFIGURED = "No matching instance is configured for this PluginFamily";

        public const string PLUGIN_CANNOT_READ_CONSTRUCTOR_PROPERTIES =
            "There was an error trying to determine the constructor arguments for a Plugin.  Check for missing dependencies of the concrete type.";

        public const string PLUGIN_FAMILY_CANNOT_BE_FOUND_FOR_INSTANCE =
            "No matching PluginFamily for the embedded memento in the <Instances> node";

        public const string PLUGIN_IS_MISSING_CONCRETE_KEY = "Plugin definition is missing a value for ConcreteKey";

        public const string UNKNOWN_PLUGIN_PROBLEM = "Exception occurred while attaching a Plugin to a PluginFamily";
        public const string VALIDATION_METHOD_FAILURE = "A Validation Method Failed";
    }
}