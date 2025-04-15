using MassTransit;
using Sample.Contracts;

namespace Sample.Components.StateMachines;

public class OrderStateMachine: MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        Event(() => OrderSubmitted, 
            x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => OrderStatusRequested,
            x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<IOrderNotFound>(new
                        {
                            context.Message.OrderId
                        });
                    }
                }));
            });
        
        
        InstanceState(x => x.CurrentState);
        
        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.SubmitDate = context.Message.Timestamp;
                    context.Saga.CustomerNumber = context.Message.CustomerNumber;
                    context.Saga.Updated = DateTime.UtcNow;
                })
                .TransitionTo(Submitted)
            );

        During(Submitted,
            Ignore(OrderSubmitted));
        
        DuringAny(
            When(OrderStatusRequested)
                .RespondAsync(x => x.Init<IOrderStatus>(new
                {
                    OrderId = x.Saga.CorrelationId,
                    Status = x.Saga.CurrentState
                }))
            );
        
        DuringAny(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.SubmitDate ??= context.Message.Timestamp;
                    context.Saga.CustomerNumber ??= context.Message.CustomerNumber;
                    //context.Saga.Updated = DateTime.UtcNow;
                })
            );
    }
    
    public State Submitted { get; private set; }
    
    
    public Event<IOrderSubmitted> OrderSubmitted { get; private set; }
    public Event<ICheckOrder> OrderStatusRequested { get; private set; }
}