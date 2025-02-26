using MassTransit;
using Newtonsoft.Json;
using SharedMessages.Messages;

namespace ShippingService.Consumers
{
    public class OrderPlacedConsumer : IConsumer<OrderPlaced>
    {
        public Task Consume(ConsumeContext<OrderPlaced> context)
        {
            Console.WriteLine($"Order Received: {JsonConvert.SerializeObject(context.Message)}");

            return Task.CompletedTask;
        }
    }
}
