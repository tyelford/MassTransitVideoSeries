using Sample.Components.Consumers;
using Sample.Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Sample.Components.StateMachines;
using Sample.Service.Services;

namespace Sample.Service;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            //Log.Information("Starting Service");

            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            //Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
        }
        finally
        {
            //await Log.CloseAndFlushAsync();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        const int rabbitPrefetchCount = 16;

        return Host.CreateDefaultBuilder(args)
            //.UseSerilogConfiguration()
            .ConfigureAppConfiguration(configBuilder => { })
            .ConfigureServices((context, services) =>
            {
                services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
                services.AddHostedService<MtHostedService>();
                services.AddMassTransit(cfg =>
                {
                    cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
                    cfg.AddRequestClient<ISubmitOrder>();

                    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
                        .RedisRepository();
                    
                    cfg.UsingRabbitMq((ctx, hostConfig) =>
                    {
                        hostConfig.ConfigureEndpoints(ctx);
                    });
                });
                
            });
    }
}