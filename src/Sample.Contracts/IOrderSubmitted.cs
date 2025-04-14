namespace Sample.Contracts;

public interface IOrderSubmitted
{
    Guid OrderId { get; }
    DateTime Timestamp { get; }
    string CustomerNumber { get; }
}