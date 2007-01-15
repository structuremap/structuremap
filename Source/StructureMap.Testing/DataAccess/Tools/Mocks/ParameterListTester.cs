using NUnit.Framework;
using StructureMap.DataAccess.Tools.Mocks;

namespace StructureMap.Testing.DataAccess.Tools.Mocks
{
	[TestFixture]
	public class ParameterListTester
	{
		[Test]
		public void SetAndRetrieveParameterValues()
		{
			ParameterList list = new ParameterList();
			string theParameterValue = "something";
			list["Param1"] = theParameterValue;

			Assert.AreEqual(theParameterValue, list["Param1"]);
		}


		[Test]
		public void GetAllParameterKeys()
		{
			ParameterList list = new ParameterList();
			list["Param1"] = string.Empty;
			list["Param2"] = string.Empty;
			list["Param3"] = string.Empty;
			list["Param4"] = string.Empty;

			string[] expected = new string[]{"Param1", "Param2", "Param3", "Param4"};
			string[] actual = list.AllKeys;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void SuccessfulVerify()
		{
			ParameterList list = new ParameterList();
			list["Param1"] = string.Empty;
			list["Param2"] = string.Empty;
			list["Param3"] = string.Empty;
			list["Param4"] = string.Empty;

			ParameterList list2 = (ParameterList)list.Clone();

			list.Verify(list2);
		}

		[Test, ExpectedException(typeof(ParameterValidationFailureException))]
		public void VerifyFailsWithWrongValue()
		{
			ParameterList list = new ParameterList();
			list["Param1"] = string.Empty;
			list["Param2"] = string.Empty;
			list["Param3"] = string.Empty;
			list["Param4"] = string.Empty;

			ParameterList list2 = (ParameterList)list.Clone();
			list2["Param1"] = "different";

			list.Verify(list2);
		}

		[Test, ExpectedException(typeof(ParameterValidationFailureException))]
		public void VerifyFailsWithUnexpectedParameter()
		{
			ParameterList list = new ParameterList();
			list["Param1"] = string.Empty;
			list["Param2"] = string.Empty;
			list["Param3"] = string.Empty;
			list["Param4"] = string.Empty;

			ParameterList list2 = (ParameterList)list.Clone();
			list2["Param5"] = "different";

			list.Verify(list2);
		}

		[Test, ExpectedException(typeof(ParameterValidationFailureException))]
		public void VerifyFailsWithMissingParameter()
		{
			ParameterList list = new ParameterList();
			list["Param1"] = string.Empty;
			list["Param2"] = string.Empty;
			list["Param3"] = string.Empty;
			list["Param4"] = string.Empty;

			ParameterList list2 = (ParameterList)list.Clone();

			// Add a parameter to "list" that will be missing in list2
			list["Param5"] = "different";

			list.Verify(list2);			
		}
	}
}
