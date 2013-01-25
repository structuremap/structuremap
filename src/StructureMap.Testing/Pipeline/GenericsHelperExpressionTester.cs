using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace StructureMap.Testing.Pipeline
{
    public class Address
    {
    }

    public class AddressDTO
    {
    }

    public class Continuation
    {
    }

    public class AddressFlattener : IFlattener<Address>
    {
        public object ToDto(object input)
        {
            object dto = createDTO((Address) input);
            return dto;
        }

        private object createDTO(Address input)
        {
            // creates the AddressDTO object from the 
            // Address object passed in
            throw new NotImplementedException();
        }
    }

    public interface IFlattener
    {
        object ToDto(object input);
    }

    public interface IFlattener<T> : IFlattener
    {
    }

    public class PassthroughFlattener<T> : IFlattener<T>
    {
        public object ToDto(object input)
        {
            return input;
        }
    }

    [TestFixture]
    public class when_accessing_a_type_registered_as_an_open_generics_type
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                // Define the basic open type for IFlattener<>
                x.For(typeof (IFlattener<>)).TheDefaultIsConcreteType(typeof (PassthroughFlattener<>));

                // Explicitly Register a specific closed type for Address
                x.For<IFlattener<Address>>().Use<AddressFlattener>();
            });
        }

        #endregion

        private Container container;

        [Test]
        public void asking_for_a_closed_type_that_is_explicitly_registered_returns_the_explicitly_defined_type()
        {
            container.GetInstance<IFlattener<Address>>()
                .ShouldBeOfType<AddressFlattener>();
        }

        [Test]
        public void asking_for_a_closed_type_that_is_not_explicitly_registered_will_close_the_open_type_template()
        {
            container.GetInstance<IFlattener<Continuation>>()
                .ShouldBeOfType<PassthroughFlattener<Continuation>>();
        }

        [Test]
        public void throws_exception_if_passed_a_type_that_is_not_an_open_generic_type()
        {
            try
            {
                container.ForGenericType(typeof (string)).WithParameters().GetInstanceAs<IFlattener>();
                Assert.Fail("Should have thrown exception");
            }
            catch (StructureMapException ex)
            {
                ex.ErrorCode.ShouldEqual(285);
            }
        }

        [Test]
        public void using_the_generics_helper_expression()
        {
            var flattener1 = container.ForGenericType(typeof (IFlattener<>))
                .WithParameters(typeof (Address)).GetInstanceAs<IFlattener>();
            flattener1.ShouldBeOfType<AddressFlattener>();

            var flattener2 = container.ForGenericType(typeof (IFlattener<>))
                .WithParameters(typeof (Continuation)).GetInstanceAs<IFlattener>();
            flattener2.ShouldBeOfType<PassthroughFlattener<Continuation>>();
        }
    }

    public class ObjectFlattener
    {
        private readonly IContainer _container;

        // You can inject the IContainer itself into an object by the way...
        public ObjectFlattener(IContainer container)
        {
            _container = container;
        }

        // This method can "flatten" any object
        public object Flatten(object input)
        {
            var flattener = _container.ForGenericType(typeof (IFlattener<>))
                .WithParameters(input.GetType())
                .GetInstanceAs<IFlattener>();

            return flattener.ToDto(input);
        }
    }

    public class FindAddressController
    {
        public Address FindAddress(long id)
        {
            return null;
        }

        public Continuation WhatShouldTheUserDoNext()
        {
            return null;
        }
    }


    [TestFixture]
    public class when_getting_a_closed_type_from_an_open_generic_type_by_providing_an_input_parameter
    {
        [Test]
        public void fetch_the_object()
        {
            var container =
                new Container(
                    x => { x.For<IHandler<Shipment>>().Use<ShipmentHandler>(); });

            var shipment = new Shipment();

            var handler = container
                .ForObject(shipment)
                .GetClosedTypeOf(typeof (IHandler<>))
                .As<IHandler>();

            handler.ShouldBeOfType<ShipmentHandler>().Shipment.ShouldBeTheSameAs(shipment);
        }
    }

    [TestFixture]
    public class when_getting_a_closed_type_from_an_open_generic_type_by_providing_an_input_parameter_from_ObjectFactory
    {
        [Test]
        public void fetch_the_object()
        {
            ObjectFactory.Initialize(
                x => { x.For<IHandler<Shipment>>().Use<ShipmentHandler>(); });

            var shipment = new Shipment();
            var handler = ObjectFactory.ForObject(shipment).GetClosedTypeOf(typeof (IHandler<>)).As<IHandler>();

            handler.ShouldBeOfType<ShipmentHandler>().Shipment.ShouldBeTheSameAs(shipment);
        }
    }

    [TestFixture]
    public class when_getting_all_closed_type_from_an_open_generic_type_by_providing_an_input_parameter
    {
        [Test]
        public void fetch_the_objects()
        {
            var container = new Container(x =>
            {
                x.For<IHandler<Shipment>>().Use<ShipmentHandler>();
                x.For<IHandler<Shipment>>().AddConcreteType<ShipmentHandler2>();
            });

            var shipment = new Shipment();

            IList<IHandler> handlers = container
                .ForObject(shipment)
                .GetAllClosedTypesOf(typeof (IHandler<>))
                .As<IHandler>();

            handlers[0].ShouldBeOfType<ShipmentHandler>();
            handlers[1].ShouldBeOfType<ShipmentHandler2>();
        }
    }

    public class Shipment
    {
    }

    public interface IHandler<T> : IHandler
    {
    }

    public interface IHandler
    {
    }

    public class ShipmentHandler : IHandler<Shipment>
    {
        private readonly Shipment _shipment;

        public ShipmentHandler(Shipment shipment)
        {
            _shipment = shipment;
        }

        public Shipment Shipment { get { return _shipment; } }
    }

    public class ShipmentHandler2 : IHandler<Shipment>
    {
        private readonly Shipment _shipment;

        public ShipmentHandler2(Shipment shipment)
        {
            _shipment = shipment;
        }

        public Shipment Shipment { get { return _shipment; } }
    }
}