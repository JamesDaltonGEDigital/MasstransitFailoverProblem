using MassTransit;
using MassTransitCommon;
using Microsoft.Extensions.Configuration;

namespace MassTransitReceive
{
    public class ValueConsumer : IConsumer<Value>
    {
        readonly ICounter counter;
        readonly int delay;

        public ValueConsumer(ICounter counter, IConfiguration config)
        {
            this.counter = counter;
            delay = config.GetValue("delay", 50);
        }

        public Task Consume(ConsumeContext<Value> context)
        {
            counter.Increment();
            Thread.Sleep(delay);
            return Task.CompletedTask;
        }
    }
}