using System.Reflection;
using System.Xml;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Graph;
using StructureMap.Testing.Configuration.Tokens;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration
{
	[TestFixture]
	public class DiagnosticGraphBuilderTester
	{
		private DiagnosticGraphBuilder _builder;
		private PluginGraphReport _report;

		[SetUp]
		public void SetUp()
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml("<StructureMap/>");

			_builder = new DiagnosticGraphBuilder(new InstanceDefaultManager());
			_report = _builder.Report;
		}

		[Test]
		public void AddAssembly()
		{
			Assert.AreEqual(0, _report.Assemblies.Length);
			string theAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			string[] theTargets = new string[]{"Client", "Server"};

			_builder.AddAssembly(theAssemblyName, theTargets);

			Assert.AreEqual(1, _report.Assemblies.Length);
			AssemblyToken actual = _report.Assemblies[0];
			AssemblyToken expected = new AssemblyToken(theAssemblyName, theTargets);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void AddNonExistentOrUnreachableAssembly()
		{
			_builder.AddAssembly("SomethingThatDoesNotExist", new string[0]);
			Problem expected = new Problem(ConfigurationConstants.COULD_NOT_LOAD_ASSEMBLY, string.Empty);
			Assert.AreEqual(new Problem[]{expected}, _report.Assemblies[0].Problems);
		}

		[Test]
		public void AddPluginFamily()
		{
			TypePath typePath = new TypePath(this.GetType());
			string theDefaultKey = "red";
			string[] theTargets = new string[]{"Client"};

			InstanceScope theScope = InstanceScope.PerRequest;
			_builder.AddPluginFamily(typePath, theDefaultKey, theTargets, theScope);

			FamilyToken expected = new FamilyToken(typePath, theDefaultKey, theTargets);
			Assert.AreEqual(new FamilyToken[]{expected}, _report.Families);

			FamilyToken actual = _report.Families[0];
			Assert.AreEqual(theTargets, actual.DeploymentTargets);
			Assert.AreEqual(theScope, actual.Scope);
		}


		[Test]
		public void AddPluginFamilyWithScope()
		{
			TypePath typePath = new TypePath(this.GetType());
			string theDefaultKey = "red";
			string[] theTargets = new string[]{"Client"};

			InstanceScope theScope = InstanceScope.Singleton;
			_builder.AddPluginFamily(typePath, theDefaultKey, theTargets, theScope);

			FamilyToken expected = new FamilyToken(typePath, theDefaultKey, theTargets);
			Assert.AreEqual(new FamilyToken[]{expected}, _report.Families);

			FamilyToken actual = _report.Families[0];
			Assert.AreEqual(theScope, actual.Scope);
		}


		[Test]
		public void AttachSourceAllSuccessful()
		{
			string theAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			string[] theTargets = new string[]{"Client", "Server"};

			_builder.AddAssembly(theAssemblyName, theTargets);
			_builder.StartFamilies();

			TypePath typePath = new TypePath(this.GetType());
			string theDefaultKey = "red";

			_builder.AddPluginFamily(typePath, theDefaultKey, theTargets, InstanceScope.PerRequest);

			InstanceMemento sourceMemento = MockMementoSource.CreateSuccessMemento();

			string pluginTypeName = typePath.ClassName;
			_builder.AttachSource(pluginTypeName, sourceMemento);

			FamilyToken family = _report.FindFamily(pluginTypeName);
			Assert.IsNotNull(family.SourceInstance);
			Assert.AreEqual(0, family.SourceInstance.Problems.Length);
		}

		[Test]
		public void AttachSourceCannotCreateMemento()
		{
			string theAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			string[] theTargets = new string[]{"Client", "Server"};

			_builder.AddAssembly(theAssemblyName, theTargets);
			_builder.StartFamilies();

			TypePath typePath = new TypePath(this.GetType());
			string theDefaultKey = "red";

			_builder.AddPluginFamily(typePath, theDefaultKey, theTargets, InstanceScope.PerRequest);

			InstanceMemento sourceMemento = MockMementoSource.CreateFailureMemento();

			string pluginTypeName = typePath.ClassName;
			_builder.AttachSource(pluginTypeName, sourceMemento);

			FamilyToken family = _report.FindFamily(pluginTypeName);

			Problem expected = new Problem(ConfigurationConstants.COULD_NOT_CREATE_MEMENTO_SOURCE, string.Empty);
			Assert.AreEqual(new Problem[]{expected}, family.Problems);
		}

		[Test]
		public void AddInterceptorHappyPath()
		{
			string theAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			string[] theTargets = new string[]{"Client", "Server"};

			_builder.AddAssembly(theAssemblyName, theTargets);
			_builder.StartFamilies();

			TypePath typePath = new TypePath(this.GetType());
			string theDefaultKey = "red";

			_builder.AddPluginFamily(typePath, theDefaultKey, theTargets, InstanceScope.PerRequest);

			InstanceMemento memento = MockInterceptor.CreateSuccessMemento();
			_builder.AddInterceptor(typePath.ClassName, memento);

			FamilyToken family = _report.FindFamily(typePath.ClassName);
			Assert.AreEqual(1, family.Interceptors.Length);

			Assert.AreEqual(0, family.Interceptors[0].Problems.Length);
		}

		[Test]
		public void AddInterceptorThatCannotBeBuilt()
		{
			string theAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			string[] theTargets = new string[]{"Client", "Server"};

			_builder.AddAssembly(theAssemblyName, theTargets);
			_builder.StartFamilies();

			TypePath typePath = new TypePath(this.GetType());
			string theDefaultKey = "red";

			_builder.AddPluginFamily(typePath, theDefaultKey, theTargets, InstanceScope.PerRequest);

			InstanceMemento memento = MockInterceptor.CreateFailureMemento();
			_builder.AddInterceptor(typePath.ClassName, memento);

			FamilyToken family = _report.FindFamily(typePath.ClassName);
			Assert.AreEqual(1, family.Interceptors.Length);	
		
			InstanceToken interceptor = family.Interceptors[0];
			
			Problem expected = new Problem(ConfigurationConstants.COULD_NOT_CREATE_INSTANCE, string.Empty);

			Assert.AreEqual(new Problem[]{expected}, interceptor.Problems);
		}



		[Test]
		public void AddPluginFamilyWithFakeType()
		{
			TypePath typePath = new TypePath("Nothing", "Wrong");
			_builder.AddPluginFamily(typePath, "", new string[0], InstanceScope.PerRequest);

			FamilyToken family = _report.Families[0];

			Problem expected = new Problem(ConfigurationConstants.COULD_NOT_LOAD_TYPE, string.Empty);

			Assert.AreEqual(new Problem[]{expected}, family.Problems);
		}

		[Test]
		public void AddPluginHappyPath()
		{
			TypePath familyPath = new TypePath(typeof(IGateway));
			TypePath pluginPath = new TypePath(typeof(DefaultGateway));

			_builder.AddPluginFamily(familyPath, string.Empty, new string[0], InstanceScope.PerRequest);

			string theConcreteKey = "green";
			_builder.AddPlugin(familyPath.ClassName, pluginPath, theConcreteKey);

			FamilyToken family = _report.Families[0];
			PluginToken expected = new PluginToken(pluginPath, theConcreteKey, DefinitionSource.Explicit);


			Assert.AreEqual(new PluginToken[]{expected}, family.Plugins);
		}

		[Test]
		public void AddPluginWithoutConcreteKey()
		{
			TypePath familyPath = new TypePath(typeof(IGateway));
			TypePath pluginPath = new TypePath(typeof(DefaultGateway));

			_builder.AddPluginFamily(familyPath, string.Empty, new string[0], InstanceScope.PerRequest);

			string[] theSetters = new string[]{"Name", "Color"};
			_builder.AddPlugin(familyPath.ClassName, pluginPath, string.Empty);

			PluginToken plugin = _report.FindFamily(familyPath.ClassName).Plugins[0];

			Problem expected = new Problem(ConfigurationConstants.PLUGIN_IS_MISSING_CONCRETE_KEY, string.Empty);

			Assert.AreEqual(new Problem[]{expected}, plugin.Problems);
		}


		[Test]
		public void AddPluginWithBadType()
		{
			TypePath familyPath = new TypePath(typeof(IGateway));
			TypePath pluginPath = new TypePath("Something", "Nothing");

			_builder.AddPluginFamily(familyPath, string.Empty, new string[0], InstanceScope.PerRequest);

			string[] theSetters = new string[]{"Name", "Color"};
			_builder.AddPlugin(familyPath.ClassName, pluginPath, "red");

			PluginToken plugin = _report.FindFamily(familyPath.ClassName).Plugins[0];

			Problem expected = new Problem(ConfigurationConstants.COULD_NOT_LOAD_TYPE, string.Empty);

			Assert.AreEqual(new Problem[]{expected}, plugin.Problems);
		}

		[Test]
		public void AddSetter()
		{
			TypePath familyPath = new TypePath(typeof(IGateway));
			TypePath pluginPath = new TypePath(typeof(DefaultGateway));

			_builder.AddPluginFamily(familyPath, string.Empty, new string[0], InstanceScope.PerRequest);
			_builder.AddPlugin(familyPath.ClassName, pluginPath, "red");	

			_builder.AddSetter(familyPath.ClassName, "red", "Name");

			PluginToken pluginToken = _report.FindFamily(familyPath.ClassName).Plugins[0];
			PropertyDefinition actual = pluginToken["Name"];

			PropertyDefinition expected = new PropertyDefinition("Name", typeof(string).FullName, PropertyDefinitionType.Setter, ArgumentType.Primitive);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void AddInvalidSetter()
		{
			TypePath familyPath = new TypePath(typeof(IGateway));
			TypePath pluginPath = new TypePath(typeof(DefaultGateway));

			_builder.AddPluginFamily(familyPath, string.Empty, new string[0], InstanceScope.PerRequest);
			_builder.AddPlugin(familyPath.ClassName, pluginPath, "red");

			string theFakePropertyName = "FakeProperty";
			_builder.AddSetter(familyPath.ClassName, "red", theFakePropertyName);

			PluginToken pluginToken = _report.FindFamily(familyPath.ClassName).Plugins[0];
			PropertyDefinition property = pluginToken[theFakePropertyName];
	
			Problem expected = new Problem(ConfigurationConstants.INVALID_SETTER, string.Empty);

			Assert.AreEqual(new Problem[]{expected}, property.Problems);
		}

		[Test]
		public void RegisterMementoHappy()
		{
			TypePath familyPath = new TypePath(typeof(IGateway));

			_builder.AddPluginFamily(familyPath, string.Empty, new string[0], InstanceScope.PerRequest);

			MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
			_builder.RegisterMemento(familyPath.ClassName, memento);

			Assert.AreEqual(0, _builder.Report.Problems.Length);
		}

		[Test]
		public void RegisterMementoWithNoPluginFamily()
		{
			MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
			_builder.RegisterMemento("Some family", memento);

			Problem expected = new Problem(ConfigurationConstants.PLUGIN_FAMILY_CANNOT_BE_FOUND_FOR_INSTANCE, string.Empty);
			
			Assert.AreEqual(new Problem[]{expected}, _builder.Report.Problems);
		}

		[Test]
		public void GiveItATry()
		{
			PluginGraphBuilder builder = new PluginGraphBuilder();
			builder.BuildDiagnosticPluginGraph();
			PluginGraphReport report = builder.Report;
			Assert.IsNotNull(report);
		}

	}
}
