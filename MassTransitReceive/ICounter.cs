namespace MassTransitReceive
{
    public interface ICounter
    {
        int Count { get; }
        void Increment();
        void Clear();
    }
}