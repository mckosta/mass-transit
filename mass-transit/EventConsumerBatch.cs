using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace mass_transit
{
    class EventConsumerBatch : IConsumer<Batch<OrderBatch>>
    {
        ILogger<EventConsumer> _logger;

        public EventConsumerBatch()
        {

        }

        public EventConsumerBatch(ILogger<EventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Batch<OrderBatch>> context)
        {
            for (int i = 0; i < context.Message.Length; i++)
            {
                ConsumeContext<OrderBatch> Id = context.Message[i];
                _logger.LogInformation("Value: {Value}", Id);
            }
        }
    }

    public interface OrderBatch
    {
        public int Id { get; set; }
    }


    class EventConsumerBatchDefinition : ConsumerDefinition<EventConsumerBatch>
    {
        public EventConsumerBatchDefinition()
        {
            Endpoint(x => x.PrefetchCount = 1000);
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<EventConsumerBatch> consumerConfigurator)
        {
            consumerConfigurator.Options<BatchOptions>(options => options
                .SetMessageLimit(10)
                .SetTimeLimit(1000)
                .SetConcurrencyLimit(10));
        }
    }
}
