using MassTransit;
using MassTransitCommon;
using MassTransitSend;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

const string DELAY_START = "wait";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.SetBasePath(Path.GetDirectoryName(typeof(Value).Assembly.Location))
               .AddJsonFile("appsettings.Common.json", false)
               .AddJsonFile("appsettings.json", false);
    })
    .ConfigureLogging((context, builder) =>
    {
        builder.ClearProviders()
               .AddNLog()
               .SetMinimumLevel(LogLevel.Trace);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddMassTransit(cfg =>
        {
            cfg.UsingActiveMq((cxt, cfg) =>
            {
                var config = context.Configuration.GetSection("queueOptions");
                cfg.Host(config.GetValue<string>("activeMqHost"), config.GetValue("activeMqPort", 61616), h =>
                {
                    h.Username(config.GetValue<string>("username"));
                    h.Password(config.GetValue<string>("password"));
                    var failover = config.GetSection("failoverHosts")?.Get<List<string>>();
                    if (failover != null)
                        h.FailoverHosts(failover.ToArray());
                });
            });
        });
        services.AddTransient<ISender, Sender>();
    })
    .Build();

var task = host.RunAsync();
var config = host.Services.GetRequiredService<IConfiguration>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

var senderCount = config.GetValue("messageCount", 10);

var run = host.Services.GetRequiredService<ISender>();

var delayStart = config.GetValue(DELAY_START, false);

if (delayStart)
{
    logger.LogInformation("Press any key to start");
    Console.ReadKey();
}

await run.Execute(senderCount);

host.Dispose();