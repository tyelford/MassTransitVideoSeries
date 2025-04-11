using Sample.Contracts;
using MassTransit;

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
                // cfg.UsingRabbitMq((context, config) =>
                // {
                //     config.ConfigureEndpoints(context);
                // });
                
                cfg.UsingRabbitMq();
                cfg.AddRequestClient<ISubmitOrder>();
        });

        var app = builder.Build();

        //app.UseHttpsRedirection();
        //app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}