using Sample.Contracts;
using MassTransit;
using Sample.Components.Consumers;

namespace Sample.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        //builder.Services.AddAuthorization();

        builder.Services.AddControllers();
        builder.Services.AddMassTransit(cfg =>
        {
            cfg.UsingRabbitMq();
            cfg.AddRequestClient<ISubmitOrder>(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
            cfg.AddRequestClient<ICheckOrder>();
        });

        var app = builder.Build();

        //app.UseHttpsRedirection();
        //app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}