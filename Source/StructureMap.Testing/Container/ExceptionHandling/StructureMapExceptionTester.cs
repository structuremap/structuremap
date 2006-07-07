using System;
using System.IO;
using NUnit.Framework;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container.ExceptionHandling
{
	[TestFixture]
	public class StructureMapExceptionTester
	{
		private ExceptionTestRunner _testRunner;

		#region utility functions

		private void backupConfig()
		{
			try
			{
				File.Move("StructureMap.config", "StructureMap.Bak");
			}
			catch (Exception ex)
			{
				Console.Write(ex);
			}


		}


		private void restoreConfig()
		{
			try
			{
				File.Delete("StructureMap.config");
				File.Move("StructureMap.Bak", "StructureMap.config");
			}
			catch (Exception ex)
			{
				Console.Write(ex);
			}

		}

		[SetUp]
		public void SetUp()
		{
			//backupConfig();
			_testRunner = new ExceptionTestRunner();
		}

		[TearDown]
		public void TearDown()
		{
			//restoreConfig();
		}

		#endregion

		[Test]
		public void ExceptionMessage()
		{
			StructureMapException exception = new StructureMapException(100, "StructureMap.config");
			string expected = "StructureMap Exception Code:  100\nExpected file \"StructureMap.config\" cannot be opened at StructureMap.config";

			string actual = exception.Message;

			Assert.AreEqual(expected, actual);
		}


		[Test, Ignore("Issue with MSBUILD causes this to fail in cruise build.")]
		public void CannotLoadTheConfigFile()
		{
			try
			{
				try
				{
					backupConfig();
					File.Delete("StructureMap.config");
					PluginGraphBuilder builder = new PluginGraphBuilder();
				}
				catch (StructureMapException ex)
				{
					string configPath = PluginGraphBuilder.GetStructureMapConfigurationPath();
					string expected = "StructureMap Exception Code:  100\nExpected file \"StructureMap.config\" cannot be opened at ";
					expected += configPath;

					Assert.AreEqual(100, ex.ErrorCode);
					string actual = ex.Message;
					Assert.AreEqual(expected, actual);
					return;

				}
				catch (Exception ex)
				{
					Assert.Fail("Wrong exception:  " + ex.Message);
				}

				Assert.Fail("No exception");
			}
			finally
			{
				this.restoreConfig();
			}
		}

		[Test, ExpectedException(typeof (StructureMapException)), Ignore("Issue with MSBUILD causes this to fail in cruise build.")]
		public void Exception100FromObjectFactory()
		{
			try
			{
				backupConfig();
				File.Delete("StructureMap.config");
				ObjectFactory.ResetDefaults();
			}
			finally
			{
				this.restoreConfig();
			}
		}

		[Test]
		public void CannotLoadAssemblyInAssemblyNode()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(101);
		}

		[Test]
		public void MissingAssemblyInPluginFamilyNode()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(102);
		}

		[Test]
		public void CannotLoadTypeFromPluginFamilyNode()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(103);
		}

		[Test]
		public void CannotLoadAssemblyInPluginNode()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(110);
		}

		[Test]
		public void CannotLoadTypeFromPluginNode()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(111);
		}

		[Test]
		public void MissingConcreteKeyOnPluginNode()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(112);
		}

		[Test]
		public void DuplicateConcreteKeyOnPluginFamily()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(113);
		}

		[Test]
		public void CouldNotBuildTheDesignatedMementoSourceForAPluginFamily()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(120);
		}


		[Test]
		public void CouldNotUpcastDesignatedPluggedTypeIntoPluginType()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(114);
		}

		[Test]
		public void CouldNotFindInstanceKey()
		{
			_testRunner.ExecuteGetInstance(200, "NonExistentInstanceKey", typeof (IWidget));
		}


		[Test]
		public void CouldNotFindConcreteKey()
		{
			_testRunner.ExecuteGetInstance(201, "BadConcreteKey", typeof (IWidget));
		}

		[Test]
		public void DefaultKeyDoesNotExist()
		{
			_testRunner.ExecuteGetDefaultInstance(202, typeof (IWidget));
		}

		[Test]
		public void InvalidConfigurationOfInterceptors()
		{
			_testRunner.ExecuteExceptionTestFromResetDefaults(121);
		}
	}


}