using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    // TODO -- think this just gets a redo. Just try to build everything you know about.
    public class ValidationBuildSession : BuildSession
    {
        private readonly Stack<BuildDependency> _dependencyStack = new Stack<BuildDependency>();
        private readonly List<ValidationError> _validationErrors = new List<ValidationError>();
        private ErrorCollection _errors;
        private IEnumerable<Instance> _explicitInstances;

        public ValidationBuildSession(IPipelineGraph pipelineGraph)
            : base(pipelineGraph)
        {
        }


        public bool Success
        {
            get { return _errors.BuildErrors.Length == 0 && _validationErrors.Count == 0; }
        }

        public BuildError[] BuildErrors
        {
            get { return _errors.BuildErrors; }
        }

        public ValidationError[] ValidationErrors
        {
            get { return _validationErrors.ToArray(); }
        }

        public static ValidationBuildSession ValidateForPluginGraph(PluginGraph graph)
        {
            var pipeline = PipelineGraph.BuildRoot(graph);

            return new ValidationBuildSession(pipeline);
        }

        public override object FindObject(Type pluginType, Instance instance)
        {
            _dependencyStack.Push(new BuildDependency(pluginType, instance));

            try
            {
                //clearBuildStack();
                return base.FindObject(pluginType, instance);
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
                var builtInstance = FindObject(pluginType, instance);
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

            var methods = ValidationMethodAttribute.GetValidationMethods(builtObject.GetType());
            foreach (var method in methods)
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
            _explicitInstances = pipelineGraph.Instances.GetAllInstances();
            _errors = new ErrorCollection();

            pipelineGraph.Instances.EachInstance((t, i) => {
                if (t.IsOpenGeneric()) return;

                validateInstance(t, i);
            });
        }

        public string BuildErrorMessages()
        {
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

            _validationErrors.Each(e => e.Write(writer));
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