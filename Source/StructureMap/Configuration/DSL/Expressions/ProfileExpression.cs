using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Expression class to help define a runtime Profile
    /// </summary>
    public class ProfileExpression : IExpression
    {
        private readonly List<InstanceDefaultExpression> _defaults = new List<InstanceDefaultExpression>();
        private readonly string _profileName;

        public ProfileExpression(string profileName)
        {
            _profileName = profileName;
        }

        #region IExpression Members

        void IExpression.Configure(PluginGraph graph)
        {
            foreach (InstanceDefaultExpression expression in _defaults)
            {
                expression.Configure(_profileName, graph);
            }
        }

        #endregion

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