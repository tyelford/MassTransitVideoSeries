using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Sample.Components.Consumers;
using Sample.Contracts;
using Shouldly;

namespace Sample.Components.Tests;

public class when_an_order_request_is_consumed
{
    [Fact]
    public async Task should_respond_with_accepted_if_ok()
    {
        var harness = new InMemoryTestHarness();

        var nullLogger = new NullLogger<SubmitOrderConsumer>();
        var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(nullLogger));
        
        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();
            var reqClient = await harness.ConnectRequestClient<ISubmitOrder>();
            var response = await reqClient.GetResponse<IOrderSubmitionAccepted>(new
            {
                OrderId = orderId,
                Timestamp = InVar.Timestamp,
                CustomerNumber = "123456"
            });
            
            response.Message.OrderId.ShouldBe(orderId);
            
            consumer.Consumed.Select<ISubmitOrder>().Any().ShouldBeTrue();
            harness.Sent.Select<ISubmitOrder>().Any().ShouldBeTrue();
        }
        finally
        {
           await harness.Stop();
        }
    }
    
    [Fact]
    public async Task should_respond_with_rejected_if_test()
    {
        var harness = new InMemoryTestHarness();

        var nullLogger = new NullLogger<SubmitOrderConsumer>();
        var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(nullLogger));
        
        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();
            var reqClient = await harness.ConnectRequestClient<ISubmitOrder>();
            var response = await reqClient.GetResponse<IOrderSubmitionRejected>(new
            {
                OrderId = orderId,
                Timestamp = InVar.Timestamp,
                CustomerNumber = "TEST123"
            });
            
            response.Message.OrderId.ShouldBe(orderId);
            
            consumer.Consumed.Select<ISubmitOrder>().Any().ShouldBeTrue();
            harness.Sent.Select<IOrderSubmitionRejected>().Any().ShouldBeTrue();
        }
        finally
        {
           await harness.Stop();
        }
    }
    
    [Fact]
    public async Task should_consume_submitted_order_commands()
    {
        var harness = new InMemoryTestHarness();
        harness.TestTimeout = TimeSpan.FromSeconds(5);
        
        var nullLogger = new NullLogger<SubmitOrderConsumer>();
        var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(nullLogger));
        
        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();
            var reqClient = await harness.ConnectRequestClient<ISubmitOrder>();
            
            await harness.InputQueueSendEndpoint.Send<ISubmitOrder>(new
            {
                OrderId = orderId,
                Timestamp = InVar.Timestamp,
                CustomerNumber = "12345"
            });
            
            consumer.Consumed.Select<ISubmitOrder>().Any().ShouldBeTrue();
            harness.Sent.Select<IOrderSubmitionAccepted>().Any().ShouldBeFalse();
            harness.Sent.Select<IOrderSubmitionRejected>().Any().ShouldBeFalse();
        }
        finally
        {
           await harness.Stop();
        }
    }
    
    [Fact]
    public async Task should_publish_order_submitted_events()
    {
        var harness = new InMemoryTestHarness();
        harness.TestTimeout = TimeSpan.FromSeconds(5);
        
        var nullLogger = new NullLogger<SubmitOrderConsumer>();
        var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(nullLogger));
        
        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();
            
            await harness.InputQueueSendEndpoint.Send<ISubmitOrder>(new
            {
                OrderId = orderId,
                Timestamp = InVar.Timestamp,
                CustomerNumber = "12345"
            });
            
            harness.Published.Select<IOrderSubmitted>().Any().ShouldBeTrue();
        }
        finally
        {
           await harness.Stop();
        }
    }
    
    [Fact]
    public async Task should_not_publish_order_submitted_events_when_rejected()
    {
        var harness = new InMemoryTestHarness();
        harness.TestTimeout = TimeSpan.FromSeconds(5);
        
        var nullLogger = new NullLogger<SubmitOrderConsumer>();
        var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(nullLogger));
        
        await harness.Start();

        try
        {
            var orderId = NewId.NextGuid();
            
            await harness.InputQueueSendEndpoint.Send<ISubmitOrder>(new
            {
                OrderId = orderId,
                Timestamp = InVar.Timestamp,
                CustomerNumber = "TEST123"
            });
            
            harness.Published.Select<IOrderSubmitted>().Any().ShouldBeFalse();
        }
        finally
        {
           await harness.Stop();
        }
    }
}