using System;

using NUnit.Framework;

namespace StructureMap.Testing.Examples
{
	[TestFixture]
	public class IndividualContainers
	{
		[Test]
		public void Avoid_NullReferenceException_When_Getting_Undefined_Named_Instances()
		{
			using (Container myContainer = new Container())
			{
				myContainer.GetInstance<IndividualContainers>("TEST");
			}
		}
	}
}
