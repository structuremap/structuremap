using NMock;
using NMock.Constraints;
using NUnit.Framework;
using StructureMap.Client.Controllers;
using StructureMap.Client.TreeNodes;
using StructureMap.Client.Views;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;

namespace StructureMap.Testing.Client.Controllers
{
	[TestFixture]
	public class ApplicationControllerTester
	{
		private DynamicMock shellMock;
		private DynamicMock reportSourceMock;
		private DynamicMock factoryMock;
		private ApplicationController controller;


		[SetUp]
		public void SetUp()
		{
			shellMock = new DynamicMock(typeof (IApplicationShell));
			reportSourceMock = new DynamicMock(typeof (IReportSource));
			factoryMock = new DynamicMock(typeof (IHTMLSourceFactory));

			controller = new ApplicationController((IApplicationShell) shellMock.MockInstance,
			                                       (IReportSource) reportSourceMock.MockInstance,
			                                       (IHTMLSourceFactory) factoryMock.MockInstance);
		}

		[Test]
		public void RefreshReportDifferentFolders()
		{
			string configPath = @"C:\SomeFolder\StructureMap.config";
			string assemblyFolder = @"C:\SomeOtherFolder";

			shellMock.ExpectAndReturn("ConfigurationPath", configPath);
			shellMock.ExpectAndReturn("AssemblyFolder", assemblyFolder);
			shellMock.SetupResult("LockFolders", false);

			PluginGraphReport report = new PluginGraphReport();
			reportSourceMock.ExpectAndReturn("FetchReport", report, configPath, assemblyFolder);

			shellMock.Expect("TopNode", new IsTypeOf(typeof (GraphObjectNode)));

			controller.RefreshReport();

			shellMock.Verify();
			reportSourceMock.Verify();
		}


		[Test]
		public void RefreshReportLockedFolders()
		{
			string configPath = @"C:\SomeFolder\StructureMap.config";
			string assemblyFolder = @"C:\SomeOtherFolder";

			shellMock.ExpectAndReturn("ConfigurationPath", configPath);
			shellMock.SetupResult("AssemblyFolder", assemblyFolder);
			shellMock.SetupResult("LockFolders", true);

			PluginGraphReport report = new PluginGraphReport();
			reportSourceMock.ExpectAndReturn("FetchReport", report, configPath, @"C:\SomeFolder");

			shellMock.Expect("TopNode", new IsTypeOf(typeof (GraphObjectNode)));

			controller.RefreshReport();

			shellMock.Verify();
			reportSourceMock.Verify();
		}

		[Test]
		public void ShowView()
		{
			FamilyToken token = new FamilyToken();
			string viewName = ViewConstants.PLUGINFAMILY;

			string html = "asldkfjasl;kjflsakjf";
			DynamicMock sourceMock = new DynamicMock(typeof(IHTMLSource));
			sourceMock.ExpectAndReturn("BuildHTML", html, token);

			shellMock.Expect("DisplayHTML", html);

			factoryMock.ExpectAndReturn("GetSource", sourceMock.MockInstance, viewName);

			controller.ShowView(viewName, token);

			sourceMock.Verify();
			shellMock.Verify();
			factoryMock.Verify();
		}

	}
}