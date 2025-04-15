using MassTransit;

namespace Sample.Components.Consumers;

public class SubmitOrderConsumerDefinition : ConsumerDefinition<SubmitOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(1)));
        
        // This is silly but provides the example
        endpointConfigurator.UseExecute(ctx => Console.WriteLine("HELLO"));
    }
}