using Sample.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(
    ILogger<OrderController> _logger
    ,IRequestClient<ISubmitOrder> _submitOrderRequestClient
    ,ISendEndpointProvider _sendEndpointProvider
    ) :ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "Hello From OrderController!";
    }
    
    
    [HttpPost]
    public async Task<IActionResult> Post(Guid id, string customerNumber)
    {
        var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<IOrderSubmitionAccepted, IOrderSubmitionRejected>(new
        {
            OrderId = id,
            Timestamp = InVar.Timestamp,
            CustomerNumber = customerNumber
        });

        if (accepted.IsCompletedSuccessfully)
        {
            var res = await accepted;
            return Accepted(res.Message);
        }
        else
        {
            var res = await rejected;
            return BadRequest(res.Message);
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> Put(Guid id, string customerNumber)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:submit-order"));

        await endpoint.Send<ISubmitOrder>(new
        {
            OrderId = id,
            Timestamp = InVar.Timestamp,
            CustomerNumber = customerNumber
        });
        
       
        return Accepted();
    }
}