namespace Sample.Contracts;

public interface ISubmitOrder
{
    Guid OrderId { get; }
    DateTime Timestamp  { get; }
    string CustomerNumber { get; }
}

public interface IOrderSubmitionAccepted
{
    Guid OrderId { get; }
    DateTime Timestamp { get; }
    string CustomerNumber { get; }
}

public interface IOrderSubmitionRejected
{
    Guid OrderId { get; }
    DateTime Timestamp { get; }
    string CustomerNumber { get; }
    string Reason { get; }
}