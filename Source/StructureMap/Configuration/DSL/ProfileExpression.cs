using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    /// <summary>
    /// Expression class to help define a runtime Profile
    /// </summary>
    public class ProfileExpression : IExpression
    {
        private readonly string _profileName;
        private List<InstanceDefaultExpression> _defaults = new List<InstanceDefaultExpression>();

        public ProfileExpression(string profileName)
        {
            _profileName = profileName;
        }

        void IExpression.Configure(PluginGraph graph)
        {
            Profile profile = graph.DefaultManager.GetProfile(_profileName);
            if (profile == null)
            {
                profile = new Profile(_profileName);
                graph.DefaultManager.AddProfile(profile);
            }

            foreach (InstanceDefaultExpression expression in _defaults)
            {
                expression.Configure(profile, graph);
            }
        }

        /// <summary>
        /// Starts the definition of the default instance for the containing Profile
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public InstanceDefaultExpression For<T>()
        {
            InstanceDefaultExpression defaultExpression = new InstanceDefaultExpression(typeof (T), this);
            _defaults.Add(defaultExpression);

            return defaultExpression;
        }
    }
}