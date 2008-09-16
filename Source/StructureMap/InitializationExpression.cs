namespace StructureMap
{
    public class InitializationExpression : ConfigurationExpression
    {
        internal InitializationExpression()
        {
            _parserBuilder.IgnoreDefaultFile = false;
            DefaultProfileName = string.Empty;
        }

        public bool UseDefaultStructureMapConfigFile
        {
            set { _parserBuilder.UseAndEnforceExistenceOfDefaultFile = value; }
        }

        public bool IgnoreStructureMapConfig
        {
            set { _parserBuilder.IgnoreDefaultFile = value; }
        }

        public bool PullConfigurationFromAppConfig
        {
            set { _parserBuilder.PullConfigurationFromAppConfig = value; }
        }

        public string DefaultProfileName { get; set; }
    }
}