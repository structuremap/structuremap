using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class ProfileReport
    {
        private readonly ContainerRole _role;
        private readonly string _profile;
        private readonly IList<BuildError> _errors = new List<BuildError>();
        private readonly IList<ValidationError> _validations = new List<ValidationError>();

        public ProfileReport(IPipelineGraph pipelineGraph)
        {
            _role = pipelineGraph.Role;
            _profile = pipelineGraph.Profile;
        }

        public ContainerRole Role
        {
            get { return _role; }
        }

        public string Profile
        {
            get { return _profile; }
        }

        public void AddError(Type pluginType, Instance instance, StructureMapException ex)
        {
            var rootCause = ex.Instances.First();
            var rootProblem = _errors.FirstOrDefault(x => x.RootInstance == rootCause);

            if (rootProblem != null)
            {
                rootProblem.AddDependency(new BuildDependency(pluginType, instance));
            }
            else
            {
                var error = new BuildError(pluginType, instance)
                {
                    Exception = ex
                };

                _errors.Add(error);
            }
        }

        public bool HasAnyErrors()
        {
            return _errors.Any() || _validations.Any();
        }

        public void AddValidationError(ValidationError error)
        {
            _validations.Add(error);
        }

        public void WriteErrorMessages(StringWriter writer)
        {
            writer.WriteLine("Profile '{0}'", _profile);

            _validations.Each(e => e.Write(writer));
            _errors.Each(e => e.Write(writer));
        }

        public IEnumerable<BuildError> Errors
        {
            get { return _errors; }
        }

        public IEnumerable<ValidationError> Validations
        {
            get { return _validations; }
        }
    }
}