using System;
using StructureMap;
using StructureMap.Source;
using NUnit.Framework;

namespace StructureMap.Testing.Container.Source
{
	[TestFixture]
	public class MachineXmlMementoSourceTester
	{
		private MachineXmlMementoSource source;

		[SetUp]
		public void SetUp()
		{
			source = new MachineXmlMementoSource("Machine.XML", string.Empty, "Rule");
		}

		[Test]
		public void Fetch1()
		{
			source.SetMachineName("server");
			InstanceMemento memento = source.GetMemento("FavoriteColor");

			Assertion.AssertNotNull(memento);
			Assertion.AssertEquals("Server's favorite color is blue", "Blue", memento.GetProperty("Color"));
		}

		[Test]
		public void Fetch2()
		{
			source.SetMachineName("localhost");
			InstanceMemento memento = source.GetMemento("FavoriteColor");

			Assertion.AssertNotNull(memento);
			Assertion.AssertEquals("Localhost's favorite color is red", "Red", memento.GetProperty("Color"));
		}	
	


		[Test]
		public void FetchMachineNotFound()
		{
			source.SetMachineName("fake-machine");
			InstanceMemento memento = source.GetMemento("FavoriteColor");

			Assertion.AssertNotNull(memento);
			Assertion.AssertEquals("Default's favorite color is red", "Orange", memento.GetProperty("Color"));
		}		

		/*
		[Test, Ignore("Only works on Jeremy's MILLERJ box")]
		public void JeremysBox()
		{
			InstanceMemento memento = source.GetMemento("FavoriteColor");

			Assertion.AssertNotNull(memento);
			Assertion.AssertEquals("Jeremy's favorite color is green", "Green", memento.GetProperty("Color"));
		}
		*/	

	}
}
