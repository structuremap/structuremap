using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.TypeRules;

namespace StructureMap.Diagnostics
{
    public class PipelineGraphValidator
    {
        private readonly IList<Guid> _buildPlanFailureIds = new List<Guid>();
        private readonly IList<ProfileReport> _reports = new List<ProfileReport>();

        public static void AssertNoErrors(IPipelineGraph graph)
        {
            var validator = new PipelineGraphValidator();
            findPipelines(graph).Each(x => validator.Validate(graph, graph.Policies));

            validator.AssertIsValid();
        }

        public void AssertIsValid()
        {
            var reports = _reports.Where(x => x.HasAnyErrors()).ToArray();
            if (reports.Any())
            {
                var errorCount = reports.Sum(x => x.Errors.Count());
                var validationCount = reports.Sum(x => x.Validations.Count());
                var title = "StructureMap Failures:  {0} Build/Configuration Failures and {1} Validation Errors".ToFormat(errorCount, validationCount);

                var writer = new StringWriter();
                reports.Each(x => x.WriteErrorMessages(writer));

                var ex = new StructureMapConfigurationException(title) {Context = writer.ToString()};

                throw ex;
            }
        }

        private static IEnumerable<IPipelineGraph> findPipelines(IPipelineGraph graph)
        {
            yield return graph;

            if (graph.Role == ContainerRole.Root)
            {
                foreach (var profile in graph.Profiles.AllProfiles())
                {
                    yield return profile;
                }
            }
        } 

        public void Validate(IPipelineGraph pipeline, Policies policies)
        {
            var report = new ProfileReport(pipeline);
            _reports.Add(report);

            var closedTypes = pipeline.ToModel().PluginTypes.Where(x => !x.PluginType.IsOpenGeneric()).ToArray();

            closedTypes.Each(family => {
                family.Instances.Each(i => {
                    tryCreateBuildPlan(family.PluginType, i, policies, report);
                });
            });

            closedTypes.Each(family => {
                family.Instances.Where(x => !_buildPlanFailureIds.Contains(x.Instance.Id)).Each(i => {
                    tryBuildInstance(family.PluginType, i.Instance, pipeline, report);
                });
            });
        }

        private void tryBuildInstance(Type pluginType, Instance instance, IPipelineGraph pipeline, ProfileReport report)
        {
            var session = new BuildSession(pipeline, instance.Name);

            try
            {
                var @object = session.FindObject(pluginType, instance);
                validate(pluginType, instance, @object, report);
            }
            catch (StructureMapException ex)
            {
                ex.Instances.Each(x => _buildPlanFailureIds.Fill(x));

                report.AddError(pluginType, instance, ex);
            }
        }

        private void tryCreateBuildPlan(Type pluginType, InstanceRef instanceRef, Policies policies, ProfileReport report)
        {
            try
            {
                instanceRef.Instance.ResolveBuildPlan(pluginType, policies);
            }
            catch (StructureMapException e)
            {
                _buildPlanFailureIds.Add(instanceRef.Instance.Id);
                e.Instances.Fill(instanceRef.Instance.Id);
                report.AddError(pluginType, instanceRef.Instance, e);
            }

        }

        private void validate(Type pluginType, Instance instance, object builtObject, ProfileReport report)
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
                    report.AddValidationError(error);
                }
            }
        }
    }
}