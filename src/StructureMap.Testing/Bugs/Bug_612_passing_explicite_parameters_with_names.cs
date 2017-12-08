using Shouldly;
using System;
using System.Diagnostics;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_612_passing_explicit_object_parameters_with_names
    {
		[Fact]
		public void ObjectsAsNamedExplicitParameters()
		{
			var container = new Container();

			var param1 = new NotSimpleParameterClass() { ParameterValue = 11 };
			var param2 = new NotSimpleParameterClass() { ParameterValue = 22 };

			var complexObject = container.
									With("objectOne").EqualTo(param1).
									With("objectTwo").EqualTo(param2).
									GetInstance<ComplexClass>();

			complexObject.ShouldNotBeNull();
			complexObject.ObjectOne.ShouldBe(param1);
			complexObject.ObjectTwo.ShouldBe(param2);
		}
	}

	public class NotSimpleParameterClass
	{
		public int ParameterValue { get; set; }
	}

	public class ComplexClass
	{
		public NotSimpleParameterClass ObjectOne { get; set; }
		public NotSimpleParameterClass ObjectTwo { get; set; }


		public ComplexClass(NotSimpleParameterClass objectOne, NotSimpleParameterClass objectTwo)
		{
			ObjectOne = objectOne;
			ObjectTwo = objectTwo;
		}
	}
}