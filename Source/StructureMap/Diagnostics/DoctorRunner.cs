using System;
using StructureMap.Graph;

namespace StructureMap.Diagnostics
{
    public class DoctorRunner : MarshalByRefObject
    {
        public DoctorRunner()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public DoctorReport RunReport(string bootstrapperTypeName)
        {
            var report = new DoctorReport(){Result = DoctorResult.Success};
            

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
            }
            catch (Exception ex)
            {
                report.Result = DoctorResult.BootstrapperFailure;
                report.ErrorMessages = ex.ToString();

                return report;
            }

            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();

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

        private void writeConfigurationAndValidate(DoctorReport report, PluginGraph graph)
        {
            var pipelineGraph = new PipelineGraph(graph);
            var writer = new WhatDoIHaveWriter(pipelineGraph);
            report.WhatDoIHave = writer.GetText();

            ValidationBuildSession session = new ValidationBuildSession(pipelineGraph, graph.InterceptorLibrary);
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