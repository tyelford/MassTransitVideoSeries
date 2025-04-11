using Sample.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Sample.Components.Consumers;

public class SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger) :
    IConsumer<ISubmitOrder>
{
    public async Task Consume(ConsumeContext<ISubmitOrder> context)
    {
        logger.LogDebug("Submit order consumer: {CustomrNumber}", context.Message.CustomerNumber);
        if (context.Message.CustomerNumber.Contains("TEST"))
        {
            await context.RespondAsync<IOrderSubmitionRejected>(new
            {
                OrderId = context.Message.OrderId,
                CustomerNumber = context.Message.CustomerNumber,
                InVar.Timestamp,
                Reason = $"Test customer cannot submit orders: {context.Message.CustomerNumber}"
            });
            
            return;
        }
        await context.RespondAsync<IOrderSubmitionAccepted>(new
        {
            OrderId = context.Message.OrderId,
            CustomerNumber = context.Message.CustomerNumber,
            InVar.Timestamp
        });
    }
}