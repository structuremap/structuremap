using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class ErrorCollection
    {
        private readonly List<Instance> _brokenInstances = new List<Instance>();
        private readonly Dictionary<Instance, BuildError> _buildErrors = new Dictionary<Instance, BuildError>();

        public BuildError[] BuildErrors
        {
            get
            {
                var errors = new BuildError[_buildErrors.Count];
                _buildErrors.Values.CopyTo(errors, 0);

                return errors;
            }
        }


        public void LogError(
            Instance instance,
            Type pluginType,
            StructureMapException ex,
            IEnumerable<BuildDependency> dependencies)
        {
            if (_buildErrors.ContainsKey(instance))
            {
                var existingError = _buildErrors[instance];
                addDependenciesToError(instance, dependencies, existingError);
            }

            if (_brokenInstances.Contains(instance))
            {
                return;
            }

            var token = ((IDiagnosticInstance) instance).CreateToken();
            var error = new BuildError(pluginType, instance);
            error.Exception = ex;

            _buildErrors.Add(instance, error);

            addDependenciesToError(instance, dependencies, error);
        }

        private void addDependenciesToError(Instance instance, IEnumerable<BuildDependency> dependencies,
            BuildError error)
        {
            foreach (var dependency in dependencies)
            {
                if (_brokenInstances.Contains(instance))
                {
                    continue;
                }

                error.AddDependency(dependency);
                _brokenInstances.Add(dependency.Instance);
            }
        }

        public BuildError Find(Type pluginType, string name)
        {
            foreach (var pair in _buildErrors)
            {
                var error = pair.Value;
                if (error.PluginType == pluginType && error.Instance.Name == name)
                {
                    return error;
                }
            }

            return null;
        }

        public void ForEach(Action<BuildError> action)
        {
            foreach (var pair in _buildErrors)
            {
                action(pair.Value);
            }
        }
    }
}