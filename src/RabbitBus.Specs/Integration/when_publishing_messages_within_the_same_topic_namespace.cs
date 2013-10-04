using System;
using Machine.Specifications;
using RabbitBus.Specs.Infrastructure;
using RabbitBus.Specs.TestTypes;
using RabbitMQ.Client;


namespace RabbitBus.Specs.Integration
{
    [Integration]
    [Subject("Topic Exchange")]
    public class when_publishing_messages_within_the_same_topic_namespace
    {
        const string SpecId = "nh.topicEx";
        const string CREATE_SWAP = "granite.creation.swap";
        const string CREATE_CAP = "granite.creation.cap";
        static RabbitExchange _exchange;
        static Bus _bus;
        static TestMessage _actualMessage;
        static readonly TestMessage _default = new TestMessage("error");

        Establish context = () =>
        {
            _exchange = new RabbitExchange(SpecId, ExchangeType.Topic);
            _bus = new BusBuilder().Configure(ctx => ctx.Consume<TestMessage>().WithExchange(SpecId, cfg => cfg.Topic())
                .WithQueue(SpecId)).Build();
            _bus.Connect();

            _bus.Subscribe<TestMessage>(OnCreateSwap, CREATE_SWAP);
            _bus.Subscribe<TestMessage>(OnCreateCap, CREATE_CAP);
        };

        private static void OnCreateSwap(IMessageContext<TestMessage> messageContext)
        {
            Console.WriteLine("OnCreateSwap");
            _actualMessage = messageContext.Message;
        }


        private static void OnCreateCap(IMessageContext<TestMessage> messageContext)
        {
            Console.WriteLine("OnCreateCap");
            _actualMessage = messageContext.Message;
        }

        Cleanup after = () => _bus.Close();

        Because of = () => new Action(() =>
                                          {
                                              _exchange.Publish(new TestMessage("test"), CREATE_CAP);
                                              _exchange.Publish(new TestMessage("test"), CREATE_SWAP);
                                          }
                                          ).BlockUntil(() => _actualMessage != null)();

        It should_receive_the_message = () => _actualMessage.ProvideDefault(() => _default).Text.ShouldEqual("test");
    }



    [Serializable]
    public class Message<T>
    {
        public Message(T body)
        {
            Body = body;
        }

        public T Body { get; set; }
    }

    [Serializable]
    public class CreateCapMessage : Message<CreateCmd>
    {
        public CreateCapMessage()
            : base(new CreateCmd("CAP"))
        {
        }
    }


    [Serializable]
    public class CreateCmd
    {
        public CreateCmd(string msg)
        {
            Msg = msg;
        }

        public string Msg { get; set; }
    }


    [Serializable]
    public class CreateFloorMessage : Message<CreateCmd>
    {
        public CreateFloorMessage()
            : base(new CreateCmd("FLOOR"))
        {
            WhenCreated = DateTime.UtcNow;
        }

        public DateTime WhenCreated { get; set; }
    }

    [Serializable]
    public class CreateSwapMessage : Message<CreateCmd>
    {
        public CreateSwapMessage()
            : base(new CreateCmd("SWP"))
        {
        }
    }
}