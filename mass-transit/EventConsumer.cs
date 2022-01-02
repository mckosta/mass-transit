using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace mass_transit
{
    class EventConsumer : IConsumer<Order>
    {
        ILogger<EventConsumer> _logger;

        public EventConsumer(ILogger<EventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Order> context)
        {
            _logger.LogInformation("Value: {Value}", context.Message.Id);
        }
    }

    class Order
    {
        public int Id { get; set; }
    }
}
