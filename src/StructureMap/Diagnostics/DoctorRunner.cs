using System;
using System.Security;
using StructureMap.Exceptions;
using StructureMap.Graph;

namespace StructureMap.Diagnostics
{
    public class DoctorRunner : MarshalByRefObject
    {
        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public DoctorReport RunReport(string bootstrapperTypeName)
        {
            var report = new DoctorReport
            {
                Result = DoctorResult.Success
            };


            IBootstrapper bootstrapper;
            try
            {
                var path = new TypePath(bootstrapperTypeName);
                Type bootstrapperType = path.FindType();
                bootstrapper = (IBootstrapper) Activator.CreateInstance(bootstrapperType);
            }
            catch (Exception e)
            {
                report.Result = DoctorResult.BootstrapperCouldNotBeFound;
                report.ErrorMessages = e.ToString();

                return report;
            }

            try
            {
                bootstrapper.BootstrapStructureMap();

                // TODO -- OMG this sucks
                var graph = (PluginGraph)ObjectFactory.Container.Model.PluginGraph;

                if (graph.Log.ErrorCount > 0)
                {
                    report.ErrorMessages = graph.Log.BuildFailureMessage();
                    report.Result = DoctorResult.ConfigurationErrors;
                }
                else
                {
                    writeConfigurationAndValidate(report, graph);
                }


                return report;
            }
            catch (StructureMapConfigurationException ex)
            {
                report.ErrorMessages = ex.Message;
                report.Result = DoctorResult.ConfigurationErrors;

                return report;
            }
            catch (Exception ex)
            {
                report.Result = DoctorResult.BootstrapperFailure;
                report.ErrorMessages = ex.ToString();

                return report;
            }
        }

        private void writeConfigurationAndValidate(DoctorReport report, PluginGraph graph)
        {
            var pipelineGraph = new RootPipelineGraph(graph);
            var writer = new WhatDoIHaveWriter(pipelineGraph);
            report.WhatDoIHave = writer.GetText();

            var session = ValidationBuildSession.ValidateForPluginGraph(graph);
            session.PerformValidations();

            if (session.HasBuildErrors())
            {
                report.Result = DoctorResult.BuildErrors;
            }
            else if (session.HasValidationErrors())
            {
                report.Result = DoctorResult.ValidationErrors;
            }

            if (!session.Success)
            {
                report.ErrorMessages = session.BuildErrorMessages();
            }
        }
    }
}