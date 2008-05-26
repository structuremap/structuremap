using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class ValidationError
    {
        public ValidationError(Type pluginType, Instance instance, Exception exception, MethodInfo method)
        {
            PluginType = pluginType;
            Instance = instance;
            Exception = exception;
            MethodName = method.Name;
        }

        public Instance Instance;
        public Type PluginType;
        public Exception Exception;
        public string MethodName;
    }

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

        

        void IPipelineGraphVisitor.PluginType(Type pluginType, Instance defaultInstance)
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

        private void validate(Type pluginType, Instance instance, object builtObject)
        {
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
    }

}