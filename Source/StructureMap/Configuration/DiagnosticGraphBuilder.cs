using System;
using StructureMap.Attributes;
using StructureMap.Configuration.Tokens;
using StructureMap.Graph;
using StructureMap.Graph.Configuration;
using StructureMap.Interceptors;

namespace StructureMap.Configuration
{
    public class DiagnosticGraphBuilder : IGraphBuilder
    {
        private NormalGraphBuilder _innerBuilder;
        private InstanceValidator _systemValidator;
        private PluginGraphReport _systemReport;
        private PluginGraphReport _report;

        public DiagnosticGraphBuilder()
        {
            _innerBuilder = new NormalGraphBuilder();
            _systemReport = new PluginGraphReport();
            _report = _innerBuilder.PluginGraph.Report;
        }

        public PluginGraph PluginGraph
        {
            get { return _innerBuilder.PluginGraph; }
        }

        public void AddProfile(string profileName)
        {
            _innerBuilder.AddProfile(profileName);
        }

        public void OverrideProfile(string fullTypeName, string instanceKey)
        {
            _innerBuilder.OverrideProfile(fullTypeName, instanceKey);
        }

        public void AddMachine(string machineName, string profileName)
        {
            try
            {
                _innerBuilder.AddMachine(machineName, profileName);
            }
            catch (Exception e)
            {
                _report.LogProblem(new Problem("Could not add Machine", e));
            }
        }

        public void OverrideMachine(string fullTypeName, string instanceKey)
        {
            _innerBuilder.OverrideMachine(fullTypeName, instanceKey);
        }

        public TypePath LocateOrCreateFamilyForType(string fullName)
        {
            try
            {
                return _innerBuilder.LocateOrCreateFamilyForType(fullName);
            }
            catch (Exception e)
            {
                string message =
                    string.Format("Could not find the implied type '{0}' in any of the designated Assemblies", fullName);
                _report.LogProblem(new Problem(message, e));

                return null;
            }
        }

        public void AddAssembly(string assemblyName, string[] deployableTargets)
        {
            AssemblyToken assemblyToken = new AssemblyToken(assemblyName, deployableTargets);
            _report.AddAssembly(assemblyToken);
            _systemReport.AddAssembly(assemblyToken);

            try
            {
                _innerBuilder.AddAssembly(assemblyName, deployableTargets);
            }
            catch (Exception ex)
            {
                assemblyToken.MarkLoadFailure(ex);
            }
        }


        public void StartFamilies()
        {
            _innerBuilder.StartFamilies();
            InstanceManager systemInstanceManager = new InstanceManager(_innerBuilder.SystemGraph);
            _systemValidator =
                new InstanceValidator(_innerBuilder.SystemGraph, new Profile("defaults"), systemInstanceManager);
            _systemReport.ImportImplicitChildren(SystemGraph);
        }

        public void AddPluginFamily(TypePath typePath, string defaultKey, string[] deploymentTargets,
                                    InstanceScope scope)
        {
            FamilyToken family = new FamilyToken(typePath, defaultKey, deploymentTargets);
            family.DefinitionSource = DefinitionSource.Explicit;
            family.Scope = scope;
            _report.AddFamily(family);

            try
            {
                Type type = typePath.FindType();
                _innerBuilder.AddPluginFamily(typePath, defaultKey, deploymentTargets, scope);
            }
            catch (Exception ex)
            {
                family.MarkAsInvalidType(ex);
            }
        }


        public void AttachSource(TypePath pluginTypePath, InstanceMemento sourceMemento)
        {
            FamilyToken family = _report.FindFamily(pluginTypePath);

            MementoSourceInstanceToken sourceInstance =
                new MementoSourceInstanceToken(typeof (MementoSource), _systemReport, sourceMemento);
            family.SourceInstance = sourceInstance;
            sourceInstance.Validate(_systemValidator);

            try
            {
                _innerBuilder.AttachSource(pluginTypePath, sourceMemento);
            }
            catch (Exception ex)
            {
                Problem problem = new Problem(ConfigurationConstants.COULD_NOT_CREATE_MEMENTO_SOURCE, ex);
                family.LogProblem(problem);
            }
        }

        public void AttachSource(TypePath pluginTypePath, MementoSource source)
        {
            _innerBuilder.AttachSource(pluginTypePath, source);
        }

