using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MassTransitCommon;

namespace MassTransitSend
{
    public class Sender : ISender
    {
        readonly IBusControl endpoint;
        readonly ILogger<Sender> logger;
        readonly int delay;
        readonly int senderCount;

        public Sender(IBusControl endpoint, ILogger<Sender> logger, IConfiguration config)
        {
            this.endpoint = endpoint;
            this.logger = logger;
            delay = config.GetValue("delay", 50);
            senderCount = config.GetValue("senderCount", 4);
        }

        async Task Send(int count)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var messageBody = $"Message - {System.Guid.NewGuid()}";
                    logger.LogInformation($"sending: {messageBody}");
                    await endpoint.Publish<Value>(new { message = messageBody });
                    await Task.Delay(delay);
                }
            }
            catch (System.Exception e)
            {
                logger.LogError(e, "Error sending");
                throw;
            }
        }

        public Task Execute(int count)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < senderCount; i++)
                tasks.Add(Task.Run(() => Send(count)));
            return Task.WhenAll(tasks);
        }
    }
}