namespace Sample.Contracts;

public interface IOrderStatus
{
    Guid OrderId { get; }
    string Status { get; }
}