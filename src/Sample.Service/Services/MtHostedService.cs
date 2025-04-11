using Microsoft.Extensions.Hosting;

namespace Sample.Service.Services;

public class MtHostedService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
           await Task.Delay(1000, stoppingToken);
        }
    }
}