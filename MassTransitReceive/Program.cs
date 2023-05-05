using MassTransit;
using MassTransitReceive;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.SetBasePath(Path.GetDirectoryName(typeof(ValueConsumer).Assembly.Location))
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
            Console.WriteLine("Using raw MassTransit");
            cfg.AddConsumer<ValueConsumer>();
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
                cfg.ConfigureEndpoints(cxt);
            });
        });
        services.AddSingleton<ICounter, Counter>();
    })
    .Build();

var task = host.RunAsync();

var counter = host.Services.GetRequiredService<ICounter>();

char key;

Func<string> clear = () => { counter.Clear(); return "message count cleared"; };

while ((key = (char)Console.ReadKey().KeyChar) != 'q')
{
    var response = key switch
    {
        'q' => "",
        's' => $"{counter.Count} messages processed",
        'c' => clear(),
        _ => "invalid key press. (q)uit, (s)how, (c)lear"
    };
    Console.WriteLine($"\n{response}");
}

host.Dispose();
