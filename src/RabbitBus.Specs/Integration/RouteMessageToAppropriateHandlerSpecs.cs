using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using RabbitBus.Specs.Infrastructure;
using RabbitBus.Specs.TestTypes;
using RabbitMQ.Client;


namespace RabbitBus.Specs.Integration
{
    [Integration]
    [Subject("Routing Messages")]
    public class RouteMessageToAppropriateHandlerSpecs
    {
        private const string SpecId = "5A450129-703C-47A3-B123-A511C195CBF1";
        static RabbitExchange _exchange;
        static Bus _bus;
        static TestMessage _actualMessage;
        static readonly TestMessage _default = new TestMessage("error");
        static IDictionary _headers;

        //Establish context = () =>
        //                        {
        //                           _bus = new BusBuilder()
        //                               .Configure( ctx => ctx.Publish<TestMessage>()
        //                                            .WithDefaultHeaders()
        //                        }
    }
}
