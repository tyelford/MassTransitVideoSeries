using MassTransit;
using Sample.Contracts;

namespace Sample.Components.StateMachines;

public class OrderStateMachineDefinition : SagaDefinition<OrderState>
{
    public OrderStateMachineDefinition()
    {
        ConcurrentMessageLimit = 4;
    }
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator,
        IRegistrationContext context)
    {
        // Not needed in the scenario - but shows an example
        var partitioner = endpointConfigurator.CreatePartitioner(8);
        sagaConfigurator.Message<IOrderSubmitted>(x => x.UsePartitioner(partitioner, y => y.Message.CustomerNumber));
        
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 5000, 10000));
        endpointConfigurator.UseInMemoryOutbox(context);
    }
}