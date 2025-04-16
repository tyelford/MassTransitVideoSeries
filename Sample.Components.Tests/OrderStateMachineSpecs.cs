using System.Runtime.InteropServices;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Sample.Components.Consumers;
using Sample.Components.StateMachines;
using Sample.Contracts;
using Shouldly;

namespace Sample.Components.Tests;

public class Submitting_an_order
{
    [Fact]
    public async Task should_create_a_state_instance()
    {
        var orderStateMachine = new OrderStateMachine();
        var harness = new InMemoryTestHarness();
        var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);
        
        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();
            await harness.Bus.Publish<IOrderSubmitted>(new
            {
                OrderId = orderId,
                Timestamp = InVar.Timestamp,
                CustomerNumber = "123456"
            });

            saga.Created.Select(x => x.CorrelationId == orderId).Any().ShouldBeTrue();

            var instanceId = await saga.Exists(orderId, x => x.Submitted);
            instanceId.ShouldNotBeNull();
            
            var instance = saga.Sagas.Contains(instanceId.Value);
            instance.CustomerNumber.ShouldBeEquivalentTo("123456");
        }
        finally
        {
            await harness.Stop();
        }
    }
    
    [Fact]
    public async Task should_respond_to_status_checks()
    {
        var orderStateMachine = new OrderStateMachine();
        var harness = new InMemoryTestHarness();
        var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);
        
        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();
            await harness.Bus.Publish<IOrderSubmitted>(new
            {
                OrderId = orderId,
                Timestamp = InVar.Timestamp,
                CustomerNumber = "123456"
            });

            saga.Created.Select(x => x.CorrelationId == orderId).Any().ShouldBeTrue();

            var instanceId = await saga.Exists(orderId, x => x.Submitted);
            instanceId.ShouldNotBeNull();

            var reqClient = await harness.ConnectRequestClient<ICheckOrder>();
            var resp = await reqClient.GetResponse<IOrderStatus>(new
            {
                OrderId = orderId,
            });
            
            resp.Message.Status.ShouldBeEquivalentTo(orderStateMachine.Submitted.Name);
        }
        finally
        {
            await harness.Stop();
        }
    }
}