        public Plugin AddPlugin(TypePath pluginTypePath, TypePath pluginPath, string concreteKey)
        {
            PluginToken pluginToken = new PluginToken(pluginPath, concreteKey, DefinitionSource.Explicit);
            FamilyToken familyToken = _report.FindFamily(pluginTypePath);
            familyToken.AddPlugin(pluginToken);

            Plugin returnValue = null;

            try
            {
                Plugin plugin = _innerBuilder.AddPlugin(pluginTypePath, pluginPath, concreteKey);
                pluginToken.ReadProperties(plugin);
                returnValue = plugin;
            }
            catch (StructureMapException ex)
            {
                if (ex.ErrorCode == 112)
                {
                    Problem problem = new Problem(ConfigurationConstants.PLUGIN_IS_MISSING_CONCRETE_KEY, ex);
                    pluginToken.LogProblem(problem);
                }
                else
                {
                    Problem problem = new Problem(ConfigurationConstants.COULD_NOT_LOAD_TYPE, ex);
                    pluginToken.LogProblem(problem);
                }
            }

            return returnValue;
        }

        public SetterProperty AddSetter(TypePath pluginTypePath, string concreteKey, string setterName)
        {
            FamilyToken familyToken = _report.FindFamily(pluginTypePath);
            PluginToken pluginToken = familyToken.FindPlugin(concreteKey);

            SetterProperty setter = null;

            try
            {
                setter = _innerBuilder.AddSetter(pluginTypePath, concreteKey, setterName);
                PropertyDefinition property = PropertyDefinitionBuilder.CreatePropertyDefinition(setter.Property);
                pluginToken.AddPropertyDefinition(property);
            }
            catch (Exception ex)
            {
                PropertyDefinition property =
                    new PropertyDefinition(setterName, PropertyDefinitionType.Setter, ArgumentType.Primitive);
                pluginToken.AddPropertyDefinition(property);
                Problem problem = new Problem(ConfigurationConstants.INVALID_SETTER, ex);

                property.LogProblem(problem);
            }

            return setter;
        }

        public void AddInterceptor(TypePath pluginTypePath, InstanceMemento interceptorMemento)
        {
            InstanceToken instance =
                new InterceptorInstanceToken(typeof (InstanceFactoryInterceptor), _systemReport, interceptorMemento);
            instance.Validate(_systemValidator);
            FamilyToken family = _report.FindFamily(pluginTypePath);
            family.AddInterceptor(instance);

            try
            {
                _innerBuilder.AddInterceptor(pluginTypePath, interceptorMemento);
            }
            catch (Exception)
            {
                // no-op;  The call above to instance.Validate(_systemValidator) will find the Problem
            }
        }


        public void FinishFamilies()
        {
            _innerBuilder.FinishFamilies();
        }

        public PluginGraph CreatePluginGraph()
        {
            PluginGraph pluginGraph = _innerBuilder.CreatePluginGraph();
            _report.ImportImplicitChildren(pluginGraph);
            _report.AnalyzeInstances(pluginGraph);

            Profile defaultProfile = _innerBuilder.DefaultManager.CalculateDefaults();

            InstanceManager manager = new InstanceManager();
            try
            {
                manager = new InstanceManager(pluginGraph);
            }
            catch (Exception ex)
            {
                Problem problem = new Problem(ConfigurationConstants.FATAL_ERROR, ex);
                _report.LogProblem(problem);
            }

            IInstanceValidator validator = new InstanceValidator(pluginGraph, defaultProfile, manager);
            _report.ValidateInstances(validator);

            return pluginGraph;
        }

        public PluginGraph SystemGraph
        {
            get { return _innerBuilder.SystemGraph; }
        }

        public PluginGraphReport Report
        {
            get { return _report; }
        }

        public InstanceDefaultManager DefaultManager
        {
            get { return _innerBuilder.DefaultManager; }
        }

        public void RegisterMemento(TypePath pluginTypePath, InstanceMemento memento)
        {
            try
            {
                _innerBuilder.RegisterMemento(pluginTypePath, memento);
            }
            catch (Exception ex)
            {
                Problem problem = new Problem(ConfigurationConstants.PLUGIN_FAMILY_CANNOT_BE_FOUND_FOR_INSTANCE, ex);
                _report.LogProblem(problem);
            }
        }
    }
}