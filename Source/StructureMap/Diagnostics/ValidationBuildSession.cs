using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class ValidationBuildSession : BuildSession, IPipelineGraphVisitor
    {
        private ErrorCollection _errors;
        
        private readonly List<ValidationError> _validationErrors = new List<ValidationError>();
        private readonly Stack<BuildDependency> _dependencyStack = new Stack<BuildDependency>();
        private List<Instance> _explicitInstances;

        public ValidationBuildSession(PipelineGraph pipelineGraph, InterceptorLibrary interceptorLibrary)
            : base(pipelineGraph, interceptorLibrary)
        {
        }

        public ValidationBuildSession(PluginGraph graph) : base(graph)
        {
        }


        public override object CreateInstance(Type pluginType, Instance instance)
        {
            _dependencyStack.Push(new BuildDependency(pluginType, instance));

            try
            {
                return base.CreateInstance(pluginType, instance);
                _dependencyStack.Pop();
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

        public bool Success
        {
            get
            {
                return _errors.BuildErrors.Length == 0 && _validationErrors.Count == 0;
            }
        }

        public BuildError[] BuildErrors
        {
            get
            {
                return _errors.BuildErrors;
            }
        }

        public ValidationError[] ValidationErrors
        {
            get
            {
                return _validationErrors.ToArray();
            }
        }

        public BuildError Find(Type pluginType, string name)
        {
            return _errors.Find(pluginType, name);
        }

        

        void IPipelineGraphVisitor.PluginType(Type pluginType, Instance defaultInstance, IBuildPolicy policy)
        {
            // don't care
        }

        void IPipelineGraphVisitor.Instance(Type pluginType, Instance instance)
        {
            try
            {
                _dependencyStack.Clear();
                object builtInstance = CreateInstance(pluginType, instance);
                validate(pluginType, instance, builtInstance);
            }
#pragma warning disable EmptyGeneralCatchClause
            catch (Exception)
#pragma warning restore EmptyGeneralCatchClause
            {
                // All exceptions are being dealt with in another place
            }
        }

        public void Source(string source)
        {
            throw new NotImplementedException();
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
                catch(Exception ex)
                {
                    ValidationError error = new ValidationError(pluginType, instance, ex.InnerException, method);
                    _validationErrors.Add(error);
                }
            }
        }

        public void PerformValidations()
        {
            _explicitInstances = pipelineGraph.GetAllInstances();
            _errors = new ErrorCollection();

            pipelineGraph.Visit(this);
        }

        public string BuildErrorMessages()
        {
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
        
            _validationErrors.ForEach(e => e.Write(writer));
            _errors.ForEach(e => e.Write(writer));

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