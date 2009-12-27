using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap.Diagnostics
{
    public class ValidationBuildSession : BuildSession
    {
        private readonly Stack<BuildDependency> _dependencyStack = new Stack<BuildDependency>();
        private readonly List<ValidationError> _validationErrors = new List<ValidationError>();
        private ErrorCollection _errors;
        private List<Instance> _explicitInstances;

        public ValidationBuildSession(PipelineGraph pipelineGraph, InterceptorLibrary interceptorLibrary)
            : base(pipelineGraph, interceptorLibrary, new NulloObjectCache())
        {
        }

        public ValidationBuildSession(PluginGraph graph)
            : base(graph)
        {
        }


        public bool Success { get { return _errors.BuildErrors.Length == 0 && _validationErrors.Count == 0; } }

        public BuildError[] BuildErrors { get { return _errors.BuildErrors; } }

        public ValidationError[] ValidationErrors { get { return _validationErrors.ToArray(); } }

        public override object CreateInstance(Type pluginType, Instance instance)
        {
            _dependencyStack.Push(new BuildDependency(pluginType, instance));

            try
            {
                //clearBuildStack();
                return base.CreateInstance(pluginType, instance);
            }
            catch (StructureMapException ex)
            {
                _dependencyStack.Pop();

                // Don't log exceptions for inline instances.  I
                // think it would cause more confusion that not
                // because the name is a Guid
                if (!_explicitInstances.Contains(instance))
                {
                    throw;
                }

                _errors.LogError(instance, pluginType, ex, _dependencyStack);

                throw;
            }
        }

        public BuildError Find(Type pluginType, string name)
        {
            return _errors.Find(pluginType, name);
        }

        private void validateInstance(Type pluginType, Instance instance)
        {
            try
            {
                _dependencyStack.Clear();
                object builtInstance = CreateInstance(pluginType, instance);
                validate(pluginType, instance, builtInstance);
            }
            catch (Exception)
            {
                // All exceptions are being dealt with in another place
            }
        }

        private void validate(Type pluginType, Instance instance, object builtObject)
        {
            if (builtObject == null) return;

            MethodInfo[] methods = ValidationMethodAttribute.GetValidationMethods(builtObject.GetType());
            foreach (MethodInfo method in methods)
            {
                try
                {
                    method.Invoke(builtObject, new object[0]);
                }
                catch (Exception ex)
                {
                    var error = new ValidationError(pluginType, instance, ex.InnerException, method);
                    _validationErrors.Add(error);
                }
            }
        }

        public void PerformValidations()
        {
            _explicitInstances = pipelineGraph.GetAllInstances();
            _errors = new ErrorCollection();

            foreach (PluginTypeConfiguration pluginType in pipelineGraph.PluginTypes)
            {
                foreach (Instance instance in pluginType.Instances)
                {
                    _buildStack = new BuildStack();
                    validateInstance(pluginType.PluginType, instance);
                }
            }
        }

        public string BuildErrorMessages()
        {
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

            _validationErrors.ForEach(e => e.Write(writer));
            _errors.ForEach(e => e.Write(writer));

            writer.WriteLine();
            writer.WriteLine();


            writer.WriteLine("StructureMap Failures:  {0} Build/Configuration Failures and {1} Validation Errors",
                             _errors.BuildErrors.Length, _validationErrors.Count);

            return builder.ToString();
        }

        public bool HasBuildErrors()
        {
            return _errors.BuildErrors.Length > 0;
        }

        public bool HasValidationErrors()
        {
            return _validationErrors.Count > 0;
        }
    }
